using System;

using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

using SeriesManager.Enum;

namespace SeriesManager.Converter
{
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || targetType != typeof(bool)) return null;

            if (value is WatchState)
            {
                return (WatchState)value != WatchState.Unwatched;
            }
            else if (value is ElementTheme)
            {
                return (ElementTheme)value == ElementTheme.Dark;
            }

            throw new Exception("Value Type not supported!");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is bool) || parameter == null) return null;

            if (parameter.ToString() == "WatchState")
            {
                return (bool)value ? WatchState.Watched : WatchState.Unwatched;
            }
            else if (parameter.ToString() == "ElementTheme")
            {
                return (bool)value ? ElementTheme.Dark : ElementTheme.Light;
            }

            throw new Exception("Parameter not supported!");
        }
    }
}
