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

            InitializeAsync(); // initialize at creation
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
        public async Task LoadDatabaseInfoAsync()
        {
            await InitializeAsync();
        }
        #endregion

        private async Task InitializeAsync()
        {
            IsLoading = true;

            try
            {
                // Query initial data
                TotalNodeCount = await _neo4jService.GetNodeCountAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load database information: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
