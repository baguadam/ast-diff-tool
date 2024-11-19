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
        private readonly string _toolPath;
        private readonly string _baseASTDirectoryPath;

        public string ProjectResultPath { get; set; } = string.Empty;

        public CPlusPlusService()
        {
            _toolPath = CPlusPlusToolPaths.DUMP_TOOL_PATH;
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
                bool resultFirst = ExecuteASTDumpTool(tempFileStd1, mainPath, firstStandardOutput);
                File.Delete(tempFileStd1);

                // second run
                string tempFileStd2 = CreateModifiedCompileCommands(compilationDatabasePath, secondStandard);
                bool resultSecond = ExecuteASTDumpTool(tempFileStd2, mainPath, secondStandardOutput);
                File.Delete(tempFileStd2);
                
                return resultFirst && resultSecond;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Exception occurred while running AST Dump Tool: {ex.Message}");
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
                string originalJson = File.ReadAllText(originalFilePath);
                var commands = JsonSerializer.Deserialize<List<JsonObject>>(originalJson);

                if (commands is null || commands.Count == 0)
                {
                    throw new InvalidOperationException("No commands found in compile_commands.json");
                }

                var modifiedCommands = ModifyCompileCommands(commands, standard);

                string tempFilePath = Path.GetTempFileName();
                File.WriteAllText(tempFilePath, JsonSerializer.Serialize(modifiedCommands));
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

            if (!Directory.Exists(projectDirectory))
            {
                Directory.CreateDirectory(projectDirectory);
            }

            return projectDirectory;
        }

        private bool ExecuteASTDumpTool(string compilationDatabasePath, string mainPath, string outputFile)
        {
            try
            {
                string arguments = $"-p \"{compilationDatabasePath}\" \"{mainPath}\" -o \"{outputFile}\"";

                var processInfo = new ProcessStartInfo
                {
                    FileName = _toolPath,
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

                if (!File.Exists(outputFile))
                {
                    Debug.WriteLine($"Output file not created: {outputFile}");
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
