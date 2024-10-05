using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Utilities
{
    public class NavigationEventArgs : EventArgs
    {
        public ViewModelBase ViewModel { get; set; }

        public NavigationEventArgs(ViewModelBase viewModelBase)
        {
            ViewModel = viewModelBase;
        }
    }
}
