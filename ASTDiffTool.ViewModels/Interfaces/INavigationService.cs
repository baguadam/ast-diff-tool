using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTDiffTool.ViewModels.Events;

namespace ASTDiffTool.ViewModels.Interfaces
{
    /// <summary>
    /// Navigation Service interface for navigating between different view models. 
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Event triggered when the navigation to a new view model is completed.
        /// </summary>
        event EventHandler<NavigationEventArgs> NavigationCompleted;

        /// <summary>
        /// Navigates to a specified view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model to navigate to.</typeparam>
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;

        /// <summary>
        /// Invokes the <see cref="NavigationCompleted"/> event when the navigation process is completed.
        /// </summary>
        /// <param name="args">The event arguments containing information about the navigation, such as the target view model.</param>
        void OnNavigationCompleted(NavigationEventArgs args);
    }
}
