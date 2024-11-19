using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services.Interfaces
{
    public interface IFileService
    {
        string ReadFile(string filePath);
        void WriteFile(string filePath, string content);
        string CreateTemporaryFile(string directory, string fileName);
        void EnsureDirectoryExists(string directoryPath);
        void DeleteFile(string filePath);
    }
}
