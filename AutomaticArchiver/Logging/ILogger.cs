namespace AutomaticArchiver.Logging
{
    public interface ILogger
    {
        public void LogMessage(string text);
        public void LogWarning(string text);
        public void LogError(string textBefore, Exception exception);
        public void LogError(string textBefore, Exception exception, string textAfter);
        public void LogError(string text);
    }
}
