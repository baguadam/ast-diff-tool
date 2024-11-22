using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.ViewModels.Events;
using ASTDiffTool.ViewModels.Interfaces;
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
        private readonly IEventAggregator _eventAggregator;

        private ViewModelBase _currentViewModel;
        private bool _isCompilationCompleted = false;

        #region Properties
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public bool IsCompilationCompleted
        {
            get => _isCompilationCompleted;
            set
            {
                _isCompilationCompleted = value;
                OnPropertyChanged(nameof(IsCompilationCompleted));
            }
        }
        #endregion

        public MainViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            // subscribing 
            _navigationService.NavigationCompleted += OnNavigationService_NavigationCompleted;
            _eventAggregator.Subscribe<ProjectCompilationEvent>(HandleProjectCompiled);

            navigationService.NavigateTo<NewProjectPageViewModel>();
        }

        #region Commands
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

        [RelayCommand]
        public void NavigateTreeDisplayPage()
        {
            _navigationService.NavigateTo<TreeDisplayPageViewModel>();
        }
        #endregion

        #region Event handlers
        private void OnNavigationService_NavigationCompleted(object? sender, NavigationEventArgs args)
        {
            CurrentViewModel = args.ViewModel;
        }

        private void HandleProjectCompiled(ProjectCompilationEvent compilationEvent)
        {
            IsCompilationCompleted = compilationEvent.IsSuccessful;
        }
        #endregion
    }
}
