namespace AutomaticArchiver
{
    public static class ConsoleExtension
    {
        public static void WriteException(string text, Exception exception, bool waitUserBeforeContinue)
        {
            string errorMessage = $"{text}\n\t{exception.GetType().Name}:{exception.Message}";
            WriteError(errorMessage, true);
        }

        public static void WriteError(string text, bool waitUserBeforeContinue)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;

            if(waitUserBeforeContinue)
            {
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.WriteLine();
            }
        }

        public static void WriteWarning(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
