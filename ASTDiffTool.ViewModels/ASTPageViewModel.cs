using ASTDiffTool.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class ASTPageViewModel : ViewModelBase
    {
        private readonly Project _project;

        public ObservableCollection<LineViewModel> FirstStandardLines { get; set; } = new ObservableCollection<LineViewModel>();
        public ObservableCollection<LineViewModel> SecondStandardLines { get; set; } = new ObservableCollection<LineViewModel>();

        public ASTPageViewModel(Project project)
        {
            _project = project;

            FillLinesWithDummyData();
        }

        public int NumberOfDifferences
        {
            get => _project.NumberOfDifferences;
            set
            {
                _project.NumberOfDifferences = value;
                OnPropertyChanged(nameof(NumberOfDifferences));
            }
        }

        [RelayCommand]
        public void JumpToSourceCode()
        {
            Debug.WriteLine($"Line is clicked!");
        }

        private void FillLinesWithDummyData()
        {
            for (int i = 0; i < 100; i++)
            {
                var firstStandardLineViewModel = new LineViewModel { Line = $"Line {i + 1} in File 1" };
                var secondStandardLineViewModel = new LineViewModel { Line = $"Line {i + 1} in File 2" };

                // Highlight every 10th line
                if (i % 10 == 0)
                {
                    firstStandardLineViewModel.State = LineState.Highlighted;
                    secondStandardLineViewModel.State = LineState.Highlighted;
                }

                FirstStandardLines.Add(firstStandardLineViewModel);
                SecondStandardLines.Add(secondStandardLineViewModel);
            }
        }
    }
}
