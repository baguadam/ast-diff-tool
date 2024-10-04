using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Services
{
    public interface INavigationService
    {
        event EventHandler<NavigationEventArgs> NavigationCompleted;
        ViewModelBase CurrentViewModel { get; }
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
        void OnNavigationCompleted(NavigationEventArgs args);
    }
}
