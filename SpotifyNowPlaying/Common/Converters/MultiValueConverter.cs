using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SpotifyNowPlaying.Common.Converters
{
    public class MultiValueStringConverter : IMultiValueConverter
    {
        public string StringFormat { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || !values.Any()) return string.Empty;

            return string.Format(StringFormat, values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
