using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ASTDiffTool.Views.Utilities
{
    /// <summary>
    /// A custom navigation button that inherits from <see cref="RadioButton"/> to provide custom styling and behavior for navigation purposes.
    /// </summary>
    public class NavigationButton : RadioButton
    {
        /// <summary>
        /// Static constructor to override the default style key for the <see cref="NavigationButton"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor ensures that the button will use a custom style defined for the <see cref="NavigationButton"/> type.
        /// </remarks>
        static NavigationButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationButton), new FrameworkPropertyMetadata(typeof(NavigationButton)));
        }
    }
}
