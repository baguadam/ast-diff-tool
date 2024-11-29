using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Events
{
    /// <summary>
    /// Represents an event that is triggered upon the loading state of the project.
    /// </summary>
    /// <param name="isLoading">State of loading</param>
    public class LoadingEvent(bool isLoading, string toolState)
    {
        /// <summary>
        /// Indicates whether the project is loading at the moment.
        /// </summary>
        public bool IsLoading { get; set; } = isLoading;

        /// <summary>
        /// The state of the tools to be displayed above the progress bar.
        /// </summary>
        public string ToolState { get; set; } = toolState;
    }
}
