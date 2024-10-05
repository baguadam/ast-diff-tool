using ASTDiffTool.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class ASTPageViewModel(Project project) : ViewModelBase
    {
        private readonly Project _project = project;

        public int NumberOfDifferences
        {
            get => _project.NumberOfDifferences;
            set
            {
                _project.NumberOfDifferences = value;
                OnPropertyChanged(nameof(NumberOfDifferences));
            }
        }
    }
}
