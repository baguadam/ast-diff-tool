using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.ViewModels.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Services
{
    public class NavigationService(IServiceProvider serviceProvider) : INavigationService
    {
        public event EventHandler<NavigationEventArgs> NavigationCompleted = null!;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        private ViewModelBase _currentViewModel = null!;

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            _currentViewModel = _serviceProvider.GetRequiredService<TViewModel>();
            OnNavigationCompleted(new NavigationEventArgs(_currentViewModel));
        }

        public void OnNavigationCompleted(NavigationEventArgs args)
        {
            NavigationCompleted?.Invoke(this, args);
        }
    }
}
