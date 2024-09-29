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

        #region Observable Properties
        [ObservableProperty]
        private int firstSelectedStandard;

        [ObservableProperty]
        private int secondSelectedStandard;

        [ObservableProperty]
        private bool isStoreAssemblyChecked;

        [ObservableProperty]
        private bool isStorePreprocessedCodeChecked;

        [ObservableProperty]
        private bool hasSelectedFile = false;

        [ObservableProperty]
        private IList<string> allStandards;
        #endregion

        #region Constructor(s)
        public MainViewModel(ProjectSettings projectSettings)
        {
            _projectSettings = projectSettings;

            // settings the values retrieved from the model
            AllStandards = _projectSettings.AllStandards;
            FirstSelectedStandard = _projectSettings.FirstSelectedStandard;
            SecondSelectedStandard = _projectSettings.SecondSelectedStandard;
            IsStoreAssemblyChecked = _projectSettings.IsStoreAssemblyChecked;
            IsStorePreprocessedCodeChecked = _projectSettings.IsStorePreprocessedCodeChecked;
        }
        #endregion

        #region Partial methods
        partial void OnFirstSelectedStandardChanged(int value)
        {
            _projectSettings.FirstSelectedStandard = value;
        }
        partial void OnSecondSelectedStandardChanged(int value)
        {
            _projectSettings.SecondSelectedStandard = value;
        }
        partial void OnIsStoreAssemblyCheckedChanged(bool value)
        {
            _projectSettings.IsStoreAssemblyChecked = value;
        }
        partial void OnIsStorePreprocessedCodeCheckedChanged(bool value)
        {
            _projectSettings.IsStorePreprocessedCodeChecked = value;
        }
        #endregion

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
            Debug.WriteLine($"Compilation settings: {AllStandards[FirstSelectedStandard]}, {AllStandards[SecondSelectedStandard]} \n" +
                $"Assembly: {IsStoreAssemblyChecked} \n" +
                $"Preprocessed: {IsStorePreprocessedCodeChecked}");
        }
        #endregion
    }
}
