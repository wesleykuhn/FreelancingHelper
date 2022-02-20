using FreelancingHelper.CommandModels;
using FreelancingHelper.Enums;
using FreelancingHelper.Models;
using FreelancingHelper.Services.Objects;
using FreelancingHelper.Services.Serializator;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FreelancingHelper.ViewModels
{
    public class AddEditHirerViewModel : BaseViewModel
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _doneButtonText = "Add";
        public string DoneButtonText
        {
            get => _doneButtonText;
            set => SetProperty(ref _doneButtonText, value);
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new BasicCommand(async () => await CloseCommandExecute());

        private ICommand _addEditCommand;
        public ICommand AddEditCommand => _addEditCommand ??= new BasicCommand(async () => await AddEditCommandExecute());

        private Hirer _hirerToEdit;

        private readonly IHirerService _hirerService;

        public AddEditHirerViewModel()
        {
            _hirerService = App.ServiceProvider.GetService<IHirerService>();
        }

        public override Task InitAsync(object args = null)
        {
            if (args != null && args is Hirer hirer)
            {
                _hirerToEdit = hirer;

                Name = hirer.Name;
                Email = hirer.Email;

                DoneButtonText = "Edit";
            }

            return Task.CompletedTask;
        }

        private ValueTask CloseCommandExecute() =>
            Navigation.BackAsync(this);

        private async Task AddEditCommandExecute()
        {
            Hirer hirerToSendBack;

            if (_hirerToEdit == null)
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrWhiteSpace(Name) || string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(Email))
                {
                    MessageBox.Show("The E-mail and Name fields are required!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Email.Contains('@') || !Email.Contains('.') || Email.Length < 5 || Email.IndexOf('@') > Email.LastIndexOf('.'))
                {
                    MessageBox.Show("The E-mail is invalid!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                hirerToSendBack = await _hirerService.AddHirer(Name, Email);
            }
            else
            {
                _hirerToEdit.Name = Name;
                _hirerToEdit.Email = Email;

                await _hirerService.UpdateHirer(_hirerToEdit);

                hirerToSendBack = _hirerToEdit;
            }

            await Navigation.BackAsync(this, hirerToSendBack);
        }
    }
}
