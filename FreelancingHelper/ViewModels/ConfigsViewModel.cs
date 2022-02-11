using FreelancingHelper.Services.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FreelancingHelper.ViewModels
{
    public class ConfigsViewModel : BaseViewModel
    {
        private string _primaryColorHex;
        public string PrimaryColorHex
        {
            get => _primaryColorHex;
            set => SetProperty(ref _primaryColorHex, value);
        }

        private readonly ISettingsService _settingsService;
        public ConfigsViewModel()
        {
            _settingsService = App.ServiceProvider.GetService<ISettingsService>();

            var curBrush = new SolidColorBrush(_settingsService.AppConfiguration.PrimaryColor);
            PrimaryColorHex = curBrush.Color.ToString();
            //ColorConverter.ConvertFromString(_settingsService.AppConfiguration.PrimaryColor);
        }
    }
}
