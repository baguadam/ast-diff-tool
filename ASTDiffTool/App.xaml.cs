using ASTDiffTool.Models;
using ASTDiffTool.ViewModels;
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
        private MainViewModel _mainViewModel = null!;
        private ProjectSettings _projectSettings = null!;
        private MainWindow _view = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _projectSettings = new ProjectSettings();
            _mainViewModel = new MainViewModel(_projectSettings);
            _view = new MainWindow();

            _view.DataContext = _mainViewModel;
            _view.Show();
        }
    }
}
