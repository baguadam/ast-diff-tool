using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.Shared;
using ASTDiffTool.ViewModels;
using ASTDiffTool.ViewModels.Events;
using ASTDiffTool.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    /// <summary>
    /// ViewModel responsible for managing the creation and compilation of a new project.
    /// </summary>
    public partial class NewProjectPageViewModel : ViewModelBase
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly ICPlusPlusService _cPlusPlusService;
        private readonly IEventAggregator _eventAggregator;

        private readonly NewProjectModel _projectModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectPageViewModel"/> class.
        /// </summary>
        /// <param name="fileDialogService">Service for opening file dialogs.</param>
        /// <param name="cPlusPlusService">Service responsible for compiling and comparing ASTs.</param>
        /// <param name="eventAggregator">Service to publish events within the application.</param>
        /// <param name="projectModel">Model that represents the project data.</param>
        public NewProjectPageViewModel(
            IFileDialogService fileDialogService,
            ICPlusPlusService cPlusPlusService,
            IEventAggregator eventAggregator,
            NewProjectModel projectModel)
        {
            _fileDialogService = fileDialogService;
            _cPlusPlusService = cPlusPlusService;
            _eventAggregator = eventAggregator;

            // initialize Model
            _projectModel = projectModel;

            // initialize available C++ standards
            AllStandards = new List<string> { "c++98", "c++03", "c++11", "c++14", "c++17", "c++20" };
        }

        #region Properties
        /// <summary>
        /// The path to the compilation database file.
        /// </summary>
        public string CompilationDatabasePath
        {
            get => _projectModel.CompilationDatabasePath;
            set
            {
                if (_projectModel.CompilationDatabasePath != value)
                {
                    _projectModel.CompilationDatabasePath = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCompile));
                }
            }
        }

        /// <summary>
        /// The path to the main file of the project.
        /// </summary>
        public string MainFilePath
        {
            get => _projectModel.MainFilePath;
            set
            {
                if (_projectModel.MainFilePath != value)
                {
                    _projectModel.MainFilePath = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCompile));
                }
            }
        }

        /// <summary>
        /// The name of the project. 
        /// </summary>
        public string ProjectName
        {
            get => _projectModel.ProjectName;
            set
            {
                if (_projectModel.ProjectName != value)
                {
                    _projectModel.ProjectName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCompile));
                }
            }
        }

        /// <summary>
        /// The first standard that is selected for AST dump.
        /// </summary>
        public string FirstSelectedStandard
        {
            get => _projectModel.FirstSelectedStandard;
            set
            {
                if (_projectModel.FirstSelectedStandard != value)
                {
                    _projectModel.FirstSelectedStandard = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCompile));
                }
            }
        }

        /// <summary>
        /// The second standard that is selected for AST dump.
        /// </summary>
        public string SecondSelectedStandard
        {
            get => _projectModel.SecondSelectedStandard;
            set
            {
                if (_projectModel.SecondSelectedStandard != value)
                {
                    _projectModel.SecondSelectedStandard = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCompile));
                }
            }
        }

        /// <summary>
        /// The path of the folder that contains the dumped ASTs and logs.
        /// </summary>
        public string ProjectResultPath
        {
            get => _projectModel.ProjectResultPath;
            set
            {
                if (_projectModel.ProjectResultPath != value)
                {
                    _projectModel.ProjectResultPath = value;
                    OnPropertyChanged(nameof(ProjectResultPath));
                }
            }
        }

        /// <summary>
        /// List of the available standards for selection.
        /// </summary>
        public List<string> AllStandards { get; }

        /// <summary>
        /// Indicates whether the project is ready for compilation.
        /// </summary>
        public bool CanCompile => _projectModel.IsComplete;

        private bool _isLoading;
        /// <summary>
        /// Indicates whether the project is loading.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        private string _notificationMessage;
        /// <summary>
        /// The notification message that is displayed in a popup.
        /// </summary>
        public string NotificationMessage
        {
            get => _notificationMessage;
            set
            {
                if (_notificationMessage != value)
                {
                    _notificationMessage = value;
                    OnPropertyChanged(nameof(NotificationMessage));
                }
            }
        }

        private bool _isNotificationVisible;
        /// <summary>
        /// Indicates whether the notification should be displayed.
        /// </summary>
        public bool IsNotificationVisible
        {
            get => _isNotificationVisible;
            set
            {
                if (_isNotificationVisible != value)
                {
                    _isNotificationVisible = value;
                    OnPropertyChanged(nameof(IsNotificationVisible));
                }
            }
        }

        private bool _isProjectCompiled;
        /// <summary>
        /// Indicates whether compilation has completed successfully.
        /// </summary>
        public bool IsProjectCompiled
        {
            get => _isProjectCompiled;
            set
            {
                if (_isProjectCompiled != value) 
                {
                    _isProjectCompiled = value;
                    OnPropertyChanged(nameof(IsProjectCompiled));
                }
            }
        }

        private string _cPlusPlusToolState;
        /// <summary>
        /// The state of the CPlusPlus tool.
        /// </summary>
        public string CPlusPlusToolState
        {
            get => _cPlusPlusToolState;
            set
            {
                if (_cPlusPlusToolState != value)
                {
                    _cPlusPlusToolState = value;
                    OnPropertyChanged(nameof(CPlusPlusToolState));
                } 
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Command to select the compilation database.
        /// </summary>
        [RelayCommand]
        public void SelectCompilationDatabase()
        {
            string? filePath = _fileDialogService.OpenFile("Compilation Database File (*.json)|*.json");
            if (!string.IsNullOrEmpty(filePath))
            {
                CompilationDatabasePath = filePath;
            }
        }

        /// <summary>
        /// Command to select the main project file.
        /// </summary>
        [RelayCommand]
        public void SelectMainFile()
        {
            string? filePath = _fileDialogService.OpenFile("C++ Source File (*.cpp)|*.cpp");
            if (!string.IsNullOrEmpty(filePath))
            {
                MainFilePath = filePath;
            }
        }

        /// <summary>
        /// Command to compile the project, runs the C++ tools with the selected parameters
        /// </summary>
        [RelayCommand]
        public async Task CompileProject()
        {
            if (!CanCompile)
            {
                await ShowNotification("Please ensure all inputs are provided before compiling.", false);
                return;
            }

            IsLoading = true;

            try
            {
                // Step 1: Running AST Dump Tool
                CPlusPlusToolState = "Dumping ASTs of the trees...";
                bool isDumpSuccessful = await RunCPlusPlusToolAsync(() =>
                    _cPlusPlusService.RunASTDumpTool(
                        _projectModel.CompilationDatabasePath,
                        _projectModel.MainFilePath,
                        _projectModel.ProjectName,
                        _projectModel.FirstSelectedStandard,
                        _projectModel.SecondSelectedStandard));

                ProjectResultPath = _cPlusPlusService.ProjectResultPath; // path is constructed at the beginning

                if (!isDumpSuccessful)
                {
                    await ShowNotification($"Dump Tool failed! See logs: {ProjectResultPath}", false);
                    return;
                }

                // Step 2: Running AST Tree Comparer Tool
                CPlusPlusToolState = "Comparing ASTs and writing results...";
                bool isComparerSuccessful = await RunCPlusPlusToolAsync(() =>
                    _cPlusPlusService.RunComparerTool(
                        _projectModel.FirstSelectedStandard,
                        _projectModel.SecondSelectedStandard));

                if (!isComparerSuccessful)
                {
                    await ShowNotification($"Comparer Tool failed! See logs: {ProjectResultPath}", false);
                    return;
                }

                // if both succeeded
                var projectCompilationEvent = new ProjectCompilationEvent(true);
                _eventAggregator.Publish(projectCompilationEvent);

                await ShowNotification("Compilation completed successfully!", true);
                IsProjectCompiled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during compilation: {ex.Message}");
                await ShowNotification($"An error occurred during compilation! See logs: {ProjectResultPath} \n Error: {ex.Message}", false);
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Runs the C++ tools async.
        /// </summary>
        /// <param name="toolAction">Delegate to run the necessary tool</param>
        /// <returns></returns>
        private async Task<bool> RunCPlusPlusToolAsync(Func<bool> toolAction)
        {
            try
            {
                return await Task.Run(toolAction);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Tool execution error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Helper method to set the notification values.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="success">Whether it is success or fail</param>
        private async Task ShowNotification(string message, bool success)
        {
            IsLoading = false; // stop loading

            NotificationMessage = message;
            IsProjectCompiled = success;
            IsNotificationVisible = true;
            await Task.Delay(3000);
            IsNotificationVisible = false;
        }
        #endregion
    }
}