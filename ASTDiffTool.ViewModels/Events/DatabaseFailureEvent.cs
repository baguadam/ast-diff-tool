using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Events
{
    /// <summary>
    /// Represents an event that is triggered upon a database error
    /// </summary>
    public class DatabaseFailureEvent(string message)
    {
        /// <summary>
        /// The message to be displayed in case of database error.
        /// </summary>
        public string Message { get; set; } = message;
    }
}
