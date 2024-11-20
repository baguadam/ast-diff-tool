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
        #endregion

        #region Commands
        [RelayCommand]
        public async Task LoadDatabaseInfoAsync()
        {
            try
            {
                TotalNodeCount = await _neo4jService.GetNodeCountAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading database info: {ex.Message}");
            }
        }
        #endregion
    }
}
