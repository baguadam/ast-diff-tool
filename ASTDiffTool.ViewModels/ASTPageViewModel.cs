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

        public ASTPageViewModel(Project project)
        {
            _project = project;
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
    }
}
