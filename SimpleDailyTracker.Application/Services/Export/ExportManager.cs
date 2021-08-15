using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using SimpleDailyTracker.Application.Enums;
using SimpleDailyTracker.Application.Models;
using SimpleDailyTracker.Application.Models.Maps;
using SimpleDailyTracker.Application.Settings;

namespace SimpleDailyTracker.Application.Services.Export
{
    public class ExportManager
    {
        private readonly DirectorySettings _directorySettings;
        private readonly IMapper _mapper;
        private readonly CsvConfiguration _internalCsvConfiguration;
        private readonly JsonSerializerOptions _internalJsonConfiguration;
        private readonly XmlSerializerNamespaces _internalXmlConfiguration;

        public ExportManager(DirectorySettings directorySettings,
                             IMapper mapper)
        {
            _directorySettings = directorySettings;
            _mapper = mapper;

            _internalCsvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
                                        {
                                            Delimiter = ";",
                                            Encoding = Encoding.UTF8,
                                            Mode = CsvMode.RFC4180,
                                            LeaveOpen = true
                                        };

            _internalJsonConfiguration = new JsonSerializerOptions()
                                         {
                                             AllowTrailingCommas = true,
                                             DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                                             DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                                             PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                             Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                             IgnoreNullValues = false,
                                             WriteIndented = true
                                         };

            _internalXmlConfiguration = new XmlSerializerNamespaces();
            _internalXmlConfiguration.Add(String.Empty, String.Empty);
        }

        private async Task ExportAsCsvAsync(UserExportModel user, TextWriter writer)
        {
            await using var csvWriter = new CsvWriter(writer, _internalCsvConfiguration);
            csvWriter.Context.RegisterClassMap<ExportUserModelMap>();
            await csvWriter.WriteRecordsAsync(new [] { user });
        }

        private Task ExportAsXmlAsync(UserExportModel user, TextWriter writer)
        {
            var serializer = new XmlSerializer(typeof(UserExportModel));

            return Task.Run(() => serializer.Serialize(writer, user, _internalXmlConfiguration));
        }

        private Task ExportAsJsonAsync(UserExportModel user, StreamWriter writer) =>
            JsonSerializer.SerializeAsync(writer.BaseStream, user, typeof(UserExportModel), _internalJsonConfiguration);

        public async Task ExportAsync(FullUserInformation user, ExportType type)
        {
            var fileName = String.Empty;
            var fileNameResult = false;

            while (!fileNameResult)
                try
                {
                    fileName = GetFilename(type, user.Name);
                    fileNameResult = true;
                }
                catch (FileLoadException)
                {
                    fileNameResult = false;
                    // write logger
                }

            var writer = GetWriter(fileName);

            try
            {
                var exportUser = _mapper.Map<UserExportModel>(user);

                switch (type)
                {
                    case ExportType.XML:
                        await ExportAsXmlAsync(exportUser, writer);

                        break;

                    case ExportType.CSV:
                        await ExportAsCsvAsync(exportUser, writer);

                        break;

                    case ExportType.JSON:
                        await ExportAsJsonAsync(exportUser, writer);

                        break;

                    default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
            catch (Exception)
            {
                // todo write logger
            }
            finally
            {
                if (writer.BaseStream.CanWrite)
                    await writer.FlushAsync();
            }
        }

        private StreamWriter GetWriter(string fileName) => new(new FileStream(fileName, FileMode.CreateNew));

        private string GetFilename(ExportType type, string userName)
        {
            var finalName = Path.Combine(_directorySettings.ExportDirectory,
                                         $"{userName}_{DateTime.UtcNow:yyyyMMdd_hhmmss}.{type.ToString().ToLowerInvariant()}");

            if (!File.Exists(finalName))
                return finalName;

            throw new FileLoadException($"File with name {finalName} already exists");
        }
    }
}