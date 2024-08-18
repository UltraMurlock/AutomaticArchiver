using AutomaticArchiver.Tasks;

namespace AutomaticArchiver
{
    [Serializable]
    public class TaskList
    {
        public ArchiveDirectoryTask[] DirectoryTasks { get; set; }
        public ArchiveFileTask[] FileTasks { get; set; }



        public TaskList()
        {
            DirectoryTasks = new ArchiveDirectoryTask[0];
            FileTasks = new ArchiveFileTask[0];
        }



        public static TaskList Template
        {
            get
            {
                TaskList template = new TaskList();
                template.DirectoryTasks = new ArchiveDirectoryTask[1] { ArchiveDirectoryTask.Template };
                template.FileTasks = new ArchiveFileTask[1] { ArchiveFileTask.Template };
                return template;
            }
        }



        public ArchiveTask[] GetTasks()
        {
            ArchiveTask[] tasks = new ArchiveTask[DirectoryTasks.Length + FileTasks.Length];
            DirectoryTasks.CopyTo(tasks, 0);
            FileTasks.CopyTo(tasks, DirectoryTasks.Length);
            return tasks;
        }
    }
}
