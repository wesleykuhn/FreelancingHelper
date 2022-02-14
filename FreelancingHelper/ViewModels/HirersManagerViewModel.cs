using FreelancingHelper.CommandModels;
using FreelancingHelper.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

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

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new BasicCommand(async () => await CloseCommandExecute());

        private async ValueTask CloseCommandExecute()
        {
            BindedWindow.Close();
        }
    }
}
