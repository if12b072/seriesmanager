using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SeriesManager.Converter
{
    public sealed class VisibilityNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is Visibility)) return null;

            return (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
