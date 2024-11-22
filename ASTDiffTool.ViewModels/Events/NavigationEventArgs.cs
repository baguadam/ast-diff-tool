using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Events
{
    /// <summary>
    /// Event arguments for navigation events that provide information about the target ViewModel being navigated to.
    /// </summary>
    public class NavigationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the ViewModel that is the target of the navigation event.
        /// </summary>
        public ViewModelBase ViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationEventArgs"/> class with the specified target ViewModel.
        /// </summary>
        /// <param name="viewModelBase">The target ViewModel for the navigation event.</param>
        public NavigationEventArgs(ViewModelBase viewModelBase)
        {
            ViewModel = viewModelBase;
        }
    }
}
