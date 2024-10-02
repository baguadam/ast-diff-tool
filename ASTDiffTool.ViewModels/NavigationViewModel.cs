﻿using ASTDiffTool.ViewModels.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public partial class NavigationViewModel : ObservableRecipient
    {
        private readonly IViewModelFactory _viewModelFactory;
        private ObservableObject _currentView;

        public ObservableObject CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public NavigationViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            NavigateASTPage();
        }

        [RelayCommand]
        public void NavigateASTPage()
        {
            CurrentView = _viewModelFactory.CreateASTPageViewModel();
        }

        [RelayCommand]
        public void NavigatePreprocessedCodePage()
        {
            CurrentView = _viewModelFactory.CreatePreprocessedCodePageViewModel();
        }

        [RelayCommand]
        public void NavigateProjectPage()
        {
            CurrentView = _viewModelFactory.CreateProjectPageViewModel();
        }
    }
}
