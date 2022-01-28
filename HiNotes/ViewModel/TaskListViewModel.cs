using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        string path = @"C:\Users\Mateusz\Desktop\FORMAT_1\repos\HiNotes\bin\Debug\net5.0-windows\zadania.xml";
        public TaskListViewModel()
        {
            taskListModel = TasksData.ReadFromXml(path);

            TaskList = new ObservableCollection<TaskViewModel>();
            int i = 0;
            foreach (TaskModel task in taskListModel)
            {
                TaskList.Add(new TaskViewModel { Name = task.Name, Note = task.Note, TaskPosition = i * 140, Zone = i }); //mozliwe ze nie trza tu ustawiac TaskPosition bo po starcie programu i tak wykonuje sie Border_SizeChanged i tam jeszcze raz sie ustalaja TaskPOsitions
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
            TaskList.Clear();  //moze jakos inczej sie da zeby nie czyscic tej listy bo to chyba troche slabe?
            foreach (TaskViewModel task in temporaryList)
            {
                TaskList.Add(task);
            }

            //ARRANGE NEW POSITIONS FOR TASKS
            int counter = 0;
            foreach (TaskViewModel task in TaskList)
            {
                task.TaskPosition = 10 + (counter * 140);
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
    }
}