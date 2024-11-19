using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.Shared;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ASTDiffTool.Services
{
    public class CPlusPlusService : ICPlusPlusService
    {
        private readonly IFileService _fileService;
        private readonly string _dumpToolPath;
        private readonly string _comparerToolPath;
        private readonly string _baseASTDirectoryPath;

        public string ProjectResultPath { get; set; } = string.Empty;

        public CPlusPlusService(IFileService fileService)
        {
            _fileService = fileService;

            _dumpToolPath = CPlusPlusToolPaths.DUMP_TOOL_PATH;
            _comparerToolPath = CPlusPlusToolPaths.COMPARER_TOOL_PATH;
            _baseASTDirectoryPath = CPlusPlusToolPaths.BASE_AST_DIRECTORY_PATH;
        }

        public bool RunASTDumpTool(string compilationDatabasePath, string mainPath, string projectName, string firstStandard, string secondStandard)
        {
            try
            {
                // arguments
                ProjectResultPath = EnsureProjectDirectoryExists(projectName);
                string firstStandardOutput = Path.Combine(ProjectResultPath, $"{firstStandard}.txt");
                string secondStandardOutput = Path.Combine(ProjectResultPath, $"{secondStandard}.txt");

                // first run
                string tempFileStd1 = CreateModifiedCompileCommands(compilationDatabasePath, firstStandard);
                string argumentsFirst = $"-p \"{CPlusPlusToolPaths.TEMP_AST_PATH}\" \"{mainPath}\" -o \"{firstStandardOutput}\"";
                bool resultFirst = ExecuteTool(CPlusPlusToolPaths.DUMP_TOOL_PATH, argumentsFirst);
                _fileService.DeleteFile(tempFileStd1);

                // second run
                string tempFileStd2 = CreateModifiedCompileCommands(compilationDatabasePath, secondStandard);
                string argumentsSecond = $"-p \"{CPlusPlusToolPaths.TEMP_AST_PATH}\" \"{mainPath}\" -o \"{secondStandardOutput}\"";
                bool resultSecond = ExecuteTool(CPlusPlusToolPaths.DUMP_TOOL_PATH, argumentsSecond);
                _fileService.DeleteFile(tempFileStd2);

                return resultFirst && resultSecond;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Exception occurred while running AST Dump Tool: {ex.Message}");
                return false;
            }
        }

        public bool RunComparerTool(string firstStandard, string secondStandard)
        {
            try
            {
                string firstStandardOutput = Path.Combine(ProjectResultPath, $"{firstStandard}.txt");
                string secondStandardOutput = Path.Combine(ProjectResultPath, $"{secondStandard}.txt");
                string logsOutput = Path.Combine(ProjectResultPath, "comparer_logs.txt");

                string arguments = $"\"{firstStandardOutput}\" \"{secondStandardOutput}\" \"{logsOutput}\"";
                bool result = ExecuteTool(CPlusPlusToolPaths.COMPARER_TOOL_PATH, arguments);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while running AST Tree Comparer: {ex.Message}");
                return false;
            }
        }

        private List<JsonObject> ModifyCompileCommands(List<JsonObject> commands, string standard)
        {
            var modifiedCommands = new List<JsonObject>();

            foreach (var commandEntry in commands)
            {
                // seep copy of the current command entry
                var modifiedEntry = JsonNode.Parse(commandEntry.ToJsonString()).AsObject();

                if (modifiedEntry["command"] != null)
                {
                    string command = modifiedEntry["command"].ToString();

                    // replace or add the -std flag
                    if (command.Contains("-std=c++"))
                    {
                        command = Regex.Replace(command, @"-std=c\+\+\d{2}", $"-std={standard}");
                    }
                    else
                    {
                        command += $" -std={standard}";
                    }

                    modifiedEntry["command"] = command;
                }

                modifiedCommands.Add(modifiedEntry);
            }

            return modifiedCommands;
        }

        private string CreateModifiedCompileCommands(string originalFilePath, string standard)
        {
            try
            {
                string originalJson = _fileService.ReadFile(originalFilePath);
                var commands = JsonSerializer.Deserialize<List<JsonObject>>(originalJson);

                if (commands is null || commands.Count == 0)
                {
                    throw new InvalidOperationException("No commands found in compile_commands.json");
                }

                var modifiedCommands = ModifyCompileCommands(commands, standard);

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

        private string EnsureProjectDirectoryExists(string projectName)
        {
            string projectDirectory = Path.Combine(_baseASTDirectoryPath, projectName);
            _fileService.EnsureDirectoryExists(projectDirectory);
            return projectDirectory;
        }

        private bool ExecuteTool(string toolPath, string arguments)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = toolPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                using var process = new Process { StartInfo = processInfo };
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Debug.WriteLine($"Error: {error}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred during AST Dump Tool execution: {ex.Message}");
                return false;
            }
        }
    }
}
