using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    /// <summary>
    /// Represents the model of a new project.
    /// </summary>
    public class NewProjectModel
    {
        /// <summary>
        /// The path to the compilation database.
        /// </summary>
        public string CompilationDatabasePath { get; set; }

        /// <summary>
        /// The path to the main file of the project.
        /// </summary>
        public string MainFilePath { get; set; }

        /// <summary>
        /// The name of the new project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// The first standard that is selected for the dump tool.
        /// </summary>
        public string FirstSelectedStandard { get; set; }

        /// <summary>
        /// The second standard that is selected for the dump tool.
        /// </summary>
        public string SecondSelectedStandard { get; set; }

        /// <summary>
        /// The path to the project folder that contains the dumped ASTs, logs.
        /// </summary>
        public string ProjectResultPath {  get; set; }

        /// <summary>
        /// Indicates whether everything is set.
        /// </summary>
        public bool IsComplete =>
            !string.IsNullOrWhiteSpace(CompilationDatabasePath) &&
            !string.IsNullOrWhiteSpace(MainFilePath) &&
            !string.IsNullOrWhiteSpace(ProjectName) &&
            !string.IsNullOrWhiteSpace(FirstSelectedStandard) &&
            !string.IsNullOrWhiteSpace(SecondSelectedStandard);
    }
}
