using System;
using System.Xml.Serialization;

namespace SimpleDailyTracker.Application.Models
{
    [XmlRoot("User")]
    [Serializable]
    public class UserExportModel
    {
        public string Name { get; set; }

        public int AverageSteps { get; set; }

        public int BestDaySteps { get; set; }

        public int WorstDaySteps { get; set; }

        public string Status { get; set; }

        public int Rank { get; set; }
    }
}