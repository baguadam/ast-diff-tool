using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.Shared;
using ASTDiffTool.ViewModels;
using ASTDiffTool.ViewModels.Events;
using ASTDiffTool.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ASTDiffTool.ViewModels
{
    /// <summary>
    /// ViewModel responsible for handling the functionalities for TreeDisplayPage.
    /// </summary>
    public partial class TreeDisplayPageViewModel : ViewModelBase
    {
        private readonly INeo4jService _neo4jService;
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        /// <param name="neo4jService">Service that is responsible for interacting the Neo4j database</param>
        public TreeDisplayPageViewModel(INeo4jService neo4jService, INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _neo4jService = neo4jService;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            DifferenceTypes = new ObservableCollection<Differences>(Enum.GetValues(typeof(Differences)).Cast<Differences>());
            SelectedDifferenceType = Differences.ONLY_IN_FIRST_AST; // default selection

            _eventAggregator.Subscribe<ProjectCompilationEvent>(HandleProjectCompiled);
            Task.Run(LoadNodesAsync);
        }

        #region Properties
        /// <summary>
        /// Collection of possible difference types.
        /// </summary>
        public ObservableCollection<Differences> DifferenceTypes { get; }

        private Differences _selectedDifferenceType;
        /// <summary>
        /// The currently selected difference type that is used to filter the nodes.
        /// </summary>
        public Differences SelectedDifferenceType
        {
            get => _selectedDifferenceType;
            set
            {
                if (_selectedDifferenceType != value)
                {
                    _selectedDifferenceType = value;
                    OnPropertyChanged(nameof(SelectedDifferenceType));
                    CurrentPage = 1; // reset page when filter changes

                    Task.Run(LoadNodesAsync);
                }
            }
        }

        private ObservableCollection<Node> _currentNodes = new();
        /// <summary>
        /// Collection of the currently displayed nodes on the page.
        /// </summary>
        public ObservableCollection<Node> CurrentNodes
        {
            get => _currentNodes;
            set
            {
                if (_currentNodes != value)
                {
                    _currentNodes = value;
                    OnPropertyChanged(nameof(CurrentNodes));
                }
            }
        }

        private bool _isLoading;
        /// <summary>
        /// Indicates whether the nodes are currently being queried.
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

        private int _currentPage = 1;
        /// <summary>
        /// The number of the current page.
        /// </summary>
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                    UpdatePageInfo();
                }
            }
        }

        private string _currentPageInfo = "Page 1";
        /// <summary>
        /// Displayed between the pagination buttons, info about the page.
        /// </summary>
        public string CurrentPageInfo
        {
            get => _currentPageInfo;
            set
            {
                if (_currentPageInfo != value)
                {
                    _currentPageInfo = value;
                    OnPropertyChanged(nameof(CurrentPageInfo));
                }
            }
        }

        private bool _canGoToNextPage = true;
        /// <summary>
        /// Indicated whether the user can navigate to the next page.
        /// </summary>
        public bool CanGoToNextPage
        {
            get => _canGoToNextPage;
            set
            {
                if (_canGoToNextPage != value)
                {
                    _canGoToNextPage = value;
                    OnPropertyChanged(nameof(CanGoToNextPage));
                }
            }
        }

        private bool _canGoToPreviousPage = false;
        /// <summary>
        /// Indicates whether the user can navigate to the previous page.
        /// </summary>
        public bool CanGoToPreviousPage
        {
            get => _canGoToPreviousPage;
            set
            {
                if (_canGoToPreviousPage != value)
                {
                    _canGoToPreviousPage = value;
                    OnPropertyChanged(nameof(CanGoToPreviousPage));
                }
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Loads the node from the Neo4j database based on the selected difference type, updates pagination.
        /// </summary>
        [RelayCommand]
        public async Task LoadNodesAsync()
        {
            IsLoading = true;

            try
            {
                List<Node> nodes;

                // fetch nodes based on the selected types
                if (SelectedDifferenceType == Differences.ONLY_IN_FIRST_AST || SelectedDifferenceType == Differences.ONLY_IN_SECOND_AST)
                {
                    nodes = await _neo4jService.GetHighestLevelSubtreesAsync(SelectedDifferenceType, CurrentPage);
                }
                else
                {
                    nodes = await _neo4jService.GetFlatNodesByDifferenceTypeAsync(SelectedDifferenceType, CurrentPage);
                }

                // pagination control update
                CanGoToPreviousPage = CurrentPage > 1;
                CanGoToNextPage = nodes.Count >= 20;

                // observable collections for the tree structure
                CurrentNodes = new ObservableCollection<Node>(nodes);
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

        /// <summary>
        /// Command navigate to the next page.
        /// </summary>
        [RelayCommand]
        private async Task GoToNextPage()
        {
            if (CanGoToNextPage)
            {
                CurrentPage++;
                await LoadNodesAsync();
            }
        }

        /// <summary>
        /// Command navigate to the previous page.
        /// </summary>
        [RelayCommand]
        private async Task GoToPreviousPage()
        {
            if (CanGoToPreviousPage)
            {
                CurrentPage--;
                await LoadNodesAsync();
            }
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// Helper method to update the information about the current page.
        /// </summary>
        private void UpdatePageInfo()
        {
            CurrentPageInfo = $"Page {CurrentPage}";
        }

        /// <summary>
        /// Handles the project compilation event, in case of success, retrieves the nodes from the database. 
        /// </summary>
        /// <param name="compilationEvent">Event containing the result of the project compilation.</param>
        private async void HandleProjectCompiled(ProjectCompilationEvent compilationEvent)
        {
            if (compilationEvent.IsSuccessful)
            {
                await LoadNodesAsync();
            }
        }
        #endregion
    }
}