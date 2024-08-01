using System.IO.Compression;
using System.Text.Json;

namespace AutomaticArchiver
{
    internal class Program
    {
        public const string ConfigPath = ".\\config.cfg";

        public static Settings? Settings;



        static void Main(string[] args)
        {
            if(File.Exists(ConfigPath))
            {
                using(FileStream stream = new FileStream(ConfigPath, FileMode.Open))
                {
                    try
                    {
                        Settings = JsonSerializer.Deserialize<Settings>(stream);
                        Console.WriteLine("Файл конфигурации загружен");
                    }
                    catch(Exception exception)
                    {
                        ConsoleExtension.WriteError($"Ошибка при загрузке файла конфигурации \"config.cfg\"");
                        ConsoleExtension.WriteError($"Сообщение: {exception.Message}");
                        ConsoleExtension.WriteWarning($"Использована конфигурация по умолчанию");

                        Settings = Settings.Default;
                    }
                }
            }
            else
            {
                Settings = Settings.Default;

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                using(FileStream stream = new FileStream(ConfigPath, FileMode.CreateNew))
                    JsonSerializer.Serialize(stream, Settings, options);
            }


            if(Settings != null)
            {
                foreach(var task in Settings.ArchieveDirectoryTasks)
                    ArhieveDirectory(task);

                foreach(var task in Settings.ArchieveFileTasks)
                    ArhieveFile(task);
            }
        }



        private static void ArhieveDirectory(ArchieveDirectoryTask task)
        {
            if(task.SourceDirectory == null || task.ArchieveDirectory == null)
                return;

            task.SourceDirectory = task.SourceDirectory.Replace('/', '\\');
            if(task.SourceDirectory.EndsWith('\\'))
                task.SourceDirectory = task.SourceDirectory.Remove(task.SourceDirectory.Length - 1);

            string[] splittedPath = task.SourceDirectory.Split('\\');
            string name = splittedPath[splittedPath.Length - 1];

            string targetFile = task.ArchieveDirectory + name + '_' + DateTime.Now.Date.ToString("dd_MM_yyyy").Replace('/', '_') + ".zip";

            if(File.Exists(targetFile))
            {
                Console.WriteLine($"Файл {targetFile} уже существует. Задача архивации пропущена");
                return;
            }

            try
            {
                ZipFile.CreateFromDirectory(task.SourceDirectory, targetFile, CompressionLevel.SmallestSize, task.IncludeSourceDirectory);
                Console.WriteLine($"Папка {task.SourceDirectory} успешно помещена в {targetFile}");
            }
            catch (Exception exception)
            {
                ConsoleExtension.WriteError($"Ошибка архивирования: {exception.GetType().ToString()}");
                ConsoleExtension.WriteError($"\tСообщение: {exception.Message}");
            }
        }

        private static void ArhieveFile(ArchieveFileTask task)
        {
            if(task.SourceFile == null || task.ArchieveDirectory == null)
                return;

            task.SourceFile = task.SourceFile.Replace('/', '\\');
            if(task.SourceFile.EndsWith('\\'))
                ConsoleExtension.WriteError("Некорректный синтаксис пути к файлу");

            string[] splittedPath = task.SourceFile.Split('\\');
            string name = splittedPath[splittedPath.Length - 1];

            string targetFile = task.ArchieveDirectory + name + '_' + DateTime.Now.Date.ToString("dd_MM_yyyy").Replace('/', '_') + ".zip";

            if(File.Exists(targetFile))
            {
                Console.WriteLine($"Файл {targetFile} уже существует. Задача архивации пропущена");
                return;
            }

            try
            {
                using(var stream = new FileStream(targetFile, FileMode.OpenOrCreate))
                {
                    using(ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Create))
                    {
                        zip.CreateEntryFromFile(task.SourceFile, splittedPath[splittedPath.Length - 1]);
                    }
                }

                Console.WriteLine($"Файл {task.SourceFile} успешно помещён в {targetFile}");
            }
            catch(Exception exception)
            {
                ConsoleExtension.WriteError($"Ошибка архивирования: {exception.GetType().ToString()}");
                ConsoleExtension.WriteError($"\tСообщение: {exception.Message}");
            }
        }
    }
}
