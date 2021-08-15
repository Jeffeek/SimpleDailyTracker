using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SimpleDailyTracker.Application.Models;
using SimpleDailyTracker.Application.Settings;

namespace SimpleDailyTracker.Application.Services.Import
{
    public class DailyTrackerParser
    {
        private readonly FileSearch _fileSearch;
        private readonly IMapper _mapper;

        public DailyTrackerParser(FileSearch fileSearch,
                                  IMapper mapper)
        {
            _fileSearch = fileSearch;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DailyInformation>> GetDailyInformationAsync(IEnumerable<string> fileNames, CancellationToken cancellationToken = default)
        {
            var resultList = new List<DailyInformation>();

            foreach (var fileName in fileNames)
            {
                var stream = new FileStream(fileName, FileMode.Open);

                try
                {
                    var result = await JsonSerializer.DeserializeAsync<IEnumerable<UserImportModel>>(stream, null, cancellationToken);

                    resultList.Add(new DailyInformation(result,
                                                        GetDayOfFile(fileName)));
                }
                catch (ArgumentException e)
                {
                    // todo write logger for bad file
                }
                catch (Exception e)
                {
                    //todo write logger for unknown error
                }
                finally
                {
                    await stream.DisposeAsync();
                }
            }

            return resultList;
        }

        private int GetDayOfFile(string fileName)
        {
            if (Int32.TryParse(Regex.Match(fileName, _fileSearch.DayFinderRegex)
                                    .Groups[1]
                                    .Value,
                               out var result))
                return result;

            throw new ArgumentException($"Name of import file must match the pattern: {_fileSearch.DayFinderRegex}");
        }
    }
}