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
                foreach(var task in Settings.ArchieveTasks)
                    ArhieveDirectory(task);
            }
        }



        private static void ArhieveDirectory(ArchieveTask task)
        {
            if(task.SourceDirectory == null || task.ArchieveDirectory == null)
                return;

            task.SourceDirectory = task.SourceDirectory.Replace('/', '\\');
            string[] splittedPath = task.SourceDirectory.Split('\\');
            string name = splittedPath[splittedPath.Length - 2];

            string targetFile = task.ArchieveDirectory + name + '_' + DateTime.Now.Date.ToString("dd_MM_yyyy").Replace('/', '_') + ".zip";

            if(File.Exists(targetFile))
            {
                Console.WriteLine($"Файл {targetFile} уже существует. Задача архивации пропущена");
                return;
            }

            try
            {
                ZipFile.CreateFromDirectory(task.SourceDirectory, targetFile, CompressionLevel.SmallestSize, false);
                Console.WriteLine($"Папка {task.SourceDirectory} успешно помещена в {targetFile}");
            }
            catch (Exception exception)
            {
                ConsoleExtension.WriteError($"Ошибка архивирования: {exception.GetType().ToString()}");
                ConsoleExtension.WriteError($"\tСообщение: {exception.Message}");
            }
        }
    }
}
