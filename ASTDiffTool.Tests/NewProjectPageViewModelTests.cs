using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.Models;
using ASTDiffTool.ViewModels;
using Moq;
using Xunit;
using System.Threading.Tasks;
using ASTDiffTool.ViewModels.Interfaces;

namespace ASTDiffTool.Tests
{
    public class NewProjectPageViewModelTests
    {
        private readonly Mock<IFileDialogService> _mockFileDialogService;
        private readonly Mock<ICPlusPlusService> _mockCPlusPlusService;
        private readonly Mock<IEventAggregator> _mockEventAggregator;
        private readonly NewProjectModel _projectModel;
        private readonly NewProjectPageViewModel _viewModel;

        public NewProjectPageViewModelTests()
        {
            _mockFileDialogService = new Mock<IFileDialogService>();
            _mockCPlusPlusService = new Mock<ICPlusPlusService>();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _projectModel = new NewProjectModel();

            _viewModel = new NewProjectPageViewModel(
                _mockFileDialogService.Object,
                _mockCPlusPlusService.Object,
                _mockEventAggregator.Object,
                _projectModel
            );
        }

        [Fact]
        public void SelectCompilationDatabase_ShouldUpdateCompilationDatabasePath_WhenValidFileSelected()
        {
            string expectedPath = "path/to/compile_commands.json";
            _mockFileDialogService.Setup(service => service.OpenFile(It.IsAny<string>())).Returns(expectedPath);

            _viewModel.SelectCompilationDatabase();

            Assert.Equal(expectedPath, _viewModel.CompilationDatabasePath);
        }

        [Fact]
        public void SelectMainFile_ShouldUpdateMainFilePath_WhenValidFileSelected()
        {
            string expectedPath = "path/to/main.cpp";
            _mockFileDialogService.Setup(service => service.OpenFile(It.IsAny<string>())).Returns(expectedPath);

            _viewModel.SelectMainFile();

            Assert.Equal(expectedPath, _viewModel.MainFilePath);
        }

        [Fact]
        public async Task CompileProject_ShouldSetIsLoadingTrueWhileExecuting_AndFalseAfterExecution()
        {
            _viewModel.ProjectName = "TestProject";
            _viewModel.CompilationDatabasePath = "path/to/compile_commands.json";
            _viewModel.MainFilePath = "path/to/main.cpp";
            _viewModel.FirstSelectedStandard = "c++17";
            _viewModel.SecondSelectedStandard = "c++20";

            _mockCPlusPlusService.Setup(s => s.RunASTDumpTool(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            _mockCPlusPlusService.Setup(s => s.RunComparerTool(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var compileTask = _viewModel.CompileProject();
            Assert.True(_viewModel.IsLoading);

            await compileTask;
            Assert.False(_viewModel.IsLoading);
        }

        [Fact]
        public async Task CompileProject_ShouldShowNotification_OnFailure()
        {
            _viewModel.ProjectName = "TestProject";
            _viewModel.CompilationDatabasePath = "path/to/compile_commands.json";
            _viewModel.MainFilePath = "path/to/main.cpp";
            _viewModel.FirstSelectedStandard = "c++17";
            _viewModel.SecondSelectedStandard = "c++20";

            _mockCPlusPlusService.Setup(s => s.RunASTDumpTool(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            await _viewModel.CompileProject();

            Assert.False(_viewModel.IsProjectCompiled);
            Assert.Contains("Dump Tool failed!", _viewModel.NotificationMessage);
        }

        [Fact]
        public async Task CompileProject_ShouldUpdateProperties_WhenSuccessful()
        {
            _viewModel.ProjectName = "TestProject";
            _viewModel.CompilationDatabasePath = "path/to/compile_commands.json";
            _viewModel.MainFilePath = "path/to/main.cpp";
            _viewModel.FirstSelectedStandard = "c++17";
            _viewModel.SecondSelectedStandard = "c++20";

            _mockCPlusPlusService.Setup(s => s.RunASTDumpTool(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            _mockCPlusPlusService.Setup(s => s.RunComparerTool(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            await _viewModel.CompileProject();

            Assert.True(_viewModel.IsProjectCompiled);
            Assert.Contains("Compilation completed successfully!", _viewModel.NotificationMessage);
        }

        [Fact]
        public void CanCompile_ShouldBeFalse_IfPropertiesNotSet()
        {
            _viewModel.ProjectName = "";
            _viewModel.CompilationDatabasePath = "";
            _viewModel.MainFilePath = "";

            Assert.False(_viewModel.CanCompile);
        }

        [Fact]
        public void CanCompile_ShouldBeTrue_IfAllRequiredPropertiesSet()
        {
            _viewModel.ProjectName = "TestProject";
            _viewModel.CompilationDatabasePath = "path/to/compile_commands.json";
            _viewModel.MainFilePath = "path/to/main.cpp";
            _viewModel.FirstSelectedStandard = "c++17";
            _viewModel.SecondSelectedStandard = "c++20";

            Assert.True(_viewModel.CanCompile);
        }
    }
}