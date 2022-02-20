using FreelancingHelper.Models;
using FreelancingHelper.Models.Interfaces;
using FreelancingHelper.Services.Serializator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

        public async Task LoadAppConfigurationAsync()
        {
            AppConfiguration = await _serializatorService.DesserializeAppConfigurationAsync();

            LoadAppsColors();
        }

        public async Task SaveAppConfigurationAsync() =>
            await _serializatorService.SerializeAppConfigurationAsync(AppConfiguration);

        public async Task GenerateDefaultAppConfiguration()
        {
            AppConfiguration = new AppConfiguration
            {
                CurLanguage = ConstantsAndSettings.AvailableLanguages.Where(w => w.Type == Enums.AppAvailableLanguageEnum.English).First(),
                CurrentSelectedHirerId = -1,
                PrimaryColor = Color.FromRgb
                (
                    ConstantsAndSettings.DefaultCrimsonPrimaryColorR,
                    ConstantsAndSettings.DefaultCrimsonPrimaryColorG,
                    ConstantsAndSettings.DefaultCrimsonPrimaryColorB
                ),
                HirerIdCounter = 0,
                DayWorkIdCounter = 0,
            };

            await SaveAppConfigurationAsync();
        }

        public bool CheckIfEmailSettingsAreSet() =>
            AppConfiguration.CurSmtpPort != 0 &&
            !string.IsNullOrEmpty(AppConfiguration.CurSmtpAddress) &&
            !string.IsNullOrEmpty(AppConfiguration.CurOriginEmail) &&
            !string.IsNullOrEmpty(AppConfiguration.CurOriginEmailPswd) &&
            !string.IsNullOrEmpty(AppConfiguration.DevName);

        #region [ ID COUNTERS ]

        public async Task<long> GetAndIncrementObjectTypeIdCounter<TModel>() where TModel : IAutoId
        {
            long id = -1;

            switch (typeof(TModel).Name)
            {
                case nameof(Hirer):
                    id = AppConfiguration.HirerIdCounter;
                    AppConfiguration.HirerIdCounter++;
                    break;

                case nameof(DayWork):
                    id = AppConfiguration.DayWorkIdCounter;
                    AppConfiguration.DayWorkIdCounter++;
                    break;
            }

            await SaveAppConfigurationAsync();

            return id;
        }

        #endregion

        #region [ COLORS ]

        private void LoadAppsColors()
        {
            SetAppsPrimaryColor(AppConfiguration.PrimaryColor);
        }

        public Color TrySetAppsPrimaryColorFromHexa(string newColorHexa)
        {
            if (string.IsNullOrEmpty(newColorHexa)
                || string.IsNullOrWhiteSpace(newColorHexa)
                || (newColorHexa.Length != 3 && newColorHexa.Length != 6 && newColorHexa.Length != 8))
            {
                return default(Color);
            }

            try
            {
                var newColorConverted = (Color)ColorConverter.ConvertFromString($"#{newColorHexa}");

                if (newColorConverted == default(Color))
                    return default(Color);

                SetAppsPrimaryColor(newColorConverted);

                return newColorConverted;
            }
            catch (Exception)
            {
                return default(Color);
            }
        }

        public void SetAppsPrimaryColor(Color newColor)
        {
            var newTransp4Color = Color.FromArgb(85, newColor.R, newColor.G, newColor.B);

            Application.Current.Resources["PrimaryColor"] = newColor;
            Application.Current.Resources["PrimarySolid"] = new SolidColorBrush(newColor);

            Application.Current.Resources["PrimaryTransp4Color"] = newTransp4Color;
            Application.Current.Resources["PrimaryTransp4Solid"] = new SolidColorBrush(newTransp4Color);

            AppConfiguration.PrimaryColor = newColor;
        }

        #endregion
    }
}
