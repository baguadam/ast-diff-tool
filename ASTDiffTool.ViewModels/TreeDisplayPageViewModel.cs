using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASTDiffTool.ViewModels
{
    public partial class TreeDisplayPageViewModel : ViewModelBase
    {
        private readonly INeo4jService _neo4jService;

        public TreeDisplayPageViewModel(INeo4jService neo4jService)
        {
            _neo4jService = neo4jService;
            DifferenceTypes = new ObservableCollection<Differences>(Enum.GetValues(typeof(Differences)).Cast<Differences>());
            SelectedDifferenceType = Differences.ONLY_IN_FIRST_AST; // default selection

            Task.Run(LoadNodesAsync);
        }

        #region Properties

        public ObservableCollection<Differences> DifferenceTypes { get; }

        private Differences _selectedDifferenceType;
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
        [RelayCommand]
        private async Task LoadNodesAsync()
        {
            IsLoading = true;

            try
            {
                List<Node> nodes;

                // Fetch nodes based on the selected difference type
                if (SelectedDifferenceType == Differences.ONLY_IN_FIRST_AST || SelectedDifferenceType == Differences.ONLY_IN_SECOND_AST)
                {
                    nodes = await _neo4jService.GetHighestLevelSubtreesAsync(SelectedDifferenceType, CurrentPage - 1);
                }
                else
                {
                    nodes = await _neo4jService.GetFlatNodesByDifferenceTypeAsync(SelectedDifferenceType, CurrentPage - 1);
                }

                // Update pagination controls
                CanGoToPreviousPage = CurrentPage > 1;
                CanGoToNextPage = nodes.Count >= 100;

                // Assign nodes directly if they are already hierarchical
                CurrentNodes = new ObservableCollection<Node>(nodes);

                // DEBUG
                foreach (var node in CurrentNodes)
                {
                    Debug.WriteLine($"Node: {node.EnhancedKey} - topological order: {node.TopologicalOrder}, Children Count: {node.Children.Count}");
                    foreach (var child in node.Children)
                    {
                        Debug.WriteLine($"  Child: {child.EnhancedKey}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading nodes: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void GoToNextPage()
        {
            if (CanGoToNextPage)
            {
                CurrentPage++;
                Task.Run(LoadNodesAsync);
            }
        }

        [RelayCommand]
        private void GoToPreviousPage()
        {
            if (CanGoToPreviousPage)
            {
                CurrentPage--;
                LoadNodesCommand.Execute(null);
            }
        }

        #endregion

        #region Private Helpers

        private void UpdatePageInfo()
        {
            CurrentPageInfo = $"Page {CurrentPage}";
        }

        #endregion
    }
}