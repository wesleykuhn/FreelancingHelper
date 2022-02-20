using FreelancingHelper.CommandModels;
using FreelancingHelper.Extensions;
using FreelancingHelper.Models;
using FreelancingHelper.Services.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
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
                    _newColor = _settingsService.TrySetAppsPrimaryColorFromHexa(PrimaryColorHex);
                else
                    _newColor = default;
            }
        }

        private AppLanguage _selectedLanguage;
        public AppLanguage SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        private ObservableCollection<AppLanguage> _languages;
        public ObservableCollection<AppLanguage> Languages
        {
            get => _languages;
            set => SetProperty(ref _languages, value);
        }

        private string _devName;
        public string DevName
        {
            get => _devName;
            set => SetProperty(ref _devName, value);
        }

        private string _smtpAddress;
        public string SmtpAddress
        {
            get => _smtpAddress;
            set => SetProperty(ref _smtpAddress, value);
        }

        private string _smtpPort;
        public string SmtpPort
        {
            get => _smtpPort;
            set => SetProperty(ref _smtpPort, value);
        }

        private string _devEmail;
        public string DevEmail
        {
            get => _devEmail;
            set => SetProperty(ref _devEmail, value);
        }

        private string _devEmailPswd;
        public string DevEmailPswd
        {
            get => _devEmailPswd;
            set => SetProperty(ref _devEmailPswd, value);
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new BasicCommand(async () => await CloseCommandExecute());

        private string _oldTypedColorHexa;

        private Color _newColor = default(Color);

        private readonly ISettingsService _settingsService;
        public ConfigsViewModel()
        {
            _settingsService = App.ServiceProvider.GetService<ISettingsService>();
        }

        public override Task InitAsync(object args = null)
        {
            var curBrush = new SolidColorBrush(_settingsService.AppConfiguration.PrimaryColor);

            _oldTypedColorHexa = curBrush.Color.ToString().Replace("#", "");
            PrimaryColorHex = _oldTypedColorHexa;

            DevName = _settingsService.AppConfiguration.DevName;
            DevEmail = _settingsService.AppConfiguration.CurOriginEmail;
            DevEmailPswd = _settingsService.AppConfiguration.CurOriginEmailPswd;
            SmtpAddress = _settingsService.AppConfiguration.CurSmtpAddress;
            SmtpPort = _settingsService.AppConfiguration.CurSmtpPort.ToString();

            Languages = new ObservableCollection<AppLanguage>(ConstantsAndSettings.AvailableLanguages);
            SelectedLanguage = _settingsService.AppConfiguration.CurLanguage;

            return Task.CompletedTask;
        }

        private async ValueTask CloseCommandExecute()
        {
            if (_newColor != default(Color))
                _settingsService.SetAppsPrimaryColor(_newColor);

            var parseResult = int.TryParse(SmtpPort, out int parsed);
            if (!parseResult)
            {
                MessageBox.Show("The SMTP port must be a number!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _settingsService.AppConfiguration.CurSmtpPort = parsed < 0 ? 0 : parsed;
            _settingsService.AppConfiguration.DevName = DevName.IsNullOrEmptyOrWhiteSpace() ? null : DevName;
            _settingsService.AppConfiguration.CurSmtpAddress = SmtpAddress.IsNullOrEmptyOrWhiteSpace() ? null : SmtpAddress;
            _settingsService.AppConfiguration.CurOriginEmail = DevEmail.IsNullOrEmptyOrWhiteSpace() ? null : DevEmail;
            _settingsService.AppConfiguration.CurOriginEmailPswd = DevEmailPswd.IsNullOrEmptyOrWhiteSpace() ? null : DevEmailPswd;
            _settingsService.AppConfiguration.CurLanguage  = SelectedLanguage;

            await _settingsService.SaveAppConfigurationAsync();

            await Navigation.BackAsync(this);
        }
    }
}
