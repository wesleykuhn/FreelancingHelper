using FreelancingHelper.Models;
using FreelancingHelper.Services.Serializator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FreelancingHelper.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        public AppConfiguration AppConfiguration { get; private set; }

        private readonly ISerializatorService _serializatorService;
        public SettingsService(ISerializatorService serializatorService)
        {
            _serializatorService = serializatorService;
        }

        public async Task LoadAppConfigurationAsync() =>
            AppConfiguration = await _serializatorService.DesserializeAppConfigurationAsync();

        public async Task SaveAppConfigurationAsync() =>
            await _serializatorService.SerializeAppConfigurationAsync(AppConfiguration);

        public async Task GenerateDefaultAppConfiguration()
        {
            AppConfiguration = new AppConfiguration
            {
                PrimaryColor = Color.FromRgb(Constants.DefaultCrimsonPrimaryColorR, Constants.DefaultCrimsonPrimaryColorG, Constants.DefaultCrimsonPrimaryColorB)
            };

            await SaveAppConfigurationAsync();
        }
    }
}
