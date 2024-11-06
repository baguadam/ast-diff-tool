using ASTDiffTool.Models;
using ASTDiffTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ASTDiffTool.Views.UserControls
{
    /// <summary>
    /// Interaction logic for TextDisplayerControl.xaml
    /// </summary>
    public partial class TextDisplayerControl : UserControl
    {
        public static readonly DependencyProperty LinesProperty =
            DependencyProperty.Register("Lines", typeof(ObservableCollection<LineModel>), typeof(TextDisplayerControl));

        public ObservableCollection<LineModel> Lines
        {
            get { return (ObservableCollection<LineModel>)GetValue(LinesProperty); }
            set { SetValue(LinesProperty, value); }
        }

        public TextDisplayerControl()
        {
            InitializeComponent();
        }
    }
}
