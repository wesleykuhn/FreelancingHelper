using FreelancingHelper.Services.Navigation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace FreelancingHelper.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected NavigationService Navigation => NavigationService.Current;

        public Window BindedWindow;

        #region [ BUSY STATE ]

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                SetProperty(ref _isNotBusy, !value);
            }
        }

        private bool _isNotBusy = true;
        public bool IsNotBusy
        {
            get => _isNotBusy;
            private set => SetProperty(ref _isNotBusy, value);
        }

        #endregion

        #region [ PRE-SET NAVIGATION METHODS ]

        //Loaded when I come to the new page
        public virtual Task InitAsync(object args = null) => Task.CompletedTask;

        //Loaded when I came back to the page
        public virtual Task BackAsync(object args = null) => Task.CompletedTask;

        #endregion

        #region [ PROP WATCHER ]

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;

            backingStore = value;

            OnPropertyChanged(propertyName);

            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
