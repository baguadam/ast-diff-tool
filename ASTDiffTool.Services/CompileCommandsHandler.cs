using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

namespace ASTDiffTool.Services
{
    /// <summary>
    /// Class to modify compile commands and generate temporary files for executing the dump tool.
    /// </summary>
    public class CompileCommandsHandler
    {
        private readonly IFileService _fileService;
        private readonly CommandModifier _commandsModifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompileCommandsHandler"/> class.
        /// </summary>
        /// <param name="fileService">Service to handle file operations, such as reading and writing files.</param>
        public CompileCommandsHandler(IFileService fileService)
        {
            _fileService = fileService;
            _commandsModifier = new CommandModifier();
        }

        /// <summary>
        /// Creates a modified version of the original compile commands by changing the C++ standard.
        /// </summary>
        /// <param name="originalFilePath">Path to the original compile_commands.json file.</param>
        /// <param name="standard">The new C++ standard to be applied (e.g., "c++17").</param>
        /// <returns>The file path of the newly created temporary modified compile commands file.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the compile commands file is invalid or no commands are found.</exception>
        public string CreateModifiedCompileCommands(string originalFilePath, string standard)
        {
            try
            {
                string originalJson = _fileService.ReadFile(originalFilePath);
                var commands = JsonSerializer.Deserialize<List<JsonObject>>(originalJson);

                if (commands is null || commands.Count == 0)
                {
                    throw new InvalidOperationException("No commands found in compile_commands.json");
                }

                var modifiedCommands = _commandsModifier.ModifyCompileCommands(commands, standard);

                _fileService.EnsureDirectoryExists(CPlusPlusToolPaths.TEMP_AST_PATH);
                string tempFilePath = _fileService.CreateTemporaryFile(CPlusPlusToolPaths.TEMP_AST_PATH, "compile_commands.json");
                _fileService.WriteFile(tempFilePath, JsonSerializer.Serialize(modifiedCommands));

                return tempFilePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error parsing JSON: {ex.Message}");
                throw new InvalidOperationException("Failed to parse compile_commands.json", ex);
            }
        }
    }
}
