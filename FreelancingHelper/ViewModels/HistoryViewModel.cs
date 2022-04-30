using FreelancingHelper.CommandModels;
using FreelancingHelper.Extensions;
using FreelancingHelper.Models;
using FreelancingHelper.Services.Email;
using FreelancingHelper.Services.Objects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

        private string _multipleHistories;
        public string MultipleHistories
        {
            get => _multipleHistories;
            set => SetProperty(ref _multipleHistories, value);
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new BasicCommand(async () => await CloseCommandExecute());

        private ICommand _dayWorkDoubleClickCommand;
        public ICommand DayWorkDoubleClickCommand => _dayWorkDoubleClickCommand ??= new ParamCommand(async (param) => await DayWorkDoubleClickCommandExecute(param));

        private ICommand _sendMultipleCommand;
        public ICommand SendMultipleCommand => _sendMultipleCommand ??= new BasicCommand(async () => await SendMultipleCommandExecute());

        private IDayWorkService _dayWorkService;
        private readonly IEmailService _emailService;
        private readonly IHirerService _hirerService;
        public HistoryViewModel()
        {
            IsBusy = true;

            _dayWorkService = App.ServiceProvider.GetService<IDayWorkService>();
            _emailService = App.ServiceProvider.GetService<IEmailService>();
            _hirerService = App.ServiceProvider.GetService<IHirerService>();
        }

        public override async Task InitAsync(object args = null)
        {
            var daysWork = (await _dayWorkService.GetAllDayWorks()).OrderBy(o => o.Id).ToArray();

            AllDaysWork = daysWork.Any() ?
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

        private async Task SendMultipleCommandExecute()
        {
            if (MultipleHistories.IsNullOrEmptyOrWhiteSpace())
            {
                MessageBox.Show("The IDs of the histories are invalid! Please, verify and try again.", "ERROR", MessageBoxButton.OK);
                return;
            }

            var trimmed = MultipleHistories.Trim();
            var idsStringArray = trimmed.Split(',');

            List<long> idsList = new();
            foreach (var id in idsStringArray)
            {
                if (id.Contains("-"))
                {
                    var interval = id.Split('-');

                    if (interval.Length != 2)
                    {
                        MessageBox.Show("The IDs of the histories are invalid! Please, valid format is X-Y.", "ERROR", MessageBoxButton.OK);
                        return;
                    }

                    var parseResultInitial = long.TryParse(interval[0], out long initial);
                    var parseResultFinal = long.TryParse(interval[1], out long final);

                    if (!parseResultFinal || !parseResultInitial)
                    {
                        MessageBox.Show("The IDs of the histories are invalid! Please, valid format is X-Y (Numbers only!).", "ERROR", MessageBoxButton.OK);
                        return;
                    }

                    if (initial >= final)
                    {
                        MessageBox.Show("The IDs of the histories are invalid! Please, valid format is X-Y (Where X is less than Y).", "ERROR", MessageBoxButton.OK);
                        return;
                    }

                    for (long i = initial; i <= final; i++)
                    {
                        idsList.Add(i);
                    }
                }
                else
                {
                    var parseResult = long.TryParse(id, out long parsed);

                    if (!parseResult)
                    {
                        MessageBox.Show("The IDs of the histories are invalid! Please, verify (Numbers only!).", "ERROR", MessageBoxButton.OK);
                        return;
                    }

                    if (parseResult)
                        idsList.Add(parsed);
                }
            }

            if (!idsList.Any())
                return;

            var daysWork = await _dayWorkService.GetOnlyDayWorksAsync(idsList);

            if (daysWork.Last().Finished == DateTime.MinValue)
            {
                MessageBox.Show("The finished date of the last entered day's work ID, is not set (Completed). Please, verify before trying again.",
                    "ERROR",
                    MessageBoxButton.OK);
                return;
            }

            if (daysWork.Any(a => a.TotalWorkingTime == TimeSpan.MinValue))
            {
                MessageBox.Show("One of the Days Work hasn't a total working time set! Please, verify before trying again.",
                    "ERROR",
                    MessageBoxButton.OK);
                return;
            }

            var hirerId = daysWork.First().HirerId;
            if (daysWork.Any(a => a.HirerId != hirerId))
            {
                MessageBox.Show("The Hirer from one or more Day Work is/are different. Please, verify before trying again.",
                    "ERROR",
                    MessageBoxButton.OK);
                return;
            }

            var hirerEmail = _hirerService.Hirers.Where(w => w.Id == hirerId).Select(s => s.Email).FirstOrDefault();
            if (hirerEmail.IsNullOrEmptyOrWhiteSpace())
            {
                MessageBox.Show("The hirer of the Days Work was deleted or is corrupted!",
                    "ERROR",
                    MessageBoxButton.OK);
                return;
            }

            var result = await _emailService.SetMailMessageAsWorkingTimeReportList(hirerEmail, daysWork).Send(out string exceptionMessage);
            if (!result)
            {
                MessageBox.Show("There was an error while trying to send the e-mail! Details:" + exceptionMessage,
                    "ERROR",
                    MessageBoxButton.OK);
                return;
            }

            MessageBox.Show("The E-mail was successfuly sent!", "SUCCESS", MessageBoxButton.OK);
        }
    }
}
