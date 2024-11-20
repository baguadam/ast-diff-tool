using ASTDiffTool.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class ProjectPageViewModel : ViewModelBase
    {
        private readonly INeo4jService _neo4jService;

        public ProjectPageViewModel(INeo4jService neo4jService)
        {
            _neo4jService = neo4jService;

            Task.Run(async () =>
            {
                await LoadDatabaseInfoAsync(); // initialize at creation
            });
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
                if (value != _nodesInFirstAST)
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
                if (value != _nodesInSecondAST)
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
                if (value != _onlyInFirstAST)
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
                if (value != _onlyInSecondAST)
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
                if (value != _differentParents)
                {
                    _differentParents = value;
                    OnPropertyChanged(nameof(DifferentParents));
                }
            }
        }

        private int _differentSourceLocation;
        public int DifferentSourceLocations
        {
            get => _differentSourceLocation;
            set
            {
                if (value != _differentSourceLocation)
                {
                    _differentSourceLocation = value;
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
                TotalNodeCount = await _neo4jService.GetNodeCountAsync();
                NodesInFirstAST = await _neo4jService.GetNodesByAstOriginAsync(Shared.ASTOrigins.FIRST_AST);
                NodesInSecondAST = await _neo4jService.GetNodesByAstOriginAsync(Shared.ASTOrigins.SECOND_AST);
                OnlyInFirstAST = await _neo4jService.GetNodesByDifferenceTypeAsync(Shared.Differences.ONLY_IN_FIRST_AST);
                OnlyInSecondAST = await _neo4jService.GetNodesByDifferenceTypeAsync(Shared.Differences.ONLY_IN_SECOND_AST);
                DifferentParents = await _neo4jService.GetNodesByDifferenceTypeAsync(Shared.Differences.DIFFERENT_PARENTS);
                DifferentSourceLocations = await _neo4jService.GetNodesByDifferenceTypeAsync(Shared.Differences.DIFFERENT_SOURCE_LOCATIONS);
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
