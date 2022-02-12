using FreelancingHelper.CommandModels;
using FreelancingHelper.Services.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FreelancingHelper.ViewModels
{
    public class ConfigsViewModel : BaseViewModel
    {
        private string _primaryColorHex;
        public string PrimaryColorHex
        {
            get => _primaryColorHex;
            set
            {
                SetProperty(ref _primaryColorHex, value);

                if (value != _oldTypedColorHexa && value.Length == 8)
                    TryChangePrimaryColor();
                else
                    _newColor = default;
            }
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new GenericCommand(async () => await CloseCommandExecute());

        private string _oldTypedColorHexa;

        private Color _newColor = default;

        private readonly ISettingsService _settingsService;
        public ConfigsViewModel()
        {
            _settingsService = App.ServiceProvider.GetService<ISettingsService>();
        }

        private void TryChangePrimaryColor()
        {
            try
            {
                _newColor = (Color)ColorConverter.ConvertFromString($"#{PrimaryColorHex}");

                Application.Current.Resources["PrimaryColor"] = _newColor;

                Application.Current.Resources["PrimarySolid"] = new SolidColorBrush(_newColor);
            }
            catch (Exception)
            {
                _newColor = default;

                return;
            }
        }

        public override Task InitAsync(object args = null)
        {
            var curBrush = new SolidColorBrush(_settingsService.AppConfiguration.PrimaryColor);

            _oldTypedColorHexa = curBrush.Color.ToString().Replace("#", "");
            PrimaryColorHex = _oldTypedColorHexa;

            return Task.CompletedTask;
        }

        private async Task CloseCommandExecute()
        {
            _settingsService.AppConfiguration.PrimaryColor = _newColor;

            await _settingsService.SaveAppConfigurationAsync();

            BindedWindow.Close();
        }
    }
}
