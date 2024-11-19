using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services.Interfaces
{
    public interface ILoggerService
    {
        void Log(string filePath, string output, string error);
    }
}
