namespace AutomaticArchiver
{
    public static class Cleaner
    {
        public static void CleanUp(string directoryPath, string baseFileName)
        {
            string[] files = Directory.GetFiles(directoryPath, $"{baseFileName}_*_*_*.zip");

            Dictionary<DateTime, List<DateTime>> filesInMonth = new Dictionary<DateTime, List<DateTime>>();
            Dictionary<DateTime, string> filesByDate = new Dictionary<DateTime, string>();
            foreach(string file in files)
            {
                DateTime date = GetDateFromName(file);
                DateTime month = date;
                month = month.AddDays(-month.Day + 1);

                if(!filesInMonth.ContainsKey(month))
                    filesInMonth.Add(month, new List<DateTime>());

                filesInMonth[month].Add(date);
                filesByDate.Add(date, file);
            }

            foreach(DateTime month in filesInMonth.Keys)
            {
                if(month == DateTime.Now.Date.AddDays(-DateTime.Now.Day + 1))
                    continue;
                if(filesInMonth[month].Count <= 1)
                    continue;

                DateTime lastDate = DateTime.MinValue;
                foreach(DateTime date in filesInMonth[month])
                {
                    if(date > lastDate)
                        lastDate = date;
                }

                for(int i = filesInMonth[month].Count - 1; i >= 0; i--)
                {
                    DateTime date = filesInMonth[month][i];
                    if(date != lastDate)
                    {
                        string fileToDelete = filesByDate[date];
                        File.Delete(fileToDelete);

                        filesByDate.Remove(date);
                    }
                }
            }
        }

        

        private static DateTime GetDateFromName(string path)
        {
            int lastDot = path.LastIndexOf('.');
            path = path.Remove(lastDot);

            string[] splittedPath = path.Split('_');
            int year = int.Parse(splittedPath[splittedPath.Length - 1]);
            int month = int.Parse(splittedPath[splittedPath.Length - 2]);
            int day = int.Parse(splittedPath[splittedPath.Length - 3]);

            return new DateTime(year, month, day);
        }
    }
}
