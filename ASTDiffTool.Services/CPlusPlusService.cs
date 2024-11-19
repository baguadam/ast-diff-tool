using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.Shared;
using System.Diagnostics;
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
                // directory creation/path
                ProjectResultPath = EnsureProjectDirectoryExists(projectName);
                string outputFile = Path.Combine(ProjectResultPath, version);

                Debug.WriteLine($"=== {compilationDatabasePath}");
                Debug.WriteLine($"=== {mainPath}");
                Debug.WriteLine($"=== {ProjectResultPath}");
                Debug.WriteLine($"=== {outputFile}");

                string arguments = $"-p \"{compilationDatabasePath}\" \"{mainPath}\" -o \"{outputFile}\"";

                // setting up the process
                var processInfo = new ProcessStartInfo
                {
                    FileName = _toolPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                // starting the process
                using var process = new Process { StartInfo = processInfo };
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Debug.WriteLine($"Error: {error}");
                    ProjectResultPath = string.Empty; // in case of failure, clear the path
                    return false; // failed
                }

                Debug.WriteLine($"Output: {output}");
                return true; // succeeded
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while running AST Dump Tool: {ex.Message}");
                ProjectResultPath = string.Empty; // in case of exception, clear the result path
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
    }
}
