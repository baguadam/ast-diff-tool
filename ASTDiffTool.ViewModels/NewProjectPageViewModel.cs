using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class NewProjectPageViewModel : ViewModelBase
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly ICPlusPlusService _cPlusPlusService;

        private readonly NewProjectModel _projectModel;

        public NewProjectPageViewModel(
            IFileDialogService fileDialogService,
            ICPlusPlusService cPlusPlusService,
            NewProjectModel projectModel)
        {
            _fileDialogService = fileDialogService;
            _cPlusPlusService = cPlusPlusService;

            // initialize Model
            _projectModel = projectModel;

            // initialize available C++ standards
            AllStandards = new List<string> { "C++98", "C++03", "C++11", "C++14", "C++17", "C++20" };
        }

        #region Properties

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

        public string ProjectResultPath
        {
            get => _projectModel.ProjectResultPath; // Assuming ProjectModel holds the path
            set
            {
                if (_projectModel.ProjectResultPath != value)
                {
                    _projectModel.ProjectResultPath = value;
                    OnPropertyChanged(nameof(ProjectResultPath));
                }
            }
        }

        public List<string> AllStandards { get; }

        public bool CanCompile => _projectModel.IsComplete;

        private bool _isLoading;
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
        public bool IsProjectCompiled
        {
            get => _isProjectCompiled;
            set
            {
                if (!_isProjectCompiled) 
                {
                    _isProjectCompiled = value;
                    OnPropertyChanged(nameof(IsProjectCompiled));
                }
            }
        }

        #endregion

        #region Commands

        [RelayCommand]
        public void SelectCompilationDatabase()
        {
            string? filePath = _fileDialogService.OpenFile("Compilation Database File (*.json)|*.json");
            if (!string.IsNullOrEmpty(filePath))
            {
                CompilationDatabasePath = filePath;
            }
        }

        [RelayCommand]
        public void SelectMainFile()
        {
            string? filePath = _fileDialogService.OpenFile("C++ Source File (*.cpp)|*.cpp");
            if (!string.IsNullOrEmpty(filePath))
            {
                MainFilePath = filePath;
            }
        }

        [RelayCommand]
        public async Task CompileProject()
        {
            if (!CanCompile)
            {
                NotificationMessage = "Please ensure all inputs are provided before compiling.";
                IsNotificationVisible = true;
                await Task.Delay(3000);
                IsNotificationVisible = false;
                return;
            }

            IsLoading = true;

            try
            {
                bool isSuccessful = await Task.Run(() =>
                    _cPlusPlusService.RunASTDumpTool(
                        _projectModel.CompilationDatabasePath,
                        _projectModel.MainFilePath,
                        _projectModel.ProjectName,
                        _projectModel.FirstSelectedStandard));

                if (isSuccessful)
                {
                    ProjectResultPath = _cPlusPlusService.ProjectResultPath; // update project path
                    NotificationMessage = "Project compiled successfully!";
                    IsProjectCompiled = true;
                }
                else
                {
                    NotificationMessage = "Compilation failed!";
                    IsProjectCompiled = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during compilation: {ex.Message}");
                NotificationMessage = "An error occurred during compilation.";
                IsProjectCompiled = false;
            }
            finally
            {
                IsLoading = false;
            }

            IsNotificationVisible = true;
            await Task.Delay(3000);
            IsNotificationVisible = false;
        }

        #endregion
    }
}