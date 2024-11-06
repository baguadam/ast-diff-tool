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

        private ObservableCollection<LineModel> _firstStandardLines;
        private ObservableCollection<LineModel> _secondStandardLines;

        #region Properties
        public ObservableCollection<LineModel> FirstStandardLines
        {
            get => _firstStandardLines;
            set
            {
                _firstStandardLines = value;
                OnPropertyChanged(nameof(FirstStandardLines));
            }
        }

        public ObservableCollection<LineModel> SecondStandardLines
        {
            get => _secondStandardLines;
            set
            {
                _secondStandardLines = value;
                OnPropertyChanged(nameof(_secondStandardLines));
            }
        }
        #endregion

        public ASTPageViewModel(IFileService fileService, IDatabaseConnectionService connectionService, Project project)
        {
            _project = project;
            _fileService = fileService;
            _connectionService = connectionService;

            GetAllNodes();
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

        [RelayCommand]
        public void JumpToSourceCode()
        {
            Debug.WriteLine($"Line is clicked!");
        }

        private async void ReadASTsFromFile()
        {
            var firstStandardData = await _fileService.ReadLinesFromFileAsync("C:\\Users\\bagua\\OneDrive - Eotvos Lorand Tudomanyegyetem\\Ádám\\Egyetem - 07\\SZAKDOLGOZAT\\ASTDiffTool\\asts\\vector1.txt");
            var secondStandardData = await _fileService.ReadLinesFromFileAsync("C:\\Users\\bagua\\OneDrive - Eotvos Lorand Tudomanyegyetem\\Ádám\\Egyetem - 07\\SZAKDOLGOZAT\\ASTDiffTool\\asts\\vector2.txt");

            FirstStandardLines = new ObservableCollection<LineModel>(firstStandardData);
            SecondStandardLines = new ObservableCollection<LineModel>(secondStandardData);
        }

        private async void GetAllNodes()
        {
            using (var dbContext = _connectionService.Create())
            {
                // ....
            }
        }
    }
}
