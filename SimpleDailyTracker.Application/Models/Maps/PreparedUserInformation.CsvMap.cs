using CsvHelper.Configuration;

namespace SimpleDailyTracker.Application.Models.Maps
{
    public class ExportUserModelMap : ClassMap<UserExportModel>
    {
        public ExportUserModelMap()
        {
            Map(p => p.Name)
                .Name("Name");

            Map(p => p.AverageSteps)
                .Name("AverageSteps");

            Map(p => p.BestDaySteps)
                .Name("BestDaySteps");

            Map(p => p.WorstDaySteps)
                .Name("WorstDaySteps");

            Map(p => p.Rank)
                .Name("Rank");

            Map(p => p.Status)
                .Name("Status");
        }
    }
}