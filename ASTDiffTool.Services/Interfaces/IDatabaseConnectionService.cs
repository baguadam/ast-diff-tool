using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services.Interfaces
{
    public interface IDatabaseConnectionService
    {
        string GetConnectionString();
        void UpdateConnectionString(string filePath);
    }
}
