using System.Collections.Generic;
using SimpleDailyTracker.Application.Enums;

namespace SimpleDailyTracker.UI.Models
{
    public class UserViewModel
    {
        public string Name { get; set; }

        public int AverageSteps { get; set; }

        public int BestDaySteps { get; set; }

        public int WorstDaySteps { get; set; }

        public DailyStatus Status { get; set; }

        public int Rank { get; set; }

        public bool IsOutline
        {
            get
            {
                var percentage = AverageSteps * 0.2d;

                return AverageSteps - WorstDaySteps > percentage && BestDaySteps - AverageSteps > percentage;
            }
        }

        public Dictionary<int, int> StepsByDay { get; set; } = new();
    }
}