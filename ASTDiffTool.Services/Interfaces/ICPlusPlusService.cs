using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services.Interfaces
{
    public interface ICPlusPlusService
    {
        public string ProjectResultPath { get; set; }
        bool RunASTDumpTool(string compilationDatabasePath, string mainPath, string projectName, string version);
    }
}