namespace AutomaticArchivation.Tasks
{
    [Serializable]
    public abstract class ArchiveTask
    {
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public string TargetName { get; set; }



        public ArchiveTask()
        {
            SourceDirectory = string.Empty;
            TargetDirectory = string.Empty;
            TargetName = string.Empty;
        }
    }
}
