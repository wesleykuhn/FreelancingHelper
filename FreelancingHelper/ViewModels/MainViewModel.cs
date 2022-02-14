using FreelancingHelper.CommandModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

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
        public ICommand StartPauseCommand => _startPauseCommand ??= new BasicCommand(StartPauseCommandExecute);

        private ICommand _openConfigsCommand;
        public ICommand OpenConfigsCommand => _openConfigsCommand ??= new BasicCommand(async () => await OpenConfigsCommandExecute());

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new BasicCommand(CloseCommandExecute);

        private ICommand _openHirersManagerCommand;
        public ICommand OpenHirersManagerCommand => _openHirersManagerCommand ??=  new BasicCommand(async () => await OpenHirersManagerCommandExecute());

        #region [ TIMING CONTROL ]

        private bool _running;
        public bool Running
        {
            get => _running;
            set => SetProperty(ref _running, value);
        }

        private const int OneSecondMilliseconds = 1000;

        private Timer _timer = new(OneSecondMilliseconds);
        private Stopwatch _deltaWatcher = new();
        private Stopwatch _totalTimeWatcher = new();

        #endregion

        public MainViewModel()
        {
            Navigation.TrySetupNavigationStack(this);

            _timer.Elapsed += OnTimerIntervalElapsed;
        }

        private async Task OpenConfigsCommandExecute()
        {
            await Navigation.Go<ConfigsViewModel>();
        }

        private async Task OpenHirersManagerCommandExecute()
        {
            await Navigation.Go<HirersManagerViewModel>();
        }

        #region [ TIME HANDLING ]

        private void StartPauseCommandExecute()
        {
            if (!_running)
            {
                Running = true;

                _totalTimeWatcher.Start(); //Total
                _deltaWatcher.Start();     //Delta
                _timer.Start();            //TIMER
            }
            else
            {
                Running = false;

                _totalTimeWatcher.Stop();
                _deltaWatcher.Stop();
                _timer.Stop();
            }
        }

        private void OnTimerIntervalElapsed(object sender, ElapsedEventArgs e)
        {
            HandleClockTime();

            ApplyAndResetDeltaTime();
        }

        private void HandleClockTime()
        {
            ElapsedTimeString = TimeSpan.FromMilliseconds(_totalTimeWatcher.ElapsedMilliseconds).ToString(@"hh\:mm\:ss");
        }

        //This delta time is to delay/accelerate the next interval, because of the irregular execution time.
        private void ApplyAndResetDeltaTime()
        {
            _deltaWatcher.Stop();

            _timer.Interval =  OneSecondMilliseconds - (_deltaWatcher.ElapsedMilliseconds - OneSecondMilliseconds);

            _deltaWatcher.Reset();

            _deltaWatcher.Start();
        }

        #endregion

        #region [ WINDOW CONTROL ]

        private void CloseCommandExecute()
        {
            var conf = MessageBox.Show("Do you really want to close the program?", "CONFIRMATION", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (conf == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        #endregion
    }
}
