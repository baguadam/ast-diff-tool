using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class NavigationViewModel : ObservableRecipient
    {
        private ObservableObject _currentView;

        public ObservableObject CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public NavigationViewModel()
        {
            CurrentView = new ASTPageViewModel();
        }

        [RelayCommand]
        public void NavigateASTPage()
        {
            CurrentView = new ASTPageViewModel();
        }

        [RelayCommand]
        public void NavigatePreprocessedCodePage()
        {
            CurrentView = new PreprocessedCodePageViewModel();
        }

        [RelayCommand]
        public void NavigateProjectPage()
        {
            CurrentView = new ProjectPageViewModel();
        }
    }
}
