using System;
using System.Windows.Input;

namespace FreelancingHelper.CommandModels
{
    public class BasicCommand : ICommand
    {
        private Action _actionToExecute;
        private Func<bool> _canExecuteEvaluator;

        public BasicCommand(Action actionToExecute) : this(actionToExecute, null) { }

        public BasicCommand(Action actionToExecute, Func<bool> canExecuteAction)
        {
            _actionToExecute =  actionToExecute;
            _canExecuteEvaluator = canExecuteAction;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteEvaluator == null)
                return true;
            else
                return _canExecuteEvaluator.Invoke();
        }

        public void Execute(object? parameter)
        {
            _actionToExecute.Invoke();
        }
    }
}
