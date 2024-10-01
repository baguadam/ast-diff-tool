using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
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
        private readonly IFileDialogService _fileDialogService;

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
        private string compilationDatabasePath;

        [ObservableProperty]
        private bool hasSelectedFile = false;

        [ObservableProperty]
        private IList<string> allStandards;

        [ObservableProperty]
        private string notificationMessage;

        [ObservableProperty]
        private bool isNotificationVisible;
        #endregion

        #region Constructor(s)
        public MainViewModel(ProjectSettings projectSettings, IFileDialogService fileDialogService)
        {
            _projectSettings = projectSettings;
            _fileDialogService = fileDialogService;

            // settings the values retrieved from the model
            AllStandards = _projectSettings.AllStandards;
            FirstSelectedStandard = _projectSettings.FirstSelectedStandard;
            SecondSelectedStandard = _projectSettings.SecondSelectedStandard;
            CompilationDatabasePath = _projectSettings.CompilationDatabasePath;
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
            var filePath = _fileDialogService.OpenFile("Compilation Database File (*.json)|*.json");

            if (!string.IsNullOrEmpty(filePath))
            {
                HasSelectedFile = true;
                CompilationDatabasePath = filePath;
                _projectSettings.CompilationDatabasePath = filePath;

                NotificationMessage = "File selected successfully!";
            }
            else
            {
                NotificationMessage = "Failed to select file!";
            }
                
            IsNotificationVisible = true;

            // The text is supposed to visible for 3 seconds
            Task.Delay(5000).ContinueWith(_ => IsNotificationVisible = false);
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
                $"Preprocessed: {IsStorePreprocessedCodeChecked} \n" +
                $"Compilation Database path: {CompilationDatabasePath}");
        }
        #endregion
    }
}
