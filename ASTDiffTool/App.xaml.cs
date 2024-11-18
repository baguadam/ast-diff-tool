using ASTDiffTool.Models;
using ASTDiffTool.Services;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.ViewModels;
using ASTDiffTool.ViewModels.Factories;
using ASTDiffTool.ViewModels.Interfaces;
using ASTDiffTool.ViewModels.Services;
using ASTDiffTool.Views;
using ASTDiffTool.Views.UserControls;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ASTDiffTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider = null!;

        private MainViewModel _mainViewModel = null!;
        private ProjectSettings _projectSettings = null!;
        private MainWindow _view = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        
            _view = _serviceProvider.GetRequiredService<MainWindow>();
            _view.Show();
        }

        private void ConfigureServices (IServiceCollection services)
        {
            // *********************************************
            // REGISTERING MODELS, SERVICES AND VIEW MODELS
            // *********************************************
            _ = services.AddSingleton<ProjectSettings>();
            _ = services.AddSingleton<Project>();

            _ = services.AddSingleton<IFileDialogService, FileDialogService>();
            _ = services.AddSingleton<IFileService, FileService>();
            _ = services.AddSingleton<INavigationService, NavigationService>();
            _ = services.AddSingleton<IEventAggregator, EventAggregator>();
            _ = services.AddSingleton<IDatabaseConnectionService, DatabaseConnectionService>();
            _ = services.AddSingleton<ICPlusPlusService, CPlusPlusService>();

            _ = services.AddSingleton<NewProjectPageViewModel>();
            _ = services.AddSingleton<ASTPageViewModel>();
            _ = services.AddSingleton<ProjectPageViewModel>();
            _ = services.AddSingleton<PreprocessedCodePageViewModel>();
            _ = services.AddSingleton<MainViewModel>();

            _ = services.AddSingleton<IViewModelFactory, ViewModelFactory>();

            // *********************************************
            // INJECTING THE VIEW MODELS INTO THE VIEWS
            // *********************************************
            _ = services.AddTransient<NewProjectPage>(provider =>
            {
                var newProjectPage = new NewProjectPage();
                var newProjectPageViewModel = provider.GetRequiredService<NewProjectPageViewModel>();
                newProjectPage.DataContext = newProjectPageViewModel;
                return newProjectPage;
            });

            _ = services.AddTransient<ASTPage>(provider =>
            {
                var ASTPage = new ASTPage();
                var ASTPageViewModel = provider.GetRequiredService<ASTPageViewModel>();
                ASTPage.DataContext = ASTPageViewModel;
                return ASTPage;
            });

            _ = services.AddTransient<MainWindow>(provider =>
            {
                var mainWindow = new MainWindow();
                var mainViewModel = provider.GetRequiredService<MainViewModel>();
                mainWindow.DataContext = mainViewModel;
                return mainWindow;
            });
        }
    }
}
