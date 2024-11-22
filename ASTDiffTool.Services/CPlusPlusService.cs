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
        private readonly ILoggerService _loggerService;
        private readonly CompileCommandsHandler _commandsHandler;
        private readonly string _dumpToolPath;
        private readonly string _comparerToolPath;
        private readonly string _baseASTDirectoryPath;

        private readonly string DUMP_LOG_FILE = "dump_tool_log.txt";
        private readonly string COMPARER_LOG_FILE = "comparer_tool_log.txt";

        public string ProjectResultPath { get; set; } = string.Empty;

        public CPlusPlusService(IFileService fileService, ILoggerService loggerService)
        {
            _fileService = fileService;
            _loggerService = loggerService;
            _commandsHandler = new CompileCommandsHandler(fileService);

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
                string tempFileStd1 = _commandsHandler.CreateModifiedCompileCommands(compilationDatabasePath, firstStandard);
                string argumentsFirst = $"-p \"{CPlusPlusToolPaths.TEMP_AST_PATH}\" \"{mainPath}\" -o \"{firstStandardOutput}\"";
                bool resultFirst = ExecuteTool(CPlusPlusToolPaths.DUMP_TOOL_PATH, argumentsFirst, DUMP_LOG_FILE);
                _fileService.DeleteFile(tempFileStd1);

                // second run
                string tempFileStd2 = _commandsHandler.CreateModifiedCompileCommands(compilationDatabasePath, secondStandard);
                string argumentsSecond = $"-p \"{CPlusPlusToolPaths.TEMP_AST_PATH}\" \"{mainPath}\" -o \"{secondStandardOutput}\"";
                bool resultSecond = ExecuteTool(CPlusPlusToolPaths.DUMP_TOOL_PATH, argumentsSecond, DUMP_LOG_FILE);
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

                string arguments = $"\"{firstStandardOutput}\" \"{secondStandardOutput}\"";
                bool result = ExecuteTool(CPlusPlusToolPaths.COMPARER_TOOL_PATH, arguments, COMPARER_LOG_FILE);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while running AST Tree Comparer: {ex.Message}");
                return false;
            }
        }

        private bool ExecuteTool(string toolPath, string arguments, string logFileName)
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

                // logging the output to the log file
                string logFilePath = Path.Combine(ProjectResultPath, logFileName);
                _loggerService.Log(logFilePath, output, error);

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

        private string EnsureProjectDirectoryExists(string projectName)
        {
            string projectDirectory = Path.Combine(_baseASTDirectoryPath, projectName);
            _fileService.EnsureDirectoryExists(projectDirectory);
            return projectDirectory;
        }
    }
}
