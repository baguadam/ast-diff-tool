using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Events
{
    /// <summary>
    /// Represents an event that is triggered upon the completion of a project compilation.
    /// </summary>
    public class ProjectCompilationEvent(bool isSuccessful)
    {
        /// <summary>
        /// Gets or sets a value indicating whether the compilation was successful.
        /// </summary>
        public bool IsSuccessful { get; set; } = isSuccessful;
    }
}
