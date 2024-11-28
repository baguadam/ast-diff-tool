using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.ViewModels.Events;
using ASTDiffTool.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    /// <summary>
    /// ViewModel for managing information about the current project, related to the ProjectPage.
    /// </summary>
    public partial class ProjectPageViewModel : ViewModelBase
    {
        private readonly INeo4jService _neo4jService;
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ProjectDatabaseInfoModel _databaseInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectPageViewModel"/> class.
        /// </summary>
        /// <param name="neo4jService">Service responsible for interacting with the Neo4j database.</param>
        public ProjectPageViewModel(INeo4jService neo4jService, INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _neo4jService = neo4jService;
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            _databaseInfo = new ProjectDatabaseInfoModel();

            _eventAggregator.Subscribe<ProjectCompilationEvent>(HandleProjectCompiled);
            Task.Run(LoadDatabaseInfoAsync);
        }

        #region Properties
        private int _totalNodeCount;
        /// <summary>
        /// Total count of nodes in the database.
        /// </summary>
        public int TotalNodeCount
        {
            get => _totalNodeCount;
            set
            {
                if (_totalNodeCount != value)
                {
                    _totalNodeCount = value;
                    OnPropertyChanged(nameof(TotalNodeCount));
                }
            }
        }

        private int _nodesInFirstAST;
        /// <summary>
        /// Count of the nodes related to the first AST.
        /// </summary>
        public int NodesInFirstAST
        {
            get => _nodesInFirstAST;
            set
            {
                if (_nodesInFirstAST != value)
                {
                    _nodesInFirstAST = value;
                    OnPropertyChanged(nameof(NodesInFirstAST));
                }
            }
        }

        private int _nodesInSecondAST;
        /// <summary>
        /// Count of the nodes related to the second AST.
        /// </summary>
        public int NodesInSecondAST
        {
            get => _nodesInSecondAST;
            set
            {
                if (_nodesInSecondAST != value)
                {
                    _nodesInSecondAST = value;
                    OnPropertyChanged(nameof(NodesInSecondAST));
                }
            }
        }

        private int _onlyInFirstAST;
        /// <summary>
        /// Count of the nodes only present in the first AST.
        /// </summary>
        public int OnlyInFirstAST
        {
            get => _onlyInFirstAST;
            set
            {
                if (_onlyInFirstAST != value)
                {
                    _onlyInFirstAST = value;
                    OnPropertyChanged(nameof(OnlyInFirstAST));
                }
            }
        }

        private int _onlyInSecondAST;
        /// <summary>
        /// Count of the nodes only present in the second AST.
        /// </summary>
        public int OnlyInSecondAST
        {
            get => _onlyInSecondAST;
            set
            {
                if (_onlyInSecondAST != value)
                {
                    _onlyInSecondAST = value;
                    OnPropertyChanged(nameof(OnlyInSecondAST));
                }
            }
        }

        private int _differentParents;
        /// <summary>
        /// Count of the nodes with different parents.
        /// </summary>
        public int DifferentParents
        {
            get => _differentParents;
            set
            {
                if (_differentParents != value)
                {
                    _differentParents = value;
                    OnPropertyChanged(nameof(DifferentParents));
                }
            }
        }

        private int _differentSourceLocations;
        /// <summary>
        /// Count of the nodes with different source locations.
        /// </summary>
        public int DifferentSourceLocations
        {
            get => _differentSourceLocations;
            set
            {
                if (value != _differentSourceLocations)
                {
                    _differentSourceLocations = value;
                    OnPropertyChanged(nameof(DifferentSourceLocations));
                }
            }
        }

        private bool _isLoading;
        /// <summary>
        /// Indicates whether the nodes are being queried.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Command to load information about the database and update the properties
        /// </summary>
        [RelayCommand]
        public async Task LoadDatabaseInfoAsync()
        {
            IsLoading = true;

            try
            {
                // update the internal model
                _databaseInfo.TotalNodeCount = await _neo4jService.GetNodeCountAsync();
                _databaseInfo.NodesInFirstAST = await _neo4jService.GetNodesByAstOriginAsync(Shared.ASTOrigins.FIRST_AST);
                _databaseInfo.NodesInSecondAST = await _neo4jService.GetNodesByAstOriginAsync(Shared.ASTOrigins.SECOND_AST);
                _databaseInfo.OnlyInFirstAST = await _neo4jService.GetNodesByDifferenceTypeAsync(Shared.Differences.ONLY_IN_FIRST_AST);
                _databaseInfo.OnlyInSecondAST = await _neo4jService.GetNodesByDifferenceTypeAsync(Shared.Differences.ONLY_IN_SECOND_AST);
                _databaseInfo.DifferentParents = await _neo4jService.GetNodesByDifferenceTypeAsync(Shared.Differences.DIFFERENT_PARENTS);
                _databaseInfo.DifferentSourceLocations = await _neo4jService.GetNodesByDifferenceTypeAsync(Shared.Differences.DIFFERENT_SOURCE_LOCATIONS);

                // reflect data into ViewModel properties
                TotalNodeCount = _databaseInfo.TotalNodeCount;
                NodesInFirstAST = _databaseInfo.NodesInFirstAST;
                NodesInSecondAST = _databaseInfo.NodesInSecondAST;
                OnlyInFirstAST = _databaseInfo.OnlyInFirstAST;
                OnlyInSecondAST = _databaseInfo.OnlyInSecondAST;
                DifferentParents = _databaseInfo.DifferentParents;
                DifferentSourceLocations = _databaseInfo.DifferentSourceLocations;
            }
            catch (Exception ex)
            {
                var databaseOperationFailure = new DatabaseFailureEvent($"Database connection is lost... \n {ex.Message}");
                _eventAggregator.Publish(databaseOperationFailure);
                _navigationService.NavigateTo<NewProjectPageViewModel>();
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Handles the project compilation event, in case of success, retrieves the nodes from the database. 
        /// </summary>
        /// <param name="compilationEvent">Event containing the result of the project compilation.</param>
        private async void HandleProjectCompiled(ProjectCompilationEvent compilationEvent)
        {
            if (compilationEvent.IsSuccessful)
            {
                await LoadDatabaseInfoAsync();
            }
        }
        #endregion
    }
}