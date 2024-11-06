using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Fittrack2._0.Converter
{
     public class EmptyStringToNullableIntConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? 0; // If null, show as "0" in TextBox
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
                return null; // Convert empty string to null

            if (int.TryParse(value.ToString(), out int result))
                return result;

            return DependencyProperty.UnsetValue; // Return an unset value if parsing fails
        }
    }
}
