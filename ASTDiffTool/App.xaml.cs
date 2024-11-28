using ASTDiffTool.Models;
using ASTDiffTool.Services;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.Shared;
using ASTDiffTool.ViewModels;
using ASTDiffTool.ViewModels.Interfaces;
using ASTDiffTool.ViewModels.Services;
using ASTDiffTool.Views;
using ASTDiffTool.Views.UserControls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using static System.Net.WebRequestMethods;

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
        private ProjectDatabaseInfoModel _projectDatabase = null!;
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
            // get neo4j password:
            string neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD") ?? "default_password";
            string neo4jUri = "bolt://localhost:7687";

            string neo4jUsername = "neo4j";

            // *********************************************
            // REGISTERING SERVICES
            // *********************************************
            services.AddSingleton<NewProjectModel>();

            services.AddSingleton<IFileDialogService, FileDialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddSingleton<ICPlusPlusService, CPlusPlusService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<ILoggerService, LoggerService>();

            // neo4j connection
            services.AddSingleton<INeo4jService>(provider =>
                new Neo4jService(neo4jUri, neo4jUsername, neo4jPassword));

            services.AddSingleton<NewProjectPageViewModel>();
            services.AddSingleton<ProjectPageViewModel>();
            services.AddSingleton<TreeDisplayPageViewModel>();
            services.AddSingleton<MainViewModel>();

            // *********************************************
            // INJECTING THE VIEW MODELS INTO THE VIEWS
            // *********************************************
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
