namespace AutomaticArchivation.Tasks
{
    [Serializable]
    public class ArchiveFileTask : ArchiveTask
    {
        public string SourceFileName { get; set; }



        public ArchiveFileTask() : base()
        {
            SourceFileName = string.Empty;
        }



        public static ArchiveFileTask Template
        {
            get
            {
                return new ArchiveFileTask()
                {
                    SourceDirectory = "D:\\ImportantFiles",
                    SourceFileName = "ImportantFile.txt",

                    TargetDirectory = "D:\\Archives",
                    TargetName = "ImportantFile"
                };
            }
        }
    }
}
