using ASTDiffTool.ViewModels.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class NavigationViewModel : ViewModelBase
    {
        private readonly IViewModelFactory _viewModelFactory;
        private object _currentView;
        private bool _isProjectCompiled = false;

        #region Properties
        public bool IsProjectCompiled
        {
            get => _isProjectCompiled;
            set
            {
                _isProjectCompiled = value;
                OnPropertyChanged(nameof(IsProjectCompiled));
            }
        }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }
        #endregion

        public NavigationViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            NavigateNewProjectPage();
        }

        [RelayCommand]
        public void NavigateASTPage()
        {
            CurrentView = _viewModelFactory.CreateASTPageViewModel();
        }

        [RelayCommand]
        public void NavigatePreprocessedCodePage()
        {
            CurrentView = _viewModelFactory.CreatePreprocessedCodePageViewModel();
        }

        [RelayCommand]
        public void NavigateProjectPage()
        {
            CurrentView = _viewModelFactory.CreateProjectPageViewModel();
        }

        [RelayCommand]
        public void NavigateNewProjectPage()
        {
            CurrentView = _viewModelFactory.CreateNewProjectPageViewModel();
        }
    }
}
