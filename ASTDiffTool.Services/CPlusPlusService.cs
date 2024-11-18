using ASTDiffTool.Services.Interfaces;
using System.Diagnostics;


namespace ASTDiffTool.Services
{
    public class CPlusPlusService : ICPlusPlusService
    {
        private readonly string _toolPath;
        private readonly string _baseASTDirectoryPath;

        public CPlusPlusService()
        {
            
        }

        public bool RunASTDumpTool(string compilationDatabasePath, string mainPath, string projectName, string version)
        {
            try
            {
                string projectDirectory = EnsureProjectDirectoryExists(projectName);
                string outputFile = Path.Combine(projectDirectory, version);

                Debug.WriteLine($"=== {compilationDatabasePath}");
                Debug.WriteLine($"=== {mainPath}");
                Debug.WriteLine($"=== {projectDirectory}");
                Debug.WriteLine($"=== {outputFile}");

                string arguments = $"-p \"{compilationDatabasePath}\" \"{mainPath}\" -o \"{outputFile}\"";

                // setup process info
                var processInfo = new ProcessStartInfo
                {
                    FileName = _toolPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                // start the process
                using var process = new Process { StartInfo = processInfo };
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Debug.WriteLine($"Error: {error}");
                    return false; // failed
                }

                Debug.WriteLine($"Output: {output}");
                return true; // succeeded
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Exception occurred while running AST Dump Tool: {ex.Message}");
                return false;
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
    }
}
