using ASTDiffTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ASTDiffTool.Views
{
    /// <summary>
    /// Interaction logic for ASTPage.xaml
    /// </summary>
    public partial class ASTPage : UserControl
    {
        public ASTPage()
        {
            InitializeComponent();
            DataContext = new ASTPageViewModel();
        }
    }
}
