using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ASTDiffTool.Views.Utilities
{
    /// <summary>
    /// A value converter that checks if the type of a given value matches a specific type.
    /// Used in WPF data binding to conditionally display UI elements based on the type of data.
    /// </summary>
    public class IsTypeConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value to a boolean indicating whether the value's type matches the provided parameter type.
        /// </summary>
        /// <param name="value">The value whose type is to be checked.</param>
        /// <param name="targetType">The target type of the binding (not used).</param>
        /// <param name="parameter">The type to compare the value against.</param>
        /// <param name="culture">The culture information for the conversion.</param>
        /// <returns>
        /// True if the type of <paramref name="value"/> matches the type provided in <paramref name="parameter"/>, otherwise false.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.GetType() == parameter as Type;
        }

        /// <summary>
        /// The ConvertBack operation is not supported and will throw a <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="value">The value being passed back.</param>
        /// <param name="targetType">The target type of the conversion.</param>
        /// <param name="parameter">Optional parameter for the conversion.</param>
        /// <param name="culture">The culture information for the conversion.</param>
        /// <returns>None. This method always throws a <see cref="NotImplementedException"/>.</returns>
        /// <exception cref="NotImplementedException">Thrown when this method is called.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
