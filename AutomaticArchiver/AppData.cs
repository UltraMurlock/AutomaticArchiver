namespace AutomaticArchiver
{
    public class AppData
    {
        public DateTime LastCleaningUp { get; set; }



        public static AppData Default
        {
            get
            {
                AppData appData = new AppData();
                appData.LastCleaningUp = DateTime.MinValue;
                return appData;
            }
        }
    }
}
