using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sensing4U_MVP.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        private Action<object> execute;
        /// <summary>
        /// 
        /// </summary>
        private Func<object, bool> canExecute;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}
