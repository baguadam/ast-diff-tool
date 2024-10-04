using ASTDiffTool.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Services
{
    public class NavigationService : INavigationService
    {
        public event EventHandler<NavigationEventArgs> NavigationCompleted;
        private readonly IServiceProvider _serviceProvider;

        public ViewModelBase CurrentViewModel { get; private set; }

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<TViewModel>();
            OnNavigationCompleted(new NavigationEventArgs(CurrentViewModel));
        }

        public void OnNavigationCompleted(NavigationEventArgs args)
        {
            NavigationCompleted?.Invoke(this, args);
        }
    }
}
