using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using AutoMapper;
using CsvHelper;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using SimpleDailyTracker.Application.Settings;
using SimpleDailyTracker.UI.Models;
using SimpleDailyTracker.UI.Views;

namespace SimpleDailyTracker.UI
{
    public partial class App : PrismApplicationBase
    {
        #region Overrides of PrismApplicationBase

        /// <inheritdoc />
        protected override IContainerExtension CreateContainerExtension() => new UnityContainerExtension();

        /// <inheritdoc />
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<Configuration>(() =>
                                                      {
                                                          using var stream =
                                                              new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "AppConfiguration.json"));

                                                          return JsonSerializer.Deserialize<Configuration>(stream.ReadToEnd());
                                                      });

            containerRegistry.Register<DirectorySettings>(provider => provider.Resolve<Configuration>()
                                                                              .DirectorySettings);

            containerRegistry.Register<FileSearch>(provider => provider.Resolve<Configuration>()
                                                                       .FileSearch);

            containerRegistry.RegisterSingleton<IMapper>(() => new MapperConfiguration(x => x.AddProfiles(GetAutoMapperProfilesFromAllAssemblies())).CreateMapper());

            CreateImportAndExportDirectories();
        }

        /// <inheritdoc />
        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        #endregion

        private void CreateImportAndExportDirectories()
        {
            var settings = Container.Resolve<DirectorySettings>();

            if (!Directory.Exists(settings.ExportDirectory))
                Directory.CreateDirectory(settings.ExportDirectory);

            if (!Directory.Exists(settings.ImportDirectory))
                Directory.CreateDirectory(settings.ImportDirectory);
        }

        private static IEnumerable<Profile> GetAutoMapperProfilesFromAllAssemblies() =>
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from aType in assembly.GetTypes()
            where aType.IsClass && aType.HasParameterlessConstructor() && !aType.IsAbstract && aType.IsSubclassOf(typeof(Profile))
            select (Profile)Activator.CreateInstance(aType);
    }
}