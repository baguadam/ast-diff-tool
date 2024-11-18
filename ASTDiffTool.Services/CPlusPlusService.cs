using ASTDiffTool.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services
{
    public class CPlusPlusService : ICPlusPlusService
    {
        private readonly string toolPath = "C:\\Users\\bagua\\OneDrive - Eotvos Lorand Tudomanyegyetem\\Adam\\Egyetem - 07\\SZAKDOLGOZAT\\ast-tree-comparer\\dump-tool\\build";
        public bool RunASTDumpTool(string compilationDatabasePath, string mainPath, string outputFile)
        {
            try
            {
                string arguments = $"-p \"{compilationDatabasePath}\" \"{mainPath}\" -o \"{outputFile}\"";

                // setup process info
                var processInfo = new ProcessStartInfo
                {
                    FileName = toolPath,
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
    }
}
