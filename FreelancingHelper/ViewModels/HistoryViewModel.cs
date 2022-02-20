using FreelancingHelper.CommandModels;
using FreelancingHelper.Models;
using FreelancingHelper.Services.Objects;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FreelancingHelper.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        private ObservableCollection<DayWork> _allDayswork;
        public ObservableCollection<DayWork> AllDaysWork
        {
            get => _allDayswork;
            set => SetProperty(ref _allDayswork, value);
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new BasicCommand(async () => await CloseCommandExecute());

        private ICommand _dayWorkDoubleClickCommand;
        public ICommand DayWorkDoubleClickCommand => _dayWorkDoubleClickCommand ??= new ParamCommand(async (param) => await DayWorkDoubleClickCommandExecute(param));

        private IDayWorkService _dayWorkService;
        public HistoryViewModel()
        {
            IsBusy = true;

            _dayWorkService = App.ServiceProvider.GetService<IDayWorkService>();
        }

        public override async Task InitAsync(object args = null)
        {
            var daysWork = await _dayWorkService.GetAllDayWorks();

            AllDaysWork = daysWork?.Count() > 0 ?
                new ObservableCollection<DayWork>(daysWork) :
                new ObservableCollection<DayWork>();

            IsBusy = false;
        }

        private async Task CloseCommandExecute()
        {
            await Navigation.BackAsync(this);
        }

        private Task DayWorkDoubleClickCommandExecute(object param)
        {
            if (param is ListViewItem listViewItem && listViewItem?.Content is DayWork dayWork && dayWork != null)
            {
                return Navigation.GoAsync<DayWorkDetailsViewModel>(dayWork);
            }

            return Task.CompletedTask;
        }
    }
}
