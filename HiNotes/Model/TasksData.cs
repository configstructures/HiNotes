using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace HiNotes
{
    public class TasksData
    {
        public static List<TaskModel> ReadFromXml(string path)
        {
            List<TaskModel> tasks;
            try
            {
                if (File.Exists(path) && new FileInfo(path).Length > 0)
                {
                    XDocument xDoc = XDocument.Load(path);

                    IEnumerable<TaskModel> query = from task in xDoc.Descendants("Task")
                                                   select new TaskModel
                                                   {
                                                       Name = task.Element("Name").Value,
                                                       Note = task.Element("Note").Value
                                                   };

                    tasks = new List<TaskModel>();
                    foreach (TaskModel task in query)
                    {
                        tasks.Add(task);
                    }
                }
                else
                {
                    tasks = new List<TaskModel>();
                }

                return tasks;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Cannot read Xml file.");
            }
        }
        public static void SaveToXml(string path, List<TaskModel> tasks)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.WriteAllText(path, string.Empty);
                }
                XDocument xDoc = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement("Tasks", from TaskModel task in tasks
                                              select new XElement("Task", new XElement("Name", task.Name),
                                                                          new XElement("Note", task.Note))
                                              ));
                xDoc.Save(path);
            }
            catch (System.Exception)
            {
                throw new System.Exception("Cannot save Xaml file.");
            }
        }
    }
}
