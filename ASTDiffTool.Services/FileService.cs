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
    }
}