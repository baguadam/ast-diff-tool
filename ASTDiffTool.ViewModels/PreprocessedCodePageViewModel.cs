using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class PreprocessedCodePageViewModel : ViewModelBase
    {
        private string _title = "Preprocessed Page View";

        public string Title { get { return _title; } }
    }
}
