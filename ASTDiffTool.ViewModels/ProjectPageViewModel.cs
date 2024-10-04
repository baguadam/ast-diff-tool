using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class ProjectPageViewModel : ViewModelBase
    {
        private string _title = "Project Page View";

        public string Title { get { return _title; } }
    }
}
