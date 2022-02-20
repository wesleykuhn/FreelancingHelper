using System;
using System.Globalization;
using System.Windows.Data;

namespace FreelancingHelper.Converters
{
    public class ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            switch (value)
            {
                case TimeSpan ts:
                    return ts.ToString(@"h\:mm\:ss", CultureInfo.InvariantCulture);

                case DateTime dt:
                    return dt == DateTime.MinValue ? null : dt.ToString("yyyy:MM:dd hh:mm:ss tt", CultureInfo.InvariantCulture);

                default:
                    return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
