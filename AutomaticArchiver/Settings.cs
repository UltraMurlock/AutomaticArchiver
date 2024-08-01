namespace AutomaticArchiver
{
    public class Settings
    {
        public ArchieveDirectoryTask[] ArchieveDirectoryTasks { get; set; } = new ArchieveDirectoryTask[0];
        public ArchieveFileTask[] ArchieveFileTasks { get; set; } = new ArchieveFileTask[0];



        public static Settings Default
        {
            get
            {
                Settings settings = new Settings();
                settings.ArchieveDirectoryTasks = new ArchieveDirectoryTask[1];
                settings.ArchieveDirectoryTasks[0] = new ArchieveDirectoryTask();

                settings.ArchieveFileTasks = new ArchieveFileTask[1];
                settings.ArchieveFileTasks[0] = new ArchieveFileTask();
                return settings;
            }
        }
    }
}
