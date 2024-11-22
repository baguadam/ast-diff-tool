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
    public class CompileCommandsHandler
    {
        private readonly IFileService _fileService;
        private readonly CommandModifier _commandsModifier;

        public CompileCommandsHandler(IFileService fileService)
        {
            _fileService = fileService;
            _commandsModifier = new CommandModifier();
        }

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
