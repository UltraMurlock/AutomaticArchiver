using System.IO.Compression;
using System.Text.Json;

namespace AutomaticArchiver
{
    internal class Program
    {
        public const string TaskListPath = ".\\tasklist.json";
        public const string AppDataPath = ".\\app-data.json";

        public static TaskList? TaskList;
        public static AppData? AppData;

        public static JsonSerializerOptions? AppDataSerializerOptions;



        static void Main(string[] args)
        {
            LoadAppData();

            if(File.Exists(TaskListPath))
            {
                using(FileStream stream = new FileStream(TaskListPath, FileMode.Open))
                {
                    try
                    {
                        TaskList = JsonSerializer.Deserialize<TaskList>(stream);
                        Console.WriteLine("Файл конфигурации загружен\n");
                    }
                    catch(Exception exception)
                    {
                        ConsoleExtension.WriteError($"Ошибка при загрузке файла конфигурации \"config.cfg\"" +
                            $"Сообщение: {exception.Message}" +
                            $"Будет использована конфигурация по умолчанию\n", true);

                        TaskList = TaskList.Default;
                    }
                }
            }
            else
            {
                TaskList = TaskList.Default;

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                using(FileStream stream = new FileStream(TaskListPath, FileMode.CreateNew))
                    JsonSerializer.Serialize(stream, TaskList, options);
            }


            if(TaskList != null)
            {
                bool cleanup = false;
                DateTime lastCleanMonth = AppData.LastCleaningUp.Date.AddDays(-AppData.LastCleaningUp.Date.Day + 1);
                if(lastCleanMonth != DateTime.Now.Date.AddDays(-DateTime.Now.Day + 1))
                    cleanup = true;

                foreach(var task in TaskList.ArchieveDirectoryTasks)
                    ArhieveDirectory(task, cleanup);

                foreach(var task in TaskList.ArchieveFileTasks)
                    ArhieveFile(task, cleanup);

                if(cleanup)
                {
                    AppData.LastCleaningUp = DateTime.Now.Date;
                    SaveAppData();
                }
            }
        }



        private static void LoadAppData()
        {
            AppDataSerializerOptions = new JsonSerializerOptions {
                WriteIndented = true
            };

            if(File.Exists(AppDataPath))
            {
                using(FileStream stream = new FileStream(AppDataPath, FileMode.Open))
                {
                    try
                    {
                        AppData = JsonSerializer.Deserialize<AppData>(stream);
                    }
                    catch(Exception exception)
                    {
                        ConsoleExtension.WriteError($"Ошибка при загрузке файла данных программы {AppDataPath}" +
                            $"\nСообщение: {exception.Message}" +
                            $"\nБудут использованы данные по умолчанию", true);

                        AppData = AppData.Default;
                    }
                }
            }
            else
            {
                AppData = AppData.Default;
                SaveAppData();
            }
        }

        private static void SaveAppData()
        {
            using(FileStream stream = new FileStream(AppDataPath, FileMode.Create))
                JsonSerializer.Serialize(stream, AppData, AppDataSerializerOptions);
        }



        private static void ArhieveDirectory(ArchieveDirectoryTask task, bool cleanup)
        {
            if(task.SourceDirectory == null || task.ArchieveDirectory == null)
                return;

            task.SourceDirectory = task.SourceDirectory.Replace('/', '\\');
            if(task.SourceDirectory.EndsWith('\\'))
                task.SourceDirectory = task.SourceDirectory.Remove(task.SourceDirectory.Length - 1);

            string[] splittedPath = task.SourceDirectory.Split('\\');
            string name = splittedPath[splittedPath.Length - 1];

            string targetFile = task.ArchieveDirectory + name + '_' + DateTime.Now.Date.ToString("dd_MM_yyyy").Replace('/', '_') + ".zip";

            if(task.IncludeSourceDirectory)
                Console.WriteLine($"Архивация {task.SourceDirectory} в {targetFile}");
            else
                Console.WriteLine($"Архивация из {task.SourceDirectory} в {targetFile}");

            if(File.Exists(targetFile))
            {
                Console.WriteLine($"Файл {targetFile} уже существует\nЗадача архивации пропущена");
            }
            else
            {
                try
                {
                    ZipFile.CreateFromDirectory(task.SourceDirectory, targetFile, CompressionLevel.SmallestSize, task.IncludeSourceDirectory);
                }
                catch(Exception exception)
                {
                    ConsoleExtension.WriteError($"Ошибка архивирования: {exception.GetType().ToString()}\n\tСообщение: {exception.Message}", true);
                    return;
                }
            }

            if(cleanup)
            {
                int deletedNumber = Cleaner.CleanUp(task.ArchieveDirectory, name);
                Console.WriteLine($"Очистка. Удалено файлов: {deletedNumber}");
            }

            Console.WriteLine();
        }

        private static void ArhieveFile(ArchieveFileTask task, bool cleanup)
        {
            if(task.SourceFile == null || task.ArchieveDirectory == null)
                return;

            task.SourceFile = task.SourceFile.Replace('/', '\\');
            if(task.SourceFile.EndsWith('\\'))
            {
                ConsoleExtension.WriteError($"Некорректный синтаксис пути к файлу \"{task.SourceFile}\"", true);
                return;
            }

            string[] splittedPath = task.SourceFile.Split('\\');
            string name = splittedPath[splittedPath.Length - 1];

            string targetFile = task.ArchieveDirectory + name + '_' + DateTime.Now.Date.ToString("dd_MM_yyyy").Replace('/', '_') + ".zip";
            Console.WriteLine($"Архивация {task.SourceFile} в {targetFile}");

            if(File.Exists(targetFile))
            {
                Console.WriteLine($"Файл {targetFile} уже существует\nЗадача архивации пропущена");
            }
            else
            {
                try
                {
                    using(var stream = new FileStream(targetFile, FileMode.OpenOrCreate))
                    {
                        using(ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Create))
                        {
                            zip.CreateEntryFromFile(task.SourceFile, splittedPath[splittedPath.Length - 1]);
                        }
                    }
                }
                catch(Exception exception)
                {
                    ConsoleExtension.WriteError($"Ошибка архивации: {exception.GetType().ToString()}\n\tСообщение: {exception.Message}\n", true);
                    return;
                }
            }

            if(cleanup)
            {
                int deletedNumber = Cleaner.CleanUp(task.ArchieveDirectory, name);
                Console.WriteLine($"Очистка. Удалено файлов: {deletedNumber}");
            }

            Console.WriteLine();
        }
    }
}
