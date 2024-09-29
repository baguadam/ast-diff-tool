﻿using ASTDiffTool.Models;
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
        #endregion

        public MainViewModel(ProjectSettings projectSettings)
        {
            _projectSettings = projectSettings;
            FirstSelectedStandard = _projectSettings.FirstSelectedStandard;
            SecondSelectedStandard = _projectSettings.SecondSelectedStandard;
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

        /// <summary>
        /// Handling combobox changes for selecting the two standards for compilation
        /// </summary>
        /// <param name="selectedStandard"></param>
        [RelayCommand]
        public void OnStandardSelectionChanged(int selectedStandardIndex)
        {
            Debug.WriteLine($"ITT: {selectedStandardIndex}");
        }
        #endregion
    }
}
