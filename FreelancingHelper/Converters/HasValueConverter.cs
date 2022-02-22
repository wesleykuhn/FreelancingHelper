using System;
using System.Globalization;
using System.Windows.Data;

namespace FreelancingHelper.Converters
{
    public class HasValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case string str:
                    return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);

                default:
                    return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
