using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services.Interfaces
{
    /// <summary>
    /// Interface that defines file management operations.
    /// Provides methods for reading, writing, creating, and deleting files and directories.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Reads the content of a file from the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <returns>The content of the file as a string.</returns>
        string ReadFile(string filePath);

        /// <summary>
        /// Writes the provided content to the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the file where the content will be written.</param>
        /// <param name="content">The content to write to the file.</param>
        void WriteFile(string filePath, string content);

        /// <summary>
        /// Creates a temporary file in the specified directory with the given file name.
        /// Ensures that the directory exists before creating the file.
        /// </summary>
        /// <param name="directory">The directory where the temporary file will be created.</param>
        /// <param name="fileName">The name of the temporary file to create.</param>
        /// <returns>The full path of the created temporary file.</returns>
        string CreateTemporaryFile(string directory, string fileName);

        /// <summary>
        /// Ensures that a directory exists at the specified path. If the directory does not exist, it creates it.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to ensure exists.</param>
        void EnsureDirectoryExists(string directoryPath);

        /// <summary>
        /// Deletes the file at the specified file path if it exists.
        /// </summary>
        /// <param name="filePath">The path of the file to delete.</param>
        void DeleteFile(string filePath);
    }
}
