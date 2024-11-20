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
            // *********************************************
            // LOAD APPSETTINGS.JSON
            // *********************************************
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration); // registerint the configurations

            // Get configuration values
            string encryptionKey = configuration["Encryption:Key"]!;
            string encryptionIV = configuration["Encryption:IV"]!;
            string neo4jUri = configuration["Neo4j:Uri"]!;
            string neo4jUsername = configuration["Neo4j:Username"]!;
            string encryptedPassword = configuration["Neo4j:Password"]!;

            // Decrypt the Neo4j password
            string decryptedPassword = EncryptionHelper.Decrypt(encryptedPassword, encryptionKey, encryptionIV);

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
                new Neo4jService(neo4jUri, neo4jUsername, decryptedPassword));

            services.AddSingleton<NewProjectPageViewModel>();
            services.AddSingleton<ProjectPageViewModel>();
            services.AddSingleton<MainViewModel>();

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
