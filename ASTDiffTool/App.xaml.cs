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
        private NewProjectModel _projectSettings = null!;
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
            services.AddSingleton<NewProjectModel>();
            services.AddSingleton<Project>();

            services.AddSingleton<IFileDialogService, FileDialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddSingleton<ICPlusPlusService, CPlusPlusService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<ILoggerService, LoggerService>();

            services.AddSingleton<NewProjectPageViewModel>();
            services.AddSingleton<ASTPageViewModel>();
            services.AddSingleton<ProjectPageViewModel>();
            services.AddSingleton<PreprocessedCodePageViewModel>();
            services.AddSingleton<MainViewModel>();

            services.AddSingleton<IViewModelFactory, ViewModelFactory>();

            // *********************************************
            // INJECTING THE VIEW MODELS INTO THE VIEWS
            // *********************************************
            services.AddTransient<NewProjectPage>(provider =>
            {
                var newProjectPage = new NewProjectPage();
                var newProjectPageViewModel = provider.GetRequiredService<NewProjectPageViewModel>();
                newProjectPage.DataContext = newProjectPageViewModel;
                return newProjectPage;
            });

            services.AddTransient<ASTPage>(provider =>
            {
                var ASTPage = new ASTPage();
                var ASTPageViewModel = provider.GetRequiredService<ASTPageViewModel>();
                ASTPage.DataContext = ASTPageViewModel;
                return ASTPage;
            });

            services.AddTransient<MainWindow>(provider =>
            {
                var mainWindow = new MainWindow();
                var mainViewModel = provider.GetRequiredService<MainViewModel>();
                mainWindow.DataContext = mainViewModel;
                return mainWindow;
            });
        }
    }
}
