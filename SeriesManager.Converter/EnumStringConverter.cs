using System;
using System.Globalization;

using Windows.UI.Xaml.Data;
using Windows.ApplicationModel.Resources;

using SeriesManager.Enum;

namespace SeriesManager.Converter
{
    public class EnumStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;

            if (value is WatchState)
            {
                var watchState = (WatchState)value;

                if (parameter != null && parameter is string)
                {
                    if ((string)parameter == "TwoWay" && watchState == WatchState.UnAired)
                    {
                        watchState = WatchState.Watched;
                    }
                }
                return new ResourceLoader().GetString(watchState.ToString());
            }
            else if (value is System.DayOfWeek)
            {
                return CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((System.DayOfWeek)value);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
