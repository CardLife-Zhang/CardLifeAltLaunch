using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CardLifeAltLaunch
{
    public class VMCommand : ICommand
    {
        public VMCommand(Action<object> anExecuteAction, Func<object, bool> aCanExecuteFunc)
        {
            ExecuteAction = anExecuteAction;
            CanExecuteFunc = aCanExecuteFunc;
        }

        public Action<object> ExecuteAction { get; }
        public Func<object, bool> CanExecuteFunc { get; }

        public event EventHandler CanExecuteChanged;

        public void onCanExecuteChanged()
        {
            if(CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteAction(parameter);
        }
    }
}
