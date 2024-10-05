using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTDiffTool.ViewModels.Services;

namespace ASTDiffTool.ViewModels.Interfaces
{
    public interface INavigationService
    {
        event EventHandler<NavigationEventArgs> NavigationCompleted;
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
        void OnNavigationCompleted(NavigationEventArgs args);
    }
}
