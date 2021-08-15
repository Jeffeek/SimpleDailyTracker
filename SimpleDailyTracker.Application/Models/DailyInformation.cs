using System.Collections.Generic;

namespace SimpleDailyTracker.Application.Models
{
    public record DailyInformation(IEnumerable<UserImportModel> UserInformation, int Day);
}