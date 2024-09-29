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
    public partial class MainViewModel : ObservableRecipient
    {
        private readonly ProjectSettings _projectSettings;
        private const int INDEX_FOR_FIRST = 0;
        private const int INDEX_FOR_SECOND = 1;

        #region Observable Properties
        [ObservableProperty]
        private int firstSelectedStandard = INDEX_FOR_FIRST;

        [ObservableProperty]
        private int secondSelectedStandard = INDEX_FOR_SECOND;

        [ObservableProperty]
        private IEnumerable<string> allStandards;
        #endregion

        public MainViewModel(ProjectSettings projectSettings)
        {
            _projectSettings = projectSettings;
            AllStandards = _projectSettings.AllStandards;
        }

        #region Commands
        /// <summary>
        /// Command to handle loading the compilation database for the project
        /// </summary>
        [RelayCommand]
        public void LoadCompilationDatabase()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Command to handle loading a saved project
        /// </summary>
        [RelayCommand]
        public void LoadSavedProject()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Command to handle compiling the loaded C++ project
        /// </summary>
        [RelayCommand]
        public void CompileProject()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
