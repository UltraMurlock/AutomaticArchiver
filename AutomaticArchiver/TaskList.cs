namespace AutomaticArchiver
{
    public class TaskList
    {
        public ArchieveDirectoryTask[] ArchieveDirectoryTasks { get; set; } = new ArchieveDirectoryTask[0];
        public ArchieveFileTask[] ArchieveFileTasks { get; set; } = new ArchieveFileTask[0];



        public static TaskList Default
        {
            get
            {
                TaskList settings = new TaskList();
                settings.ArchieveDirectoryTasks = new ArchieveDirectoryTask[1];
                settings.ArchieveDirectoryTasks[0] = new ArchieveDirectoryTask();

                settings.ArchieveFileTasks = new ArchieveFileTask[1];
                settings.ArchieveFileTasks[0] = new ArchieveFileTask();
                return settings;
            }
        }
    }
}
