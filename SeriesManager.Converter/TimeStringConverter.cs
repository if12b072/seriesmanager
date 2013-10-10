using System;
using System.Globalization;

using Windows.UI.Xaml.Data;

namespace SeriesManager.Converter
{
    public class TimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is TimeSpan) || targetType != typeof(string))
                return null;

            TimeSpan airTime = (TimeSpan)value;
            return new DateTime(airTime.Ticks).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);          
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
