using ASTDiffTool.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services
{
    public class FileService : IFileService
    {
        public string ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            return File.ReadAllText(filePath);
        }

        public void WriteFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }

        public string CreateTemporaryFile(string directory, string fileName)
        {
            EnsureDirectoryExists(directory);
            string filePath = Path.Combine(directory, fileName);
            File.Create(filePath).Dispose();
            return filePath;
        }

        public void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public void WriteLog(string logFilePath, string output, string error)
        {
            try
            {
                using var logWriter = new StreamWriter(logFilePath, append: true);

                logWriter.WriteLine("========================================");
                logWriter.WriteLine($"Log Timestamp: {DateTime.Now}");
                logWriter.WriteLine("========================================");

                if (!string.IsNullOrEmpty(output))
                {
                    logWriter.WriteLine("Standard Output:");
                    logWriter.WriteLine(output);
                }

                if (!string.IsNullOrEmpty(error))
                {
                    logWriter.WriteLine("Standard Error:");
                    logWriter.WriteLine(error);
                }

                logWriter.WriteLine(); // blank line for readability
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to write log: {ex.Message}");
            }
        }
    }
}