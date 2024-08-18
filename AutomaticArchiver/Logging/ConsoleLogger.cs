namespace AutomaticArchiver.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void LogMessage(string text)
        {
            Console.WriteLine(text);
        }

        public void LogWarning(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void LogError(string textBefore, Exception exception)
        {
            LogError(textBefore, exception, string.Empty);
        }

        public void LogError(string textBefore, Exception exception, string textAfter)
        {
            string errorMessage = $"{textBefore}\t{exception.GetType().Name}:{exception.Message}{textAfter}";
            LogError(errorMessage);
        }

        public void LogError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
