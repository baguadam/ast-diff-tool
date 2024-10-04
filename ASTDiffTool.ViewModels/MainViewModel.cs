using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.ViewModels.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.NavigationCompleted += OnNavigationService_NavigationCompleted;

            navigationService.NavigateTo<NewProjectPageViewModel>();
        }

        [RelayCommand]
        public void NavigateASTPage()
        {
            _navigationService.NavigateTo<ASTPageViewModel>();
        }

        [RelayCommand]
        public void NavigatePreprocessedCodePage()
        {
            _navigationService.NavigateTo<PreprocessedCodePageViewModel>();
        }

        [RelayCommand]
        public void NavigateProjectPage()
        {
            _navigationService.NavigateTo<ProjectPageViewModel>();
        }

        [RelayCommand]
        public void NavigateNewProjectPage()
        {
            _navigationService.NavigateTo<NewProjectPageViewModel>();
        }

        private void OnNavigationService_NavigationCompleted(object? sender, NavigationEventArgs args)
        {
            CurrentViewModel = args.ViewModel;
        }
    }
}
