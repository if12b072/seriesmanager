using System;

using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

using SeriesManager.Enum;

namespace SeriesManager.Converter
{
    public class EnumVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;

            if (value is WatchState && targetType == typeof(Visibility))
            {
                var state = (WatchState)value;

                if (parameter != null && parameter is string && (string)parameter == "TwoWay" && state == WatchState.UnAired)
                {
                    state = WatchState.Watched;
                }

                return state == WatchState.Watched ? Visibility.Visible : Visibility.Collapsed;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
