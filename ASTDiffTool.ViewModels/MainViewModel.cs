using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
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
    public partial class MainViewModel : ObservableRecipient
    {
        public NavigationViewModel Navigation { get; }
        public NewProjectPageViewModel NewProjectPage { get; }

        public MainViewModel(NavigationViewModel navigationViewModel, NewProjectPageViewModel newProjectPageViewModel)
        {
            Navigation = navigationViewModel;
            NewProjectPage = newProjectPageViewModel;

            // Subscribing to events
            NewProjectPage.ProjectCompiled += OnProjectCompiled;
        }

        #region Event handlers
        private void OnProjectCompiled(object? sender, EventArgs e)
        {
            Navigation.IsProjectCompiled = true;
        }
        #endregion
    }
}
