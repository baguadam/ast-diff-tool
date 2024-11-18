using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    public class NewProjectModel
    {
        public string CompilationDatabasePath { get; set; }
        public string MainFilePath { get; set; }
        public string ProjectName { get; set; }
        public string FirstSelectedStandard { get; set; }
        public string SecondSelectedStandard { get; set; }

        public bool IsComplete =>
            !string.IsNullOrWhiteSpace(CompilationDatabasePath) &&
            !string.IsNullOrWhiteSpace(MainFilePath) &&
            !string.IsNullOrWhiteSpace(ProjectName) &&
            !string.IsNullOrWhiteSpace(FirstSelectedStandard) &&
            !string.IsNullOrWhiteSpace(SecondSelectedStandard);
    }
}
