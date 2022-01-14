using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HiNotes
{
    public class TasksData
    {
        public static List<TaskModel> ReadFromXml(string path)
        {
            try
            {
                XDocument xDoc = XDocument.Load(path);

                IEnumerable<TaskModel> query = from task in xDoc.Descendants("Task")
                                               select new TaskModel
                                               {
                                                   Name = task.Element("Name").Value,
                                                   Note = task.Element("Note").Value
                                               };
                
                List<TaskModel> tasks = new List<TaskModel>();
                foreach (TaskModel task in query)
                {
                    tasks.Add(task);
                }
                return tasks;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
