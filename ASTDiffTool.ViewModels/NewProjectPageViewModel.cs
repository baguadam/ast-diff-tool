using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class NewProjectPageViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private string title = "New Project Pages";
    }
}
