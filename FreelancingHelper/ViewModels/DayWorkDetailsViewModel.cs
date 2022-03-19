using FreelancingHelper.CommandModels;
using FreelancingHelper.Models;
using FreelancingHelper.Services.Email;
using FreelancingHelper.Services.Objects;
using FreelancingHelper.Services.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FreelancingHelper.ViewModels
{
    public class DayWorkDetailsViewModel : BaseViewModel
    {
        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new BasicCommand(async () => await CloseCommandExecute());

        private long _id;
        public long Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _hirerToString;
        public string HirerToString
        {
            get => _hirerToString;
            set => SetProperty(ref _hirerToString, value);
        }

        private Hirer _hirer;
        public Hirer Hirer
        {
            get => _hirer;
            set => SetProperty(ref _hirer, value);
        }

        private DateTime _startedAt;
        public DateTime StartedAt
        {
            get => _startedAt;
            set => SetProperty(ref _startedAt, value);
        }

        private DateTime _finishedAt;
        public DateTime FinishedAt
        {
            get => _finishedAt;
            set => SetProperty(ref _finishedAt, value);
        }

        private bool _hasFinish;
        public bool HasFinish
        {
            get => _hasFinish;
            set => SetProperty(ref _hasFinish, value);
        }

        private TimeSpan _totalTime;
        public TimeSpan TotalTime
        {
            get => _totalTime;
            set => SetProperty(ref _totalTime, value);
        }

        private ObservableCollection<WorkingTime> _workingTimes;
        public ObservableCollection<WorkingTime> WorkingTimes
        {
            get => _workingTimes;
            set => SetProperty(ref _workingTimes, value);
        }

        private ICommand _sendEmailCommand;
        public ICommand SendEmailCommand => _sendEmailCommand ??= new BasicCommand(async () => await SendEmailCommandExecute());

        private ICommand _finishDayWorkCommand;
        public ICommand FinishDayWorkCommand => _finishDayWorkCommand ??= new BasicCommand(async () => await FinishDayWorkCommandExecute());

        private IHirerService _hirerService;
        private IEmailService _emailService;
        private ISettingsService _settingsService;
        private IDayWorkService _dayWorkService;
        public DayWorkDetailsViewModel()
        {
            _hirerService = App.ServiceProvider.GetService<IHirerService>();
            _emailService = App.ServiceProvider.GetService<IEmailService>();
            _settingsService = App.ServiceProvider.GetService<ISettingsService>();
            _dayWorkService = App.ServiceProvider.GetService<IDayWorkService>();
        }

        private DayWork _dayWork;

        public override Task InitAsync(object args = null)
        {
            if (args != null && args is DayWork dayWork)
            {
                _dayWork = dayWork;

                Id = dayWork.Id;
                StartedAt = dayWork.Started;
                FinishedAt = dayWork.Finished;
                TotalTime = dayWork.TotalWorkingTime;

                WorkingTimes = dayWork.DayWorkingTimes.Count > 0 ?
                    new ObservableCollection<WorkingTime>(dayWork.DayWorkingTimes) :
                    new ObservableCollection<WorkingTime>();
                
                var hirer = _hirerService.Hirers.Where(w => w.Id == dayWork.HirerId).FirstOrDefault();
                Hirer = hirer ?? null;
                HirerToString = hirer.ToString() ?? string.Empty;

                HasFinish = FinishedAt != DateTime.MinValue;
            }

            return Task.CompletedTask;
        }

        private async Task CloseCommandExecute()
        {
            await Navigation.BackAsync(this);
        }

        private async Task SendEmailCommandExecute()
        {
            if (Hirer == null || string.IsNullOrEmpty(Hirer.Email))
            {
                MessageBox.Show("The Hirer was deleted or its e-mail is currupted!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_dayWork.Finished == DateTime.MinValue)
            {
                MessageBox.Show("This Day Work can't be send as e-mail because it wasn't finished.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_settingsService.CheckIfEmailSettingsAreSet())
            {
                MessageBox.Show("You didn't set the e-mail sending settings right! Go to CONFIGURATIONS and set it.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IsBusy = true;

            var result = await _emailService.SetMailMessageAsWorkingTimeReport(Hirer.Email, _dayWork).Send(out string exceptionMessage);

            IsBusy = false;

            if (result)
                MessageBox.Show("E-mail sent successful!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("There was an error while trying to send the e-mail!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async Task FinishDayWorkCommandExecute()
        {
            if (HasFinish)
                return;

            if (!_dayWork.DayWorkingTimes.Any() || _dayWork.DayWorkingTimes.Last().FinishedAt == DateTime.MinValue)
            {
                MessageBox.Show("The Day Work is corrupted or the last pause finish is not set! Please, verify before trying again.",
                    "ERROR",
                    MessageBoxButton.OK);
                return;
            }

            var conf = MessageBox.Show("Do you really want to finish this day work?", "CONFIRMATION", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (conf != MessageBoxResult.Yes)
                return;

            _dayWork.Finished = _dayWork.DayWorkingTimes.Last().FinishedAt;

            await _dayWorkService.UpdateDayWork(_dayWork);

            FinishedAt = _dayWork.Finished;
            HasFinish = true;

            MessageBox.Show("The day work was successfuly finished!", "SUCCESS", MessageBoxButton.OK);
        }
    }
}
