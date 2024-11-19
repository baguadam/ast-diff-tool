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
                // read and parse the original compile_commands.json
                string originalJson = File.ReadAllText(compilationDatabasePath);
                var commands = JsonSerializer.Deserialize<List<JsonObject>>(originalJson);

                if (commands is null || commands.Count == 0)
                {
                    Debug.WriteLine("No commands found in compile_commands.json");
                    return false;
                }

                // create the two version for the standards
                var commandsForFirstStandard = ModifyCompileCommands(commands, firstStandard);
                var commandsForSecondStandard = ModifyCompileCommands(commands, secondStandard);

                // create temp files
                string tempFileStd1 = Path.GetTempFileName();
                string tempFileStd2 = Path.GetTempFileName();
                File.WriteAllText(tempFileStd1, JsonSerializer.Serialize(commandsForFirstStandard));
                File.WriteAllText(tempFileStd2, JsonSerializer.Serialize(commandsForSecondStandard));

                // run the tool twice
                ProjectResultPath = EnsureProjectDirectoryExists(projectName);
                string firstStandardOutput = Path.Combine(ProjectResultPath, firstStandard, ".txt");
                string secondStandardOutput = Path.Combine(ProjectResultPath, secondStandard, ".txt");

                bool resultFirst = ExecuteASTDumpTool(tempFileStd1, mainPath, firstStandardOutput);
                bool resultSecond = ExecuteASTDumpTool(tempFileStd2, mainPath, secondStandardOutput);

                // clean up
                File.Delete(tempFileStd1);
                File.Delete(tempFileStd2);

                return resultFirst && resultSecond;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Exception occurred while running AST Dump Tool: {ex.Message}");
                return false;
            }
        }

        private IList<JsonObject> ModifyCompileCommands(IList<JsonObject> commands, string standard)
        {
            var modifiedCommands = new List<JsonObject>();

            foreach (var commandsEntry in commands)
            {
                var modifiedEntry = new JsonObject(commandsEntry); // cloning the object
                if (modifiedEntry["command"] is not null)
                {
                    string command = modifiedEntry["command"].ToString();

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

                Debug.WriteLine($"Output: {output}");
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
