using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HiNotes
{
    public class TaskListViewModel : INotifyPropertyChanged
    {
        private List<TaskModel> taskListModel;
        public ObservableCollection<TaskViewModel> TaskList { get; set; }
        private double _canvasHeight;
        public double CanvasHeight
        {
            get { return _canvasHeight; }
            set { _canvasHeight = value; OnPropertyChanged(nameof(CanvasHeight)); }
        }
        public string xmlPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\HiNotesTasks.xml";
        public TaskListViewModel()
        {
            taskListModel = TasksData.ReadFromXml(xmlPath);

            TaskList = new ObservableCollection<TaskViewModel>();
            int i = 0;
            foreach (TaskModel task in taskListModel)
            {
                TaskList.Add(new TaskViewModel { Name = task.Name, Note = task.Note, TaskPosition = i * 140, Zone = i });
                i++;
            }
        }
        public void SortByPosition()
        {
            //COPY OBSERVABLE COLLECTION TO NORMAL LIST
            List<TaskViewModel> temporaryList = new List<TaskViewModel>();
            foreach (TaskViewModel task in TaskList)
            {
                temporaryList.Add(task);
            }

            //SORT THAT LIST BY TASKPOSITION
            temporaryList.Sort(comparisonByPosition);

            //REPLACE OBSERVABLE COLLECTION WITH SORTED LIST
            TaskList.Clear();
            foreach (TaskViewModel task in temporaryList)
            {
                TaskList.Add(task);
            }

            //ARRANGE NEW POSITIONS FOR TASKS
            int counter = 0;
            foreach (TaskViewModel task in TaskList)
            {
                task.TaskPosition = task.TaskHomePosition = 10 + (counter * 140);
                task.Zone = counter;
                counter++;
            }
        }
        private Comparison<TaskViewModel> comparisonByPosition = new Comparison<TaskViewModel>
            (
                (TaskViewModel t1, TaskViewModel t2) => { return t1.TaskPosition.CompareTo(t2.TaskPosition); }
            );

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string paramName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
        }

        private ICommand commandAddNewTask;
        public ICommand AddNewTask
        {
            get
            {
                if (commandAddNewTask == null) commandAddNewTask = new CommandAddNewTask(this);
                return commandAddNewTask;
            }
        }

        private ICommand commandDeleteTask;
        public ICommand DeleteTask
        {
            get
            {
                if(commandDeleteTask == null) commandDeleteTask = new CommandDeleteTask(this);
                return commandDeleteTask;
            }
        }
    }
}