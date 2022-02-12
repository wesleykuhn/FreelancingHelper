using System;
using System.Windows.Input;

namespace FreelancingHelper.CommandModels
{
    public class GenericCommand : ICommand
    {
        private Action _actionToExecute;
        private Func<bool> _canExecuteEvaluator;

        public GenericCommand(Action actionToExecute) : this(actionToExecute, null) { }

        public GenericCommand(Action actionToExecute, Func<bool> canExecuteAction)
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
