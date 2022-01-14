using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNotes
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        private TaskModel _taskModel = new TaskModel();
        private double _taskPosition;
        public string Name
        {
            get { return _taskModel.Name; }
            set { _taskModel.Name = value; OnPropertyChanged(nameof(Name)); }
        }
        public string Note
        {
            get { return _taskModel.Note; }
            set { _taskModel.Note = value; OnPropertyChanged(nameof(Note));}
        }
        public double TaskPosition
        {
            get { return _taskPosition; }
            set { _taskPosition = value; OnPropertyChanged(nameof(TaskPosition)); }
        }
        public double TaskHomePosition { get; set; }
        public int Zone { get; set; }
        public double TaskBorderHeight { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string paramName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
        }
    }
}
