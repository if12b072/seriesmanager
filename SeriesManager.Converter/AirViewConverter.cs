using System;
using System.Globalization;

using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

using SeriesManager.Enum;

namespace SeriesManager.Converter
{
    public sealed class AirViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value is string) return value;

            if (value is DayOfWeek)
            {
                return DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)value);
            }
            else if (value is WeekOfMonth)
            {
                return new ResourceLoader().GetString("Week") + " " + ((int)value + 1).ToString();
            }
            else if (value is MonthOfYear)
            {
                return DateTimeFormatInfo.CurrentInfo.GetMonthName((int)value);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
