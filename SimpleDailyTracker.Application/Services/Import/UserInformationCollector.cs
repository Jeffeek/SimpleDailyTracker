using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SimpleDailyTracker.Application.Models;
using IMapper = AutoMapper.IMapper;

namespace SimpleDailyTracker.Application.Services.Import
{
    public class UserInformationCollector
    {
        private readonly DailyTrackerParser _parser;
        private readonly IMapper _mapper;

        public UserInformationCollector(DailyTrackerParser parser,
                                        IMapper mapper)
        {
            _parser = parser;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FullUserInformation>> CollectDataAsync(IEnumerable<string> fileNames, CancellationToken cancellationToken = default)
        {
            var dailyInformation = await _parser.GetDailyInformationAsync(fileNames, cancellationToken);

            var result = new Dictionary<string, FullUserInformation>();

            foreach (var (information, day) in dailyInformation)
            {
                foreach (var importModel in information)
                {
                    result.TryGetValue(importModel.User, out var user);

                    if (user != null)
                    {
                        user.StepsByDay.Add(day, importModel.Steps);

                        continue;
                    }

                    result.Add(importModel.User,
                               _mapper.Map<FullUserInformation>(importModel,
                                                                options => options.AfterMap((_, userInformation) =>
                                                                                                userInformation.StepsByDay.TryAdd(day, importModel.Steps))));
                }
            }

            foreach (var user in result.Values)
                FillStatistics(user);

            return result
                   .Select(x => x.Value)
                   .OrderByDescending(x => x.AverageSteps);
        }

        private void FillStatistics(FullUserInformation user)
        {
            user.AverageSteps = user.StepsByDay.Sum(x => x.Value) / user.StepsByDay.Count;
            user.BestDaySteps = user.StepsByDay.Max(x => x.Value);
            user.WorstDaySteps = user.StepsByDay.Min(x => x.Value);
        }
    }
}