using FreelancingHelper.CommandModels;
using FreelancingHelper.Services.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
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

            return Task.CompletedTask;
        }

        private async ValueTask CloseCommandExecute()
        {
            if (_newColor != default(Color))
                await _settingsService.SaveAppsPrimaryColor(_newColor);

            BindedWindow.Close();
        }
    }
}
