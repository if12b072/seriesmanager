using System;
using System.Globalization;

using Windows.UI.Xaml.Data;

namespace SeriesManager.Converter
{
    public class DateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is DateTime))
                return null;

            DateTime airDate = (DateTime)value;

            if (airDate.Date.CompareTo(new DateTime(1, 1, 1)) == 0)
            {
                var res = new Windows.ApplicationModel.Resources.ResourceLoader();
                return res.GetString("UnknownDate");
            }

            if (parameter != null && parameter is string)
            {
                var arg = parameter.ToString();

                if (arg == "Year")
                    return airDate.ToString("yyyy");
                else if (arg == "Month")
                    return airDate.ToString("MM");
                else if (arg == "Day")
                    return airDate.ToString("dddd");
            }

            return airDate.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
