using FreelancingHelper.Models;
using FreelancingHelper.Services.Objects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FreelancingHelper.Converters
{
    public class HirerIdToStringConverter : IValueConverter
    {
        private IHirerService _hirerService;
        public HirerIdToStringConverter()
        {
            _hirerService = App.ServiceProvider.GetService<IHirerService>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long id)
            {
                var hirer = _hirerService.Hirers.Where(h => h.Id == id).FirstOrDefault();

                return (hirer == null || hirer == default(Hirer)) ? "Deleted Hirer" : hirer.ToString();
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
