using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.ViewModels;
using ASTDiffTool.ViewModels.Interfaces;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ASTDiffTool.Tests
{
    public class ProjectPageViewModelTests
    {
        private readonly Mock<INeo4jService> _mockNeo4jService;
        private readonly Mock<INavigationService> _mockNavigationService;
        private readonly Mock<IEventAggregator> _mockEventAggregator;
        private readonly ProjectPageViewModel _viewModel;

        public ProjectPageViewModelTests()
        {
            _mockNeo4jService = new Mock<INeo4jService>();
            _mockNavigationService = new Mock<INavigationService>();
            _mockEventAggregator = new Mock<IEventAggregator>();

            _viewModel = new ProjectPageViewModel(_mockNeo4jService.Object, _mockNavigationService.Object, _mockEventAggregator.Object);
        }

        [Fact]
        public async Task LoadDatabaseInfoCommand_ShouldUpdateProperties_WhenExecutedSuccessfully()
        {
            _mockNeo4jService.Setup(s => s.GetNodeCountAsync()).ReturnsAsync(100);
            _mockNeo4jService.Setup(s => s.GetNodesByAstOriginAsync(Shared.ASTOrigins.FIRST_AST)).ReturnsAsync(40);
            _mockNeo4jService.Setup(s => s.GetNodesByAstOriginAsync(Shared.ASTOrigins.SECOND_AST)).ReturnsAsync(60);
            _mockNeo4jService.Setup(s => s.GetNodesByDifferenceTypeAsync(Shared.Differences.ONLY_IN_FIRST_AST)).ReturnsAsync(10);
            _mockNeo4jService.Setup(s => s.GetNodesByDifferenceTypeAsync(Shared.Differences.ONLY_IN_SECOND_AST)).ReturnsAsync(20);
            _mockNeo4jService.Setup(s => s.GetNodesByDifferenceTypeAsync(Shared.Differences.DIFFERENT_PARENTS)).ReturnsAsync(5);
            _mockNeo4jService.Setup(s => s.GetNodesByDifferenceTypeAsync(Shared.Differences.DIFFERENT_SOURCE_LOCATIONS)).ReturnsAsync(8);

            await _viewModel.LoadDatabaseInfoAsync();

            Assert.Equal(100, _viewModel.TotalNodeCount);
            Assert.Equal(40, _viewModel.NodesInFirstAST);
            Assert.Equal(60, _viewModel.NodesInSecondAST);
            Assert.Equal(10, _viewModel.OnlyInFirstAST);
            Assert.Equal(20, _viewModel.OnlyInSecondAST);
            Assert.Equal(5, _viewModel.DifferentParents);
            Assert.Equal(8, _viewModel.DifferentSourceLocations);

            _mockNeo4jService.Verify(s => s.GetNodeCountAsync(), Times.Exactly(2));
            _mockNeo4jService.Verify(s => s.GetNodesByAstOriginAsync(Shared.ASTOrigins.FIRST_AST), Times.Exactly(2));
            _mockNeo4jService.Verify(s => s.GetNodesByAstOriginAsync(Shared.ASTOrigins.SECOND_AST), Times.Exactly(2));
            _mockNeo4jService.Verify(s => s.GetNodesByDifferenceTypeAsync(Shared.Differences.ONLY_IN_FIRST_AST), Times.Exactly(2));
            _mockNeo4jService.Verify(s => s.GetNodesByDifferenceTypeAsync(Shared.Differences.ONLY_IN_SECOND_AST), Times.Exactly(2));
            _mockNeo4jService.Verify(s => s.GetNodesByDifferenceTypeAsync(Shared.Differences.DIFFERENT_PARENTS), Times.Exactly(2));
            _mockNeo4jService.Verify(s => s.GetNodesByDifferenceTypeAsync(Shared.Differences.DIFFERENT_SOURCE_LOCATIONS), Times.Exactly(2));
        }

        [Fact]
        public async Task LoadDatabaseInfoCommand_IsLoadingFalseAfterwards()
        {
            _mockNeo4jService.Setup(s => s.GetNodeCountAsync()).ReturnsAsync(100);
            Assert.False(_viewModel.IsLoading);

            await _viewModel.LoadDatabaseInfoAsync();

            Assert.False(_viewModel.IsLoading, "The loading indicator should be false after the loading ends.");
        }

        [Fact]
        public async Task LoadDatabaseInfoCommand_ShouldHandleException_AndSetLoadingToFalse()
        {
            _mockNeo4jService.Setup(s => s.GetNodeCountAsync()).ThrowsAsync(new Exception("Test Exception"));

            await _viewModel.LoadDatabaseInfoAsync();

            Assert.Equal(0, _viewModel.TotalNodeCount);
            Assert.False(_viewModel.IsLoading);
        }

        [Fact]
        public void PropertyChanged_ShouldTrigger_WhenTotalNodeCountChanges()
        {
            bool wasCalled = false;

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.TotalNodeCount))
                {
                    wasCalled = true;
                }
            };


            _viewModel.TotalNodeCount = 42;

            Assert.True(wasCalled);
        }

        [Fact]
        public void PropertyChanged_ShouldNotTrigger_WhenSettingSameValue()
        {
            _viewModel.TotalNodeCount = 100;
            bool wasCalled = false;

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.TotalNodeCount))
                {
                    wasCalled = true;
                }
            };

            _viewModel.TotalNodeCount = 100;

            Assert.False(wasCalled);
        }
    }
}