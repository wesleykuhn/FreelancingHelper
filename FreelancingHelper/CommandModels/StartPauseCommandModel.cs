using System;
using System.Windows.Input;

namespace FreelancingHelper.CommandModels
{
    public class StartPauseCommandModel : ICommand
    {
        private Action _actionToExecute;
        public StartPauseCommandModel(Action actionToExecute)
        {
            _actionToExecute =  actionToExecute;
        }

        public bool CanExecute(object? parameter) => true;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object? parameter)
        {
            _actionToExecute.Invoke();
        }
    }
}
