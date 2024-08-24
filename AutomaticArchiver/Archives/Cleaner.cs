namespace AutomaticArchiver.Archives
{
    public static class Cleaner
    {
        public static void CleanUp(DirectoryInfo targetDirectory, string targetName, int dailyArchives = 10)
        {
            FileInfo[] files = targetDirectory.GetFiles($"{targetName}_*_*_*.zip");

            SortedList<DateTime, FileInfo> fileByDate = new SortedList<DateTime, FileInfo>();
            foreach(FileInfo file in files)
            {
                DateTime date = GetDateFromName(file);
                fileByDate.Add(date, file);
            }

            List<DateTime> existsMonthBackup = new List<DateTime>();
            List<FileInfo> filesToDelete = new List<FileInfo>();
            for (int i = fileByDate.Count - 1; i >= 0; i--)
            {
                DateTime date = fileByDate.GetKeyAtIndex(i);

                if(DateTime.Today.Subtract(date) < TimeSpan.FromDays(dailyArchives))
                    continue;

                DateTime month = DateToYearMonth(date);
                if(existsMonthBackup.Contains(month))
                    filesToDelete.Add(fileByDate[date]);
                else
                    existsMonthBackup.Add(month);
            }

            for(int i = 0; i < filesToDelete.Count; i++)
                filesToDelete[i].Delete();
        }



        private static DateTime DateToYearMonth(DateTime date)
        {
            return date.Date.AddDays(-date.Day + 1);
        }

        private static DateTime GetDateFromName(FileInfo file)
        {
            string name = Path.GetFileNameWithoutExtension(file.Name);

            string[] splittedPath = name.Split('_');
            int year = int.Parse(splittedPath[splittedPath.Length - 1]);
            int month = int.Parse(splittedPath[splittedPath.Length - 2]);
            int day = int.Parse(splittedPath[splittedPath.Length - 3]);

            return new DateTime(year, month, day);
        }
    }
}
