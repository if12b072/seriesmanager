using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SeriesManager.Converter
{
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is bool))
                return null;

            if (targetType == typeof(Visibility))
            {
                if (parameter != null && parameter is string && (string)parameter == "Negation")
                {
                    return (bool)value ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
