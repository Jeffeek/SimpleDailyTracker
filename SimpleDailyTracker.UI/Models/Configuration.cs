using SimpleDailyTracker.Application.Settings;

namespace SimpleDailyTracker.UI.Models
{
    public class Configuration
    {
        public DirectorySettings DirectorySettings { get; set; }

        public FileSearch FileSearch { get; set; }
    }
}