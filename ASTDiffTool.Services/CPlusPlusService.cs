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

        private string DumpToolPath => CPlusPlusToolPaths.DumpToolPath;
        private string ComparerToolPath => CPlusPlusToolPaths.ComparerToolPath;
        private string BaseASTDirectoryPath => CPlusPlusToolPaths.BaseASTDirectoryPath;
        private string TempASTPath => CPlusPlusToolPaths.TempASTPath;

        /// <summary>
        /// Runs the AST Dump Tool twice, once for each specified C++ standard.
        /// </summary>
        /// <param name="compilationDatabasePath">Path to the compilation database JSON file.</param>
        /// <param name="mainPath">Main source file path to analyze.</param>
        /// <param name="projectName">Name of the project being analyzed.</param>
        /// <param name="firstStandard">The first C++ standard (e.g., "c++17") to use for AST generation.</param>
        /// <param name="secondStandard">The second C++ standard (e.g., "c++20") to use for AST generation.</param>
        /// <returns>True if both executions are successful; otherwise, false.</returns>
        public async Task<bool> RunASTDumpToolAsync(string compilationDatabasePath, string mainPath, string projectName, string firstStandard, string secondStandard)
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
                bool resultFirst = await ExecuteToolAsync(DumpToolPath, argumentsFirst, DUMP_LOG_FILE);
                _fileService.DeleteFile(tempFileStd1);

                // second run
                string tempFileStd2 = _commandsHandler.CreateModifiedCompileCommands(compilationDatabasePath, secondStandard);
                string argumentsSecond = $"-p \"{TempASTPath}\" \"{mainPath}\" -o \"{secondStandardOutput}\"";
                bool resultSecond = await ExecuteToolAsync(DumpToolPath, argumentsSecond, DUMP_LOG_FILE);
                _fileService.DeleteFile(tempFileStd2);

                return resultFirst && resultSecond;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Runs the AST Tree Comparer Tool to compare the ASTs generated for two different C++ standards.
        /// </summary>
        /// <param name="firstStandard">Path to the first C++ standard (e.g., "c++17").</param>
        /// <param name="secondStandard">Path to the second C++ standard (e.g., "c++20").</param>
        /// <returns>True if the tool execution is successful; otherwise, false.</returns>
        public async Task<bool> RunComparerToolAsync(string firstStandard, string secondStandard)
        {
            try
            {
                string firstStandardOutput = Path.Combine(ProjectResultPath, $"{firstStandard}.txt");
                string secondStandardOutput = Path.Combine(ProjectResultPath, $"{secondStandard}.txt");

                string arguments = $"\"{firstStandardOutput}\" \"{secondStandardOutput}\"";
                bool result = await ExecuteToolAsync(ComparerToolPath, arguments, COMPARER_LOG_FILE);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Executes a command-line tool and logs its output.
        /// </summary>
        /// <param name="toolPath">The path to the tool to execute.</param>
        /// <param name="arguments">Arguments to pass to the tool.</param>
        /// <param name="logFileName">The name of the log file to store the tool's output.</param>
        /// <returns>True if the tool execution is successful; otherwise, false.</returns>
        private async Task<bool> ExecuteToolAsync(string toolPath, string arguments, string logFileName)
        {
            string logFilePath = Path.Combine(ProjectResultPath, logFileName);

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
            
            try
            {
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                // timeout to avoid indefinite hanging
                bool exited = await Task.Run(() => process.WaitForExit(90000)); // waiting for 90 seconds to finish execution 
                if (!exited)
                {
                    process.Kill();
                    throw new TimeoutException($"The process did not complete within the allocated time and was terminated.");
                }

                // logging the output to the log file
                _loggerService.Log(logFilePath, output, error);

                if (process.ExitCode != 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                // in case of running process
                if (!process.HasExited)
                {
                    process.Kill();
                }

                _loggerService.Log(logFilePath, String.Empty, $"An exception occurred during execution: {ex.Message}");
                throw;
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
            try
            {
                string projectDirectory = Path.Combine(BaseASTDirectoryPath, projectName);
                _fileService.EnsureDirectoryExists(projectDirectory);
                return projectDirectory;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
