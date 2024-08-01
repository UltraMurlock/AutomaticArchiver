namespace AutomaticArchiver
{
    [Serializable]
    public struct ArchieveFileTask
    {
        public string SourceFile { get; set; }
        public string ArchieveDirectory { get; set; }
    }
}
