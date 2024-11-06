using ASTDiffTool.Models;
using ASTDiffTool.Services;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.ViewModels.Events;
using ASTDiffTool.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class NewProjectPageViewModel(IFileDialogService fileDialogService,
        INavigationService navigationService,
        IEventAggregator eventAggregator,
        IDatabaseConnectionService connectionService,
        ProjectSettings projectSettings) : ViewModelBase
    {
        private readonly string DB_PATH = "diff.db3";

        private readonly ProjectSettings _projectSettings = projectSettings;
        private readonly IFileDialogService _fileDialogService = fileDialogService;
        private readonly INavigationService _navigationService = navigationService;
        private readonly IEventAggregator _eventAggregator = eventAggregator;
        private readonly IDatabaseConnectionService _connectionService = connectionService;

        private bool _hasSelectedFile = false;
        private string _notificationMessage;
        private bool _isNotificationVisible;

        #region Properties
        public int FirstSelectedStandard
        {
            get => _projectSettings.FirstSelectedStandard;
            set
            {
                _projectSettings.FirstSelectedStandard = value;
                OnPropertyChanged(nameof(FirstSelectedStandard));
            }
        }

        public int SecondSelectedStandard
        {
            get => _projectSettings.SecondSelectedStandard;
            set
            {
                _projectSettings.SecondSelectedStandard = value;
                OnPropertyChanged(nameof(SecondSelectedStandard));
            }
        }

        public bool IsStoreAssemblyChecked
        {
            get => _projectSettings.IsStoreAssemblyChecked;
            set
            {
                _projectSettings.IsStoreAssemblyChecked = value;
                OnPropertyChanged(nameof(IsStoreAssemblyChecked));
            }
        }

        public bool IsStorePreprocessedCodeChecked
        {
            get => _projectSettings.IsStorePreprocessedCodeChecked;
            set
            {
                _projectSettings.IsStorePreprocessedCodeChecked = value;
                OnPropertyChanged(nameof(IsStorePreprocessedCodeChecked));
            }
        }

        public string CompilationDatabasePath
        {
            get => _projectSettings.CompilationDatabasePath;
            set
            {
                _projectSettings.CompilationDatabasePath = value;
                OnPropertyChanged(nameof(CompilationDatabasePath));
            }
        }

        public bool HasSelectedFile
        {
            get => _hasSelectedFile;
            set
            {
                _hasSelectedFile = value;
                OnPropertyChanged(nameof(HasSelectedFile));
            }
        }

        public IList<string> AllStandards
        {
            get => _projectSettings.AllStandards;
        }

        public string NotificationMessage
        {
            get => _notificationMessage;
            set
            {
                _notificationMessage = value;
                OnPropertyChanged(nameof(NotificationMessage));
            }
        }

        public bool IsNotificationVisible
        {
            get => _isNotificationVisible;
            set
            {
                _isNotificationVisible = value;
                OnPropertyChanged(nameof(IsNotificationVisible));
            }
        }

        #endregion

        #region Commands
        /// <summary>
        /// Command to handle loading the compilation database for the project
        /// </summary>
        [RelayCommand]
        public void LoadCompilationDatabase()
        {
            string? filePath = _fileDialogService.OpenFile("Compilation Database File (*.json)|*.json");

            if (!string.IsNullOrEmpty(filePath))
            {
                HasSelectedFile = true;
                CompilationDatabasePath = filePath;
                _projectSettings.CompilationDatabasePath = filePath;

                // set the database path
                string? directory = Path.GetDirectoryName(filePath);
                string databasePath = Path.Combine(filePath, DB_PATH);
                _connectionService.UpdateConnectionString(databasePath);

                NotificationMessage = "File selected successfully!";
            }
            else
            {
                NotificationMessage = "Failed to select file!";
            }

            IsNotificationVisible = true;

            // The text is supposed to visible for 3 seconds
            _ = Task.Delay(5000).ContinueWith(_ => IsNotificationVisible = false);
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

            bool isSuccessful = true; // right now mocking the compilation with a true value here
            // publishing an event that contains the result of the compilation
            var projectCompilationEvent = new ProjectCompilationEvent(isSuccessful);
            _eventAggregator.Publish(projectCompilationEvent);

            if (isSuccessful)
            {
                _navigationService.NavigateTo<ASTPageViewModel>(); // in case of successful compilation, navigate to AST View            
            }
        }
        #endregion
    }
}
