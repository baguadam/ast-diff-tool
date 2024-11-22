using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.Services;
using ASTDiffTool.Shared;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Text.Json;

namespace ASTDiffTool.Tests
{
    public class CommandModificationTests
    {
        private readonly CommandModifier _commandModifier;
        private readonly Mock<IFileService> _mockFileService;
        private readonly CompileCommandsHandler _compileCommandsHandler;

        public CommandModificationTests()
        {
            _commandModifier = new CommandModifier();
            _mockFileService = new Mock<IFileService>();
            _compileCommandsHandler = new CompileCommandsHandler(_mockFileService.Object);
        }

        /*****************************************
         * CommandModifier
        *****************************************/
        [Fact]
        public void ModifyCompileCommands_ShouldModifyStdFlag_WhenStdFlagExists()
        {
            string standard = "c++17";

            var originalCommands = new List<JsonObject>
            {
                new JsonObject
                {
                    ["directory"] = "/project",
                    ["command"] = "clang++ -std=c++14 -o file.o file.cpp",
                    ["file"] = "file.cpp"
                }
            };

            var modifiedCommands = _commandModifier.ModifyCompileCommands(originalCommands, standard);

            Assert.Single(modifiedCommands);
            Assert.Contains("-std=c++17", modifiedCommands[0]["command"].ToString());
        }

        [Fact]
        public void ModifyCompileCommands_ShouldAddStdFlag_WhenNoStdFlagExists()
        {
            string standard = "c++20";

            var originalCommands = new List<JsonObject>
            {
                new JsonObject
                {
                    ["directory"] = "/project",
                    ["command"] = "clang++ -o file.o file.cpp",
                    ["file"] = "file.cpp"
                }
            };

            var modifiedCommands = _commandModifier.ModifyCompileCommands(originalCommands, standard);

            Assert.Single(modifiedCommands);
            Assert.Contains("-std=c++20", modifiedCommands[0]["command"].ToString());
        }

        [Fact]
        public void ModifyCompileCommands_ShouldNotModify_WhenCommandIsNull()
        {
            string standard = "c++20";

            var originalCommands = new List<JsonObject>
            {
                new JsonObject
                {
                    ["directory"] = "/project",
                    ["command"] = null,
                    ["file"] = "file.cpp"
                }
            };

            var modifiedCommands = _commandModifier.ModifyCompileCommands(originalCommands, standard);

            Assert.Single(modifiedCommands);
            Assert.Null(modifiedCommands[0]["command"]);
        }

        /*****************************************
         * CompileCommandHandler
        *****************************************/
        [Fact]
        public void CreateModifiedCompileCommands_ShouldThrowException_WhenCommandsListIsEmpty()
        {
            string originalFilePath = "path/to/compile_commands.json";
            string emptyJson = "[]";

            _mockFileService.Setup(fs => fs.ReadFile(originalFilePath)).Returns(emptyJson);

            Assert.Throws<InvalidOperationException>(() => _compileCommandsHandler.CreateModifiedCompileCommands(originalFilePath, "c++17"));
        }

        [Fact]
        public void CreateModifiedCompileCommands_ShouldCreateModifiedFile_WhenValidCommandsProvided()
        {
            string originalFilePath = "path/to/compile_commands.json";
            string tempFilePath = "temp/compile_commands.json";
            string standard = "c++17";

            var originalCommands = new List<JsonObject>
            {
                new JsonObject
                {
                    ["directory"] = "/project",
                    ["command"] = "clang++ -std=c++14 -o file.o file.cpp",
                    ["file"] = "file.cpp"
                }
            };

            string originalJson = JsonSerializer.Serialize(originalCommands);

            _mockFileService.Setup(fs => fs.ReadFile(originalFilePath)).Returns(originalJson);
            _mockFileService.Setup(fs => fs.CreateTemporaryFile(It.IsAny<string>(), It.IsAny<string>())).Returns(tempFilePath);

            string writtenContent = null;

            _mockFileService.Setup(fs => fs.WriteFile(tempFilePath, It.IsAny<string>()))
                .Callback<string, string>((path, content) => writtenContent = content);

            string resultFilePath = _compileCommandsHandler.CreateModifiedCompileCommands(originalFilePath, standard);

            Assert.Equal(tempFilePath, resultFilePath);
            _mockFileService.Verify(fs => fs.EnsureDirectoryExists(It.IsAny<string>()), Times.Once);
            _mockFileService.Verify(fs => fs.WriteFile(tempFilePath, It.IsAny<string>()), Times.Once);

            // deserialize
            var modifiedCommands = JsonSerializer.Deserialize<List<JsonObject>>(writtenContent);
            Assert.NotNull(modifiedCommands);
            Assert.Single(modifiedCommands);
            Assert.Contains("-std=c++17", modifiedCommands[0]["command"].ToString());
        }

        [Fact]
        public void CreateModifiedCompileCommands_ShouldThrowException_WhenFileNotFound()
        {
            string originalFilePath = "invalid/path";
            _mockFileService.Setup(fs => fs.ReadFile(originalFilePath)).Throws<FileNotFoundException>();

            Assert.Throws<InvalidOperationException>(() => _compileCommandsHandler.CreateModifiedCompileCommands(originalFilePath, "c++17"));
        }
    }
}
