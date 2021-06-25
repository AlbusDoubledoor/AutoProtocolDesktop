using System;
using System.Windows.Input;

namespace AutoProtocol
{
    /*
     * Класс команд в приложении (MVVM)
     * Позволяет использовать команды с использованием делегата действия Action
     */
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        /*
         * Событие изменения доступности команды на исполнение
         */
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /*
         * Основной конструктор, который принимает делегат Action для исполнения и делегат Func для доступности исполнения
         */
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
