using FreelancingHelper.Models;
using FreelancingHelper.Services.Serializator;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FreelancingHelper.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        public AppConfiguration AppConfiguration { get; private set; }
        //public DayWork

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
                PrimaryColor = Color.FromRgb(ConstantsAndSettings.DefaultCrimsonPrimaryColorR, ConstantsAndSettings.DefaultCrimsonPrimaryColorG, ConstantsAndSettings.DefaultCrimsonPrimaryColorB)
            };

            await SaveAppConfigurationAsync();
        }

        #region [ COLORS ]

        private void LoadAppsColors()
        {
            SetAppsPrimaryColor(AppConfiguration.PrimaryColor);
        }

        /// <summary>
        /// Tries to set the current app's primary color.
        /// </summary>
        /// <param name="newColorHexa">The new color in hexadecimal, without the '#' character. You can use 3, 6 or even 8(2 for Alpha) digits.</param>
        /// <returns>The new color if it was successful or a default(Color) if not.</returns>
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

        private void SetAppsPrimaryColor(Color newColor)
        {
            var newTransp4Color = Color.FromArgb(68, newColor.R, newColor.G, newColor.B);

            Application.Current.Resources["PrimaryColor"] = newColor;
            Application.Current.Resources["PrimarySolid"] = new SolidColorBrush(newColor);

            Application.Current.Resources["PrimaryTransp4Color"] = newTransp4Color;
            Application.Current.Resources["PrimaryTransp4Solid"] = new SolidColorBrush(newTransp4Color);
        }

        public async Task SaveAppsPrimaryColor(Color newColor)
        {
            AppConfiguration.PrimaryColor = newColor;

            await SaveAppConfigurationAsync();
        }

        #endregion
    }
}
