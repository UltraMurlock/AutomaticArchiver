namespace AutomaticArchiver
{
    [Serializable]
    public struct ArchieveDirectoryTask
    {
        public string SourceDirectory { get; set; }
        public bool IncludeSourceDirectory { get; set; }
        public string ArchieveDirectory { get; set; }
    }
}
