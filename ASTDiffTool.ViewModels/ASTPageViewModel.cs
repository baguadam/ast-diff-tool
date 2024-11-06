using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class ASTPageViewModel : ViewModelBase
    {
        private readonly Project _project;
        private readonly IFileService _fileService;
        private readonly IDatabaseConnectionService _connectionService;

        private ObservableCollection<Node> _highestLevelNode;

        #region Properties
        public ObservableCollection<Node> HighestLevelNodes
        {
            get => _highestLevelNode;
            set
            {
                _highestLevelNode = value;
                OnPropertyChanged(nameof(_highestLevelNode));
            }
        }

        public int NumberOfDifferences
        {
            get => _project.NumberOfDifferences;
            set
            {
                _project.NumberOfDifferences = value;
                OnPropertyChanged(nameof(NumberOfDifferences));
            }
        }
        #endregion

        public ASTPageViewModel(IFileService fileService, IDatabaseConnectionService connectionService, Project project)
        {
            _project = project;
            _fileService = fileService;
            _connectionService = connectionService;
        }

        [RelayCommand]
        public void JumpToSourceCode()
        {
            Debug.WriteLine($"Line is clicked!");
        }

        private IList<Node> GetHighestLevelNodesWithSubtrees()
        {
            using (var dbContext = _connectionService.Create())
            {
                var highestLevelNodes = dbContext.Nodes.Where(n => n.IsHighestLevelNode).ToList();

                foreach (var node in highestLevelNodes)
                {

                }
            }

            return null;
        }
    }
}
