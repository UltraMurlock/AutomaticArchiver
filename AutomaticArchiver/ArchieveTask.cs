namespace AutomaticArchiver
{
    [Serializable]
    public struct ArchieveTask
    {
        public string SourceDirectory { get; set; }
        public string ArchieveDirectory { get; set; }
    }
}
