using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HiNotes
{
    public class CommandDeleteTask : ICommand
    {
        private TaskListViewModel _listViewModel;
        public CommandDeleteTask(TaskListViewModel listViewModel)
        {
            if (listViewModel == null) throw new ArgumentNullException("Parameter list is empty");
            _listViewModel = listViewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            TaskViewModel task = parameter as TaskViewModel;
            _listViewModel.TaskList.Remove(task);
            _listViewModel.SortByPosition();
        }
    }
}
