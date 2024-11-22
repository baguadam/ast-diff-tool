using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.ViewModels.Events;
using ASTDiffTool.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Services
{
    /// <summary>
    /// Service that manages navigation between different view models in the application.
    /// </summary>
    public class NavigationService(IServiceProvider serviceProvider) : INavigationService
    {
        /// <summary>
        /// Event triggered when the navigation is completed, providing the newly navigated view model.
        /// </summary>
        public event EventHandler<NavigationEventArgs> NavigationCompleted = null!;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        private ViewModelBase _currentViewModel = null!;

        /// <summary>
        /// Navigates to the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model to navigate to.</typeparam>
        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            _currentViewModel = _serviceProvider.GetRequiredService<TViewModel>();
            OnNavigationCompleted(new NavigationEventArgs(_currentViewModel));
        }

        /// <summary>
        /// Invokes the NavigationCompleted event with the specified event arguments.
        /// </summary>
        /// <param name="args">Arguments containing the new view model to which navigation has occurred.</param>
        public void OnNavigationCompleted(NavigationEventArgs args)
        {
            NavigationCompleted?.Invoke(this, args);
        }
    }
}
