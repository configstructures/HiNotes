using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HiNotes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonCloseApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool taskMoving = false;
        private Point positionInGrid;
        private TaskViewModel bindingTask;
        private int movingTaskIdx; //jaka bedzie maksymalna liczba taskow?
        private int currentZone;   //jaka bedzie maksymalna liczba taskow?
        double last_position = 0;
        int zoneLimitMovingDown;
        int zoneLimitMovingUp;
        private void TaskMoveHook_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid g = sender as Grid;
            Grid gg = g.Parent as Grid;
            if(gg != null)
            {
                g.CaptureMouse();
                TextBox tb = gg.Children.OfType<TextBox>().FirstOrDefault();
                taskMoving = true;
                positionInGrid = e.GetPosition(g);
                BindingExpression bindingExpression = tb.GetBindingExpression(TextBox.TextProperty);
                bindingTask = bindingExpression.DataItem as TaskViewModel;
                movingTaskIdx = taskListViewModel.TaskList.IndexOf(bindingTask);

                if (movingTaskIdx == 0)
                {
                    zoneLimitMovingDown = (int)taskListViewModel.TaskList[movingTaskIdx + 1].TaskPosition;
                    zoneLimitMovingUp = 0;
                }
                else if (movingTaskIdx == taskListViewModel.TaskList.Count - 1)
                {
                    zoneLimitMovingDown = Int32.MaxValue;
                    zoneLimitMovingUp = (int)taskListViewModel.TaskList[movingTaskIdx - 1].TaskPosition;
                }
                else
                {
                    zoneLimitMovingDown = (int)taskListViewModel.TaskList[movingTaskIdx + 1].TaskPosition;
                    zoneLimitMovingUp = (int)taskListViewModel.TaskList[movingTaskIdx - 1].TaskPosition;
                }
                currentZone = taskListViewModel.TaskList[movingTaskIdx].Zone;

                last_position = bindingTask.TaskPosition;
            }
        }
        private void TaskMoveHook_MouseMove(object sender, MouseEventArgs e)
        {
            if(taskMoving)
            {
                //MOVE GRABBED TASK WHILE MOVING
                Point positionInWindow = e.GetPosition(null);
                bindingTask.TaskPosition = positionInWindow.Y - positionInGrid.Y - gridMain.RowDefinitions[0].Height.Value + scrollViewerTasks.VerticalOffset;

                //SWITCH REST TASKS
                if (bindingTask.TaskPosition > last_position) //IF MOVING DOWN
                {
                    if (currentZone < taskListViewModel.TaskList.Count - 1) //IF MOVING TASK IS NOT IN LAST ZONE
                    {
                        //CHECK WHICH TASK HAS ZONE THAT MOVING TASK WILL CAME IN
                        int switchTaskIdx = taskListViewModel.TaskList.IndexOf(taskListViewModel.TaskList.Where(x => x.Zone == (currentZone + 1)).FirstOrDefault());

                        //ASSIGN NEW ZONE LIMITS FOR MOVING TASK
                        zoneLimitMovingDown = (int)taskListViewModel.TaskList[switchTaskIdx].TaskPosition;

                        if (bindingTask.TaskPosition > zoneLimitMovingDown)
                        {
                            currentZone++;
                            //SWITCH TASK THAT MOVING TASK CAME IN TO UPPER POSITION
                            taskListViewModel.TaskList[switchTaskIdx].TaskPosition = SetNewTaskPosition(taskListViewModel.TaskList[switchTaskIdx].Zone - 1);
                            taskListViewModel.TaskList[switchTaskIdx].TaskHomePosition = taskListViewModel.TaskList[switchTaskIdx].TaskPosition;
                            taskListViewModel.TaskList[switchTaskIdx].Zone = currentZone - 1;
                        }
                    }
                }
                else if (bindingTask.TaskPosition < last_position) //IF MOVING UP
                {
                    if (currentZone > 0) //IF MOVING TASK IS NOT IN FIRST ZONE
                    {
                        //CHECK WHICH TASK HAS ZONE THAT MOVING TASK WILL CAME IN
                        int switchTaskIdx = taskListViewModel.TaskList.IndexOf(taskListViewModel.TaskList.Where(x => x.Zone == (currentZone - 1)).FirstOrDefault());

                        //ASSIGN NEW ZONE LIMITS FOR MOVING TASK
                        zoneLimitMovingUp = (int)taskListViewModel.TaskList[switchTaskIdx].TaskPosition;

                        if (bindingTask.TaskPosition < zoneLimitMovingUp)
                        {
                            currentZone--;
                            //SWITCH TASK THAT MOVING TASK CAME IN TO UPPER POSITION
                            taskListViewModel.TaskList[switchTaskIdx].TaskPosition = SetNewTaskPosition(taskListViewModel.TaskList[switchTaskIdx].Zone + 1);
                            taskListViewModel.TaskList[switchTaskIdx].TaskHomePosition = taskListViewModel.TaskList[switchTaskIdx].TaskPosition;
                            taskListViewModel.TaskList[switchTaskIdx].Zone = currentZone + 1;
                        }
                    }
                }
                bindingTask.Zone = currentZone;
                last_position = bindingTask.TaskPosition;
            }
        }

        private void TaskMoveHook_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Grid g = sender as Grid;
            if(g != null)
            {
                g.ReleaseMouseCapture();
                taskMoving = false;
                taskListViewModel.SortByPosition();
            }
        }
        private void TaskBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //PYTANIE SO: czy to dobra metoda na ogarnianie wysokości tasku
            //d_1.Text = "stara wys.: " + e.PreviousSize.Height.ToString() +
            //           " nowa: " + e.NewSize.Height.ToString() +
            //           "\nstara szer.: " + e.PreviousSize.Width.ToString() +
            //           " nowa: " + e.NewSize.Width.ToString();

            Border b = sender as Border;
            Grid g = b.Child as Grid;
            Grid gg = g.Children[0] as Grid;
            TextBox tb = gg.Children.OfType<TextBox>().FirstOrDefault();
            BindingExpression bindingExpression = tb.GetBindingExpression(TextBox.TextProperty);
            TaskViewModel borderTask = bindingExpression.DataItem as TaskViewModel;
            borderTask.TaskBorderHeight = e.NewSize.Height;

            UpdateTasksPositions(taskListViewModel.TaskList.IndexOf(borderTask));

            TaskListViewModel dc = DataContext as TaskListViewModel;
            dc.CanvasHeight = taskListViewModel.TaskList[taskListViewModel.TaskList.Count - 1].TaskPosition +
                              taskListViewModel.TaskList[taskListViewModel.TaskList.Count - 1].TaskBorderHeight;
        }
        private void UpdateTasksPositions(int taskIdx)
        {
            for (int i = taskIdx; i < taskListViewModel.TaskList.Count; i++)
            {
                if (i == 0)
                {
                    taskListViewModel.TaskList[i].TaskPosition = 10;
                }
                else
                {
                    taskListViewModel.TaskList[i].TaskPosition = taskListViewModel.TaskList[i - 1].TaskPosition +
                                                                 taskListViewModel.TaskList[i - 1].TaskBorderHeight + 10; //30 to odleglosc miedzy taskmi
                }
                taskListViewModel.TaskList[i].TaskHomePosition = taskListViewModel.TaskList[i].TaskPosition;
            }
        }
        private double SetNewTaskPosition(int zone)
        {
            double newTaskPosition;
            int reqZoneTaskIdx = taskListViewModel.TaskList.IndexOf(taskListViewModel.TaskList.Where(x => x.Zone == zone - 1).FirstOrDefault());

            if (zone == 0)
            {
                newTaskPosition = 10; //zmienna globalna
            }
            else
            {
                newTaskPosition = taskListViewModel.TaskList[reqZoneTaskIdx].TaskHomePosition + taskListViewModel.TaskList[reqZoneTaskIdx].TaskBorderHeight + 10;
            }
            
            return newTaskPosition;
        }
    }
}