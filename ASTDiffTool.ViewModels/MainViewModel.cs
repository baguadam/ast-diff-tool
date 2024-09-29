using ASTDiffTool.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly ProjectSettings _projectSettings;

        private int _firstSelectedStandard;
        private int _secondSelectedStandard;
        private IEnumerable<string> _allStandards;

        #region Observable Properties
        public int FirstSelectedStandard
        {
            get => _firstSelectedStandard;
            set
            {
                _projectSettings.UpdateSelectedStandards(value, SecondSelectedStandard);
                OnPropertyChanged(nameof(FirstSelectedStandard));
            }
        }
        public int SecondSelectedStandard
        {
            get => _secondSelectedStandard;
            set
            {
                _projectSettings.UpdateSelectedStandards(FirstSelectedStandard, value);
                OnPropertyChanged(nameof(SecondSelectedStandard));
            }
        }
        public IEnumerable<string> AllStandards
        {
            get => _allStandards;
            private set
            {
                if (_allStandards != value)
                {
                    _allStandards = value.ToList();
                    OnPropertyChanged(nameof(AllStandards));
                }
            }
        }
        #endregion

        public MainViewModel(ProjectSettings projectSettings)
        {
            _projectSettings = projectSettings;
            FirstSelectedStandard = _projectSettings.FirstSelectedStandard;
            SecondSelectedStandard = _projectSettings.SecondSelectedStandard;
            AllStandards = _projectSettings.AllStandards;
        }

        #region Commands
        /// <summary>
        /// Command to handle loading the compilation database for the project
        /// </summary>
        [RelayCommand]
        public void LoadCompilationDatabase()
        {
        }

        /// <summary>
        /// Command to handle loading a saved project
        /// </summary>
        [RelayCommand]
        public void LoadSavedProject()
        {
        }

        /// <summary>
        /// Command to handle compiling the loaded C++ project
        /// </summary>
        [RelayCommand]
        public void CompileProject()
        {
        }
        #endregion
    }
}
