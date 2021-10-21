﻿using System;
using System.Windows.Input;

namespace Binder
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// <code>
    /// public RelayCommand AddCommand
    /// {
    ///     get
    ///     {
    ///         return addCommand ?? (addCommand = new RelayCommand(obj =>
    ///         {
    ///             *Some code*
    ///         }));
    ///     }
    /// }
    /// </code>
    /// </example>
    internal class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;
        
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        
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

/*
get
            {
                return _showHideWindowCommand ??
                  (_showHideWindowCommand = new RelayCommand(obj =>
                  {
                      if (_parentWindow.IsVisible)
                      {
                          _parentWindow.Hide();
                      }
                      else
                      {
                          _parentWindow.Show();
                      }
                  }));
            }
*/