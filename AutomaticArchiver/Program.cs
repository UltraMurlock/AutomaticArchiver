using System.Text.Json;
using AutomaticArchiver.Archives;
using AutomaticArchiver.Tasks;

namespace AutomaticArchiver
{
    internal class Program
    {
        public const string TaskListPath = ".\\tasklist.json";
        public const string TemplateTaskListPath = ".\\tasklist-template.json";

        public static TaskList? TaskList;

        public static JsonSerializerOptions? SerializerOptions;



        static void Main(string[] args)
        {
            SerializerOptions = new JsonSerializerOptions {
                WriteIndented = true
            };

            using(FileStream stream = new FileStream(TemplateTaskListPath, FileMode.Create, FileAccess.Write))
                JsonSerializer.Serialize(stream, TaskList.Template, SerializerOptions);

            TaskList = LoadTaskList(TaskListPath);



            foreach(var task in TaskList.GetTasks())
                PerformTask(task);
        }



        private static TaskList LoadTaskList(string path)
        {
            if(File.Exists(TaskListPath))
            {
                try
                {
                    TaskList? taskList = null;
                    using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        taskList = JsonSerializer.Deserialize<TaskList>(stream);

                    if(taskList == null)
                        throw new JsonException("Deserialized file is null");

                    return taskList;
                }
                catch(Exception ex)
                {
                    string errorText = $"Ошибка при загрузке файла с задачами {Path.GetFullPath(path)}";
                    ConsoleExtension.WriteException(errorText, ex, true);
                    return new TaskList();
                }
            }
            else
            {
                TaskList taskList = new TaskList();
                using(FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    JsonSerializer.Serialize(stream, taskList, SerializerOptions);
                return taskList;
            }
        }

        private static void PerformTask(ArchiveTask task)
        {
            try
            {
                Archiver.Archive(task);
            }
            catch(Exception ex)
            {
                string source = task.SourceDirectory;
                if(task is ArchiveFileTask fileTask)
                    source += '\\' + fileTask.SourceFileName;

                string errorMessage = $"Ошибка при архивации {source}";
                ConsoleExtension.WriteException(errorMessage, ex, true);
            }
        }
    }
}
