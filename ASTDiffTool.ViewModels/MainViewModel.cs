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
    /// <summary>
    /// ViewModel representing the main interface of the application, managing navigation.
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;

        private ViewModelBase _currentViewModel;
        private bool _isCompilationCompleted = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">Service to handle navigation between different pages in the application.</param>
        /// <param name="eventAggregator">Service to handle event aggregation and message passing.</param>
        public MainViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            // subscribing 
            _navigationService.NavigationCompleted += OnNavigationService_NavigationCompleted;
            _eventAggregator.Subscribe<ProjectCompilationEvent>(HandleProjectCompiled);

            navigationService.NavigateTo<NewProjectPageViewModel>();
        }

        #region Properties
        /// <summary>
        /// Gets or sets the currently displayed view model apart from the navigation.
        /// </summary>
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        /// <summary>
        /// Indicates whether compilation has completed.
        /// </summary>
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

        #region Commands
        /// <summary>
        /// Command to navigate to the Project Page.
        /// </summary>
        [RelayCommand]
        public void NavigateProjectPage()
        {
            _navigationService.NavigateTo<ProjectPageViewModel>();
        }

        /// <summary>
        /// Command to navigate to the New Project Page.
        /// </summary>
        [RelayCommand]
        public void NavigateNewProjectPage()
        {
            _navigationService.NavigateTo<NewProjectPageViewModel>();
        }

        /// <summary>
        /// Command to navigate to the Tree View Page.
        /// </summary>
        [RelayCommand]
        public void NavigateTreeDisplayPage()
        {
            _navigationService.NavigateTo<TreeDisplayPageViewModel>();
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Handles the navigation completion event, updating the current view model.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">Navigation event arguments containing the new view model.</param>
        private void OnNavigationService_NavigationCompleted(object? sender, NavigationEventArgs args)
        {
            CurrentViewModel = args.ViewModel;
        }

        /// <summary>
        /// Handles the project compilation event, setting whether the compilation has been completed successfully.
        /// </summary>
        /// <param name="compilationEvent">Event containing the result of the project compilation.</param>
        private void HandleProjectCompiled(ProjectCompilationEvent compilationEvent)
        {
            IsCompilationCompleted = compilationEvent.IsSuccessful;
        }
        #endregion
    }
}
