namespace AutomaticArchivation.Tasks
{
    [Serializable]
    public class ArchiveDirectoryTask : ArchiveTask
    {
        public bool IncludeSourceDirectory { get; set; }



        public ArchiveDirectoryTask() : base()
        {
            IncludeSourceDirectory = false;
        }



        public static ArchiveDirectoryTask Template
        {
            get
            {
                return new ArchiveDirectoryTask()
                {
                    SourceDirectory = "D:\\ImportantFiles",

                    IncludeSourceDirectory = true,

                    TargetDirectory = "D:\\Archives",
                    TargetName = "ImportantFolder"
                };
            }
        }
    }
}
