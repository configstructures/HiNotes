using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HiNotes
{
    public class CommandAddNewTask : ICommand
    {
        private TaskListViewModel _listViewModel;
        public CommandAddNewTask(TaskListViewModel listViewModel)
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
            TaskViewModel newTask = new TaskViewModel();

            if (_listViewModel.TaskList.Count > 0)
            {
                newTask.TaskPosition = newTask.TaskHomePosition = _listViewModel.TaskList[_listViewModel.TaskList.Count - 1].TaskHomePosition +
                                                                  _listViewModel.TaskList[_listViewModel.TaskList.Count - 1].TaskBorderHeight + 10;
                newTask.Zone = _listViewModel.TaskList[_listViewModel.TaskList.Count - 1].Zone + 1;
            }
            else
            {
                newTask.TaskPosition = newTask.TaskHomePosition = 10.0;
                newTask.Zone = 0;
            }

            _listViewModel.TaskList.Add(newTask);
        }
    }
}
