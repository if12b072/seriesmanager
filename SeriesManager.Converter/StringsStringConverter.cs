using System;

using Windows.UI.Xaml.Data;

namespace SeriesManager.Converter
{
    public class StringsStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is string[]) || targetType != typeof(string))
                return null;

            string[] parts = (string[])value;
            return string.Join(", ", parts);            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
