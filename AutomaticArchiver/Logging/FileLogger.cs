namespace AutomaticArchiver.Logging
{
    public class FileLogger : ILogger, IDisposable
    {
        private TextWriter _writer;


        public FileLogger(string logPath)
        {
            bool addSpace = File.Exists(logPath);
            _writer = new StreamWriter(logPath, true);

            if(addSpace)
                _writer.Write("\n\n\n");

            LogHeader();
        }



        public void LogMessage(string text)
        {
            _writer.WriteLine(text);
        }

        public void LogWarning(string text)
        {
            _writer.WriteLine("WARNING:\n" + text);
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
            _writer.WriteLine("ERROR:\n" + text);
        }



        private void LogHeader()
        {
            _writer.WriteLine($"Запуск {DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss")}");
        }



        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
