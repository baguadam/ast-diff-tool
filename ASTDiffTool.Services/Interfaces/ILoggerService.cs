using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services.Interfaces
{
    /// <summary>
    /// Interface that defines logging capabilities for output and error messages.
    /// Provides methods for logging messages to a specific file.
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Logs output and error messages to a specified file.
        /// </summary>
        /// <param name="filePath">The path of the file where the log messages should be saved.</param>
        /// <param name="output">The output message to be logged.</param>
        /// <param name="error">The error message to be logged.</param>
        void Log(string filePath, string output, string error);
    }
}
