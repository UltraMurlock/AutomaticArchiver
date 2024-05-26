namespace AutomaticArchiver
{
    public class Settings
    {
        public ArchieveTask[] ArchieveTasks { get; set; } = new ArchieveTask[0];



        public static Settings Default
        {
            get
            {
                Settings settings = new Settings();
                settings.ArchieveTasks = new ArchieveTask[1];
                settings.ArchieveTasks[0] = new ArchieveTask();
                return settings;
            }
        }
    }
}
