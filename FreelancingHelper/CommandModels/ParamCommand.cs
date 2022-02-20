using System;
using System.Windows.Input;

namespace FreelancingHelper.CommandModels
{
    public class ParamCommand : ICommand
    {
        private Action<object> _actionToExecute;
        private Func<bool> _canExecuteEvaluator;

        public ParamCommand(Action<object> actionToExecute) : this(actionToExecute, null) { }

        public ParamCommand(Action<object> actionToExecute, Func<bool> canExecuteAction)
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
            _actionToExecute.Invoke(parameter);
        }
    }
}
