using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CulturezVous.WP8.Utils
{
    /// <summary>
    /// Very simple implementation of the ICommand interface
    /// </summary>
    public class Command : ICommand
    {
        private Action m_execute;
        private Func<bool> m_canExecute;

        public Command(Action execute)
            : this(execute, null)
        { }

        public Command(Action execute, Func<bool> canExecute)
        {
            if (execute == null) throw new ArgumentException("execute");

            m_execute = execute;
            m_canExecute = canExecute;

            if (m_canExecute != null)
            {
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, new EventArgs());
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (m_canExecute != null)
            {
                return m_canExecute();
            }

            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            m_execute();
        }
    }
}
