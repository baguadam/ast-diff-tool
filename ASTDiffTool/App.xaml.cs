using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.ViewModels;
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
            services.AddSingleton<ProjectSettings>();
            services.AddSingleton<IFileDialogService, FileDialogService>();
            services.AddSingleton<MainViewModel>();

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
