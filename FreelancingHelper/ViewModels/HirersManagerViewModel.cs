using FreelancingHelper.CommandModels;
using FreelancingHelper.Models;
using FreelancingHelper.Services.Objects;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using FreelancingHelper.Services.Settings;

namespace FreelancingHelper.ViewModels
{
    public class HirersManagerViewModel : BaseViewModel
    {
        private ObservableCollection<Hirer> _allHirers;
        public ObservableCollection<Hirer> AllHirers
        {
            get => _allHirers ??= new ObservableCollection<Hirer>();
            set => SetProperty(ref _allHirers, value);
        }

        private Hirer _selectedHirerComboBox;
        public Hirer SelectedHirerComboBox
        {
            get => _selectedHirerComboBox;
            set => SetProperty(ref _selectedHirerComboBox, value);
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new BasicCommand(async () => await CloseCommandExecute());

        private ICommand _openNewHirerCommand;
        public ICommand OpenNewHirerCommand => _openNewHirerCommand ??= new BasicCommand(async () => await OpenNewHirerCommandExecute());

        private ICommand _hirerDoubleClickCommand;
        public ICommand HirerDoubleClickCommand => _hirerDoubleClickCommand ??= new ParamCommand(async (param) => await HirerDoubleClickCommandExecute(param));

        private ICommand _hirerRightClickCommand;
        public ICommand HirerRightClickCommand => _hirerRightClickCommand ??= new ParamCommand((param) => HirerRightClickCommandExecute(param));

        private ICommand _curHirerChangedCommand;
        public ICommand CurHirerChangedCommand => _curHirerChangedCommand ??= new BasicCommand(async () => await CurHirerChangedCommandExecute());

        private IHirerService _hirerService;
        private ISettingsService _settingsService;
        public HirersManagerViewModel()
        {
            _hirerService = App.ServiceProvider.GetService<IHirerService>();
            _settingsService = App.ServiceProvider.GetService<ISettingsService>();
        }

        public override Task InitAsync(object args = null)
        {
            var allHirers = _hirerService.Hirers.OrderBy(h => h.Id).ToArray();

            AllHirers = new ObservableCollection<Hirer>(allHirers);

            var currentSelectedHirerId = _settingsService.AppConfiguration.CurrentSelectedHirerId;

            if (currentSelectedHirerId != -1 && AllHirers.Any(c => c.Id == currentSelectedHirerId))
            {
                SelectedHirerComboBox = AllHirers.Where(w => w.Id == currentSelectedHirerId).First();
            }

            return Task.CompletedTask;
        }

        public override Task BackAsync(object args = null)
        {
            if (args != null && args is Hirer navigatedHirer)
            {
                if (!AllHirers.Contains(navigatedHirer))
                    AllHirers.Add(navigatedHirer);
                else
                    AllHirers = new ObservableCollection<Hirer>(_hirerService.Hirers);
            }

            return Task.CompletedTask;
        }

        private ValueTask CloseCommandExecute() =>
            Navigation.BackAsync(this);

        private Task OpenNewHirerCommandExecute() =>
            Navigation.GoAsync<AddEditHirerViewModel>();

        private async Task HirerDoubleClickCommandExecute(object param)
        {
            if (param is ListViewItem listViewItem && listViewItem?.Content is Hirer hirer && hirer != null)
            {
                await Navigation.GoAsync<AddEditHirerViewModel>(hirer);
            }
        }

        private void HirerRightClickCommandExecute(object param)
        {
            if (param is ListViewItem listViewItem && listViewItem?.Content is Hirer hirer && hirer != null)
            {
                if (hirer.Id == _selectedHirerComboBox.Id)
                {
                    MessageBox.Show($"You cannot remove the hirer {hirer.Name} because it is the current selected hirer!" +
                        " Change it in the Combo Box below before trying again.");

                    return;
                }

                var conf = MessageBox.Show($"Do you really want to delete the hirer {hirer.Name}?", "CONFIRMATION", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (conf == MessageBoxResult.Yes)
                {
                    _hirerService.DeleteHirer(hirer);

                    AllHirers.Remove(hirer);
                }
            }
        }

        private async Task CurHirerChangedCommandExecute()
        {
            if (SelectedHirerComboBox == null ||
                SelectedHirerComboBox == default(Hirer) ||
                _settingsService.AppConfiguration.CurrentSelectedHirerId == SelectedHirerComboBox.Id)
                return;

            _settingsService.AppConfiguration.CurrentSelectedHirerId = SelectedHirerComboBox.Id;
            await _settingsService.SaveAppConfigurationAsync();
        }
    }
}
