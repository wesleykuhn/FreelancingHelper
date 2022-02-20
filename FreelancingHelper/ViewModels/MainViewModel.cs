using FreelancingHelper.CommandModels;
using FreelancingHelper.Models;
using FreelancingHelper.Services.Objects;
using FreelancingHelper.Services.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace FreelancingHelper.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _elapsedTimeString = "00:00:00";
        public string ElapsedTimeString
        {
            get => _elapsedTimeString;
            set => SetProperty(ref _elapsedTimeString, value);
        }

        private ICommand _startPauseCommand;
        public ICommand StartPauseCommand => _startPauseCommand ??= new BasicCommand(async () => await StartPauseCommandExecute());

        private ICommand _stopCommand;
        public ICommand StopCommand => _stopCommand ??=  new BasicCommand(async () => await StopCommandExecute());

        private ICommand _openConfigsCommand;
        public ICommand OpenConfigsCommand => _openConfigsCommand ??= new BasicCommand(async () => await OpenConfigsCommandExecute());

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new BasicCommand(async () => await CloseCommandExecute());

        private ICommand _openHirersManagerCommand;
        public ICommand OpenHirersManagerCommand => _openHirersManagerCommand ??=  new BasicCommand(async () => await OpenHirersManagerCommandExecute());

        private ICommand _openHistoryCommand;
        public ICommand OpenHistoryCommand => _openHistoryCommand ??=  new BasicCommand(async () => await OpenHistoryCommandExecute());

        #region [ TIMING CONTROL ]

        private bool _running;
        public bool Running
        {
            get => _running;
            set => SetProperty(ref _running, value);
        }

        private bool _hasCurDayWork;
        public bool HasCurDayWork
        {
            get => _hasCurDayWork;
            set => SetProperty(ref _hasCurDayWork, value);
        }

        private const int OneSecondMilliseconds = 990;

        private Timer _timer = new(OneSecondMilliseconds);
        private Stopwatch _deltaWatcher = new();
        private Stopwatch _totalTimeWatcher = new();

        #endregion

        private DayWork _curDayWork;
        private TimeSpan _lastWorkingTimeDuration;

        private IHirerService _hirerService;
        private ISettingsService _settingsService;
        private IDayWorkService _dayWorkService;
        public MainViewModel()
        {
            _hirerService = App.ServiceProvider.GetService<IHirerService>();
            _settingsService = App.ServiceProvider.GetService<ISettingsService>();
            _dayWorkService = App.ServiceProvider.GetService<IDayWorkService>();

            Navigation.TrySetupNavigationStack(this);

            _timer.Elapsed += new ElapsedEventHandler(OnTimerIntervalElapsed);

            IsBusy = true;
        }

        #region [ NAVIGATION ]

        public override async Task InitAsync(object args = null)
        {
            await _hirerService.LoadAllHirers();

            _curDayWork = await _dayWorkService.SeekForTodaysDayWork();

            if (_curDayWork != null)
            {
                HasCurDayWork = true;

                RecoveryUI();
            }

            IsBusy = false;
        }

        private async Task OpenConfigsCommandExecute()
        {
            await Navigation.GoAsync<ConfigsViewModel>();
        }

        private async Task OpenHirersManagerCommandExecute()
        {
            await Navigation.GoAsync<HirersManagerViewModel>();
        }

        private async Task OpenHistoryCommandExecute()
        {
            await Navigation.GoAsync<HistoryViewModel>();
        }

        #endregion


        #region [ TIME HANDLING ]

        private async Task StopCommandExecute()
        {
            var now = DateTime.Now;

            var conf = MessageBox.Show
            (
                $"Do you really want to stop Today's Work for the hirer {GetCurrentHirerName()}?",
                "CONFIRMATION",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (conf == MessageBoxResult.No)
                return;

            if (Running)
                await StartPauseCommandExecute();

            _curDayWork.Finished = now;
            await _dayWorkService.UpdateDayWork(_curDayWork);

            _curDayWork = null;

            HasCurDayWork = false;

            ResetUI();
        }

        private async Task StartPauseCommandExecute()
        {
            if (!Running)
            {
                if (_curDayWork == null)
                {
                    var now = DateTime.Now;

                    var conf = MessageBox.Show
                    (
                        $"Do you confirm the start of a new day work at {now.ToString("HH:mm:ss tt", CultureInfo.InvariantCulture)} for the hirer {GetCurrentHirerName()}?",
                        "CONFIRMATION",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if (conf == MessageBoxResult.No)
                        return;

                    var newDayWork = await _dayWorkService.CreateDayWork
                    (
                        _settingsService.AppConfiguration.CurrentSelectedHirerId,
                        now,
                        new List<WorkingTime>() { new WorkingTime(now) }
                    );

                    _curDayWork = newDayWork;
                }
                else
                    RecalculateCurDarkWorkTotalWorkingTime();

                //Isnt it a continuation? Then new working time!
                if (_curDayWork.DayWorkingTimes.Last().FinishedAt != DateTime.MinValue)
                    _curDayWork.DayWorkingTimes.Add(new WorkingTime(DateTime.Now));

                await _dayWorkService.UpdateDayWork(_curDayWork);

                Running = true;
                HasCurDayWork = true;

                _timer.Start();
            }
            else
            {
                _timer.Stop();

                _curDayWork.DayWorkingTimes.Last().FinishedAt = DateTime.Now;

                RecalculateCurDarkWorkTotalWorkingTime();

                await _dayWorkService.UpdateDayWork(_curDayWork);

                Running = false;
            }
        }

        private void RecalculateCurDarkWorkTotalWorkingTime()
        {
            _curDayWork.TotalWorkingTime = TimeSpan.Zero;
            foreach (var workingTime in _curDayWork.DayWorkingTimes)
            {
                if (workingTime.FinishedAt != DateTime.MinValue)
                {
                    _curDayWork.TotalWorkingTime += workingTime.FinishedAt - workingTime.StartedAt;
                }
            }
        }

        private void OnTimerIntervalElapsed(object sender, ElapsedEventArgs e)
        {
            UpdateUI();
        }

        private void RecoveryUI()
        {
            foreach (var workingTime in _curDayWork.DayWorkingTimes)
            {
                if (workingTime.FinishedAt != DateTime.MinValue)
                    _lastWorkingTimeDuration += workingTime.FinishedAt - workingTime.StartedAt;
            }

            if (_curDayWork.DayWorkingTimes.Last().FinishedAt == DateTime.MinValue)
                _lastWorkingTimeDuration += DateTime.Now - _curDayWork.DayWorkingTimes.Last().StartedAt;

            ElapsedTimeString = _lastWorkingTimeDuration.ToString(@"hh\:mm\:ss");
        }

        private void UpdateUI()
        {
            _lastWorkingTimeDuration = (DateTime.Now - _curDayWork.DayWorkingTimes.Last().StartedAt) + _curDayWork.TotalWorkingTime;

            ElapsedTimeString = _lastWorkingTimeDuration.ToString(@"hh\:mm\:ss");
        }

        private void ResetUI()
        {
            _lastWorkingTimeDuration = TimeSpan.Zero;
            ElapsedTimeString = "00:00:00";
        }

        private string GetCurrentHirerName() =>
            _hirerService.Hirers.Where(w => w.Id == _settingsService.AppConfiguration.CurrentSelectedHirerId).FirstOrDefault().Name;

        #endregion

        #region [ WINDOW CONTROL ]

        private async Task CloseCommandExecute()
        {
            var conf = MessageBox.Show
            (
                "Do you really want to close the program?",
                "CONFIRMATION",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (conf == MessageBoxResult.Yes)
            {
                if (Running)
                {
                    conf = MessageBox.Show
                    (
                        "The timer is still running! Do you want to automatically pause it and close the program?",
                        "CONFIRMATION",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if (conf == MessageBoxResult.Yes)
                        await StartPauseCommandExecute();
                }
                else if (_curDayWork != null && _curDayWork.Finished == DateTime.MinValue)
                {
                    conf = MessageBox.Show
                    (
                        "You current have an unfinished Day Work! If you don't stop it until the end of today it won't be able to be send as e-mail. Are you sure you want to leave without STOP it?",
                        "WARNING",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (conf == MessageBoxResult.No)
                        return;
                }

                Application.Current.Shutdown();
            }
        }

        #endregion
    }
}
