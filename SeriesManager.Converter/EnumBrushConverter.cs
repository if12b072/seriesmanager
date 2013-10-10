using System;

using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

using SeriesManager.Enum;

namespace SeriesManager.Converter
{
    public class EnumBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {            
            if (value is WatchState && targetType == typeof(Brush))
            {
                var watchState = (WatchState)value;

                if (parameter != null && parameter is string && (string)parameter == "TwoWay" && watchState == WatchState.UnAired)
                {
                    watchState = WatchState.Watched;
                }                

                switch (watchState)
                {
                    case WatchState.Watched:
                        return new SolidColorBrush(Color.FromArgb(255, 0, 100, 0)); // #006400
                    case WatchState.Unwatched:
                        return new SolidColorBrush(Color.FromArgb(255, 163, 7, 7)); // #A30707
                    case WatchState.UnAired:
                    default:
                        return new SolidColorBrush(Color.FromArgb(255, 67, 114, 170)); // #4372AA
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
