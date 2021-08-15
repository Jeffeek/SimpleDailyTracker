using System.Collections.Generic;
using SimpleDailyTracker.Application.Enums;

namespace SimpleDailyTracker.Application.Models
{
    public class FullUserInformation
    {
        public string Name { get; set; }

        public int AverageSteps { get; set; }

        public int BestDaySteps { get; set; }

        public int WorstDaySteps { get; set; }

        public DailyStatus Status { get; set; }

        public int Rank { get; set; }

        public Dictionary<int, int> StepsByDay { get; set; } = new();
    }
}