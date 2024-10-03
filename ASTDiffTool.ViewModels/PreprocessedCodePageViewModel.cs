using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class PreprocessedCodePageViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private string title = "Preprocessed Code Page";
    }
}
