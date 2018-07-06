using System;
using System.Windows.Input;

namespace CardDispenserExample
{
    /// <summary>
    /// An entity which will help to reduce the code duplication with out having to implement ICommand methods.
    /// This class have to be inherited by the View Models or the classes which implement ICommand
    /// </summary>
    public class DelegateCommand<T> : ICommand where T : class
    {
        private readonly Action<T> _command;
        private readonly Func<T, bool> _canExecute;
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Initiate a Command needed for MVVM bindings.
        /// </summary>
        /// <param name="command">Pointer to the execute command method</param>
        /// <param name="canExecute">Pointer to Can Execute Command</param>
        public DelegateCommand(Action<T> command, Func<T, bool> canExecute = null)
        {
            _canExecute = canExecute;
            _command = command ?? throw new ArgumentNullException();
        }

        public void Execute(object parameter)
        {
            var converted = (T)parameter;
            _command(converted);
        }

        public bool CanExecute(object parameter)
        {
            var converted = (T)parameter;
            return _canExecute == null || _canExecute(converted);
        }
    }

    /// <summary>
    /// An entity which will help to reduce the code duplication with out having to implement ICommand methods.
    /// This class have to be inherited by the View Models or the classes which implement ICommand
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action _command;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Initiate a Command needed for MVVM bindings.
        /// </summary>
        /// <param name="command">Pointer to the execute command method</param>
        /// <param name="canExecute">Pointer to Can Execute Command</param>
        public DelegateCommand(Action command, Func<bool> canExecute = null)
        {
            _canExecute = canExecute;
            _command = command ?? throw new ArgumentNullException();
        }

        public void Execute(object parameter)
        {
            _command();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
    }
}

