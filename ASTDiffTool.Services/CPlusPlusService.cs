using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.Shared;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ASTDiffTool.Services
{
    /// <summary>
    /// Provides functionality to interact with the C++ analysis tools, runs them as processes, provides information about 
    /// the output of the runs.
    /// </summary>
    public class CPlusPlusService : ICPlusPlusService
    {
        private readonly string DUMP_LOG_FILE = "dump_tool_log.txt";
        private readonly string COMPARER_LOG_FILE = "comparer_tool_log.txt";
        private readonly IFileService _fileService;
        private readonly ILoggerService _loggerService;
        private readonly CompileCommandsHandler _commandsHandler;

        private string? _dumpToolPath;
        private string? _comparerToolPath;
        private string? _baseASTDirectoryPath;
        private string? _tempASTPath;

        /// <summary>
        /// Gets or sets the path where the project results are stored.
        /// </summary>
        public string ProjectResultPath { get; set; } = string.Empty;

        public CPlusPlusService(IFileService fileService, ILoggerService loggerService)
        {
            _fileService = fileService;
            _loggerService = loggerService;
            _commandsHandler = new CompileCommandsHandler(fileService);
        }

        private string DumpToolPath => _dumpToolPath ??= GetPath(CPlusPlusToolPaths.DUMP_TOOL_PATH);
        private string ComparerToolPath => _comparerToolPath ??= GetPath(CPlusPlusToolPaths.COMPARER_TOOL_PATH);
        private string BaseASTDirectoryPath => _baseASTDirectoryPath ??= GetPath(CPlusPlusToolPaths.BASE_AST_DIRECTORY_PATH);
        private string TempASTPath => _tempASTPath ??= GetPath(CPlusPlusToolPaths.TEMP_AST_PATH);

        private string GetPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new InvalidOperationException("The TOOL_PATH environment variable must be set to initialize tool paths.");
            }
            return path;
        }

        /// <summary>
        /// Runs the AST Dump Tool twice, once for each specified C++ standard.
        /// </summary>
        /// <param name="compilationDatabasePath">Path to the compilation database JSON file.</param>
        /// <param name="mainPath">Main source file path to analyze.</param>
        /// <param name="projectName">Name of the project being analyzed.</param>
        /// <param name="firstStandard">The first C++ standard (e.g., "c++17") to use for AST generation.</param>
        /// <param name="secondStandard">The second C++ standard (e.g., "c++20") to use for AST generation.</param>
        /// <returns>True if both executions are successful; otherwise, false.</returns>
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
                string argumentsFirst = $"-p \"{TempASTPath}\" \"{mainPath}\" -o \"{firstStandardOutput}\"";
                bool resultFirst = ExecuteTool(DumpToolPath, argumentsFirst, DUMP_LOG_FILE);
                _fileService.DeleteFile(tempFileStd1);

                // second run
                string tempFileStd2 = _commandsHandler.CreateModifiedCompileCommands(compilationDatabasePath, secondStandard);
                string argumentsSecond = $"-p \"{TempASTPath}\" \"{mainPath}\" -o \"{secondStandardOutput}\"";
                bool resultSecond = ExecuteTool(DumpToolPath, argumentsSecond, DUMP_LOG_FILE);
                _fileService.DeleteFile(tempFileStd2);

                return resultFirst && resultSecond;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Exception occurred while running AST Dump Tool: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Runs the AST Tree Comparer Tool to compare the ASTs generated for two different C++ standards.
        /// </summary>
        /// <param name="firstStandard">Path to the first C++ standard (e.g., "c++17").</param>
        /// <param name="secondStandard">Path to the second C++ standard (e.g., "c++20").</param>
        /// <returns>True if the tool execution is successful; otherwise, false.</returns>
        public bool RunComparerTool(string firstStandard, string secondStandard)
        {
            try
            {
                string firstStandardOutput = Path.Combine(ProjectResultPath, $"{firstStandard}.txt");
                string secondStandardOutput = Path.Combine(ProjectResultPath, $"{secondStandard}.txt");

                string arguments = $"\"{firstStandardOutput}\" \"{secondStandardOutput}\"";
                bool result = ExecuteTool(ComparerToolPath, arguments, COMPARER_LOG_FILE);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while running AST Tree Comparer: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Executes a command-line tool and logs its output.
        /// </summary>
        /// <param name="toolPath">The path to the tool to execute.</param>
        /// <param name="arguments">Arguments to pass to the tool.</param>
        /// <param name="logFileName">The name of the log file to store the tool's output.</param>
        /// <returns>True if the tool execution is successful; otherwise, false.</returns>
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

        /// <summary>
        /// Ensures the project directory exists for storing output files.
        /// If it doesn't exist, it will be created.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The path to the project directory.</returns>
        private string EnsureProjectDirectoryExists(string projectName)
        {
            string projectDirectory = Path.Combine(BaseASTDirectoryPath, projectName);
            _fileService.EnsureDirectoryExists(projectDirectory);
            return projectDirectory;
        }
    }
}
