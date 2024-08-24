using System.Text.Json;
using AutomaticArchivation;
using AutomaticArchivation.Tasks;
using AutomaticArchiver.Logging;

namespace AutomaticArchiver
{
    internal class Program
    {
        public const string TaskListPath = ".\\tasklist.json";
        public const string TemplateTaskListPath = ".\\tasklist-template.json";

        public const string LogPath = ".\\log.txt";

        public static JsonSerializerOptions? SerializerOptions;
        public static TaskList? TaskList;

        public static ILogger Logger = new ConsoleLogger();



        static void Main(string[] args)
        {
            Logger = new FileLogger(LogPath);

            SerializerOptions = new JsonSerializerOptions {
                WriteIndented = true
            };

            if(!File.Exists(TemplateTaskListPath))
            {
                using(FileStream stream = new FileStream(TemplateTaskListPath, FileMode.Create, FileAccess.Write))
                    JsonSerializer.Serialize(stream, TaskList.Template, SerializerOptions);
            }

            TaskList = LoadTaskList(TaskListPath);



            foreach(var task in TaskList.GetTasks())
                PerformTask(task);

            Logger.LogMessage("Завершение программы");
            (Logger as IDisposable)?.Dispose();
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
                    string errorText = $"Ошибка при загрузке файла с задачами {Path.GetFullPath(path)}\n";
                    Logger.LogError(errorText, ex);
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
            Logger.LogMessage($"Архивация {task.TargetName} в {task.TargetDirectory}...");
            try
            {
                Archiver.Archive(task);
            }
            catch(Exception ex)
            {
                string source = task.SourceDirectory;
                if(task is ArchiveFileTask fileTask)
                    source += '\\' + fileTask.SourceFileName;

                string errorMessage = $"Ошибка архивации {source}\n";
                Logger.LogError(errorMessage, ex, "\n");
            }

            Logger.LogMessage($"Очистка старых архивов...");
            DirectoryInfo directory = new DirectoryInfo(task.TargetDirectory);
            Cleaner.CleanUp(directory, task.TargetName);

            Logger.LogMessage($"Успешно\n");
        }
    }
}
