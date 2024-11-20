using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class ProjectPageViewModel : ViewModelBase
    {
        private readonly INeo4jService _neo4jService;
        private readonly ProjectDatabaseInfoModel _databaseInfo;

        public ProjectPageViewModel(INeo4jService neo4jService)
        {
            _neo4jService = neo4jService;
            _databaseInfo = new ProjectDatabaseInfoModel();

            Task.Run(LoadDatabaseInfoAsync);
        }

        #region Properties
        private int _totalNodeCount;
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
        [RelayCommand]
        private async Task LoadDatabaseInfoAsync()
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
                Debug.WriteLine($"Error loading database info: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion
    }
}