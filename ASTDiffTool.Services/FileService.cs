using ASTDiffTool.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services
{
    /// <summary>
    /// Class responsible for file related operations
    /// </summary>
    public class FileService : IFileService
    {
        /// <summary>
        /// Reads everything from the given file.
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>Return the content of the file</returns>
        /// <exception cref="FileNotFoundException">Occurs if the file cannot be found, not exis</exception>
        public string ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Writes to a given file.
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="content">Content to be written in the file</param>
        public void WriteFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }

        /// <summary>
        /// Creates a temporary file.
        /// </summary>
        /// <param name="directory">The path to the directory that contains the temp file</param>
        /// <param name="fileName">The name of the temp file</param>
        /// <returns></returns>
        public string CreateTemporaryFile(string directory, string fileName)
        {
            EnsureDirectoryExists(directory);
            string filePath = Path.Combine(directory, fileName);
            File.Create(filePath).Dispose();
            return filePath;
        }

        /// <summary>
        /// Ensures whether a directory exists.
        /// </summary>
        /// <param name="directoryPath">Path to the directory</param>
        public void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// Deletes a given file.
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}