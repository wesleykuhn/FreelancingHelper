using FreelancingHelper.CommandModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
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
        public ICommand StartPauseCommand => _startPauseCommand ??= new GenericCommand(StartPauseCommandExecute);

        private ICommand _openConfigsCommand;
        public ICommand OpenConfigsCommand => _openConfigsCommand ??= new GenericCommand(async () => await OpenConfigsCommandExecute());

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

        #region [ UI CONTROL ]

        internal bool WindowIsVisible;

        #endregion

        public MainViewModel()
        {
            _timer.Elapsed += OnTimerIntervalElapsed;
        }

        private async Task OpenConfigsCommandExecute()
        {
            await Navigation.ShowWindow<ConfigsViewModel>();
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
    }
}
