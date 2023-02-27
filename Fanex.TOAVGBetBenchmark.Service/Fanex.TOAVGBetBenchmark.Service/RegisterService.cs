using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.TOAVGBetBenchmark.Business.Models;
using Fanex.TOAVGBetBenchmark.Business.Services;
using Fanex.TOAVGBetBenchmark.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OfficeOpenXml;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LetstalkConfig = Fanex.LetstalkService.Core.Services.AppConfigService;

namespace Fanex.TOAVGBetBenchmark.Service
{
    public static class RegisterService
    {
        public static void RegisterServices(IConfiguration configuration, IServiceCollection services)
        {
            LoadConfig();
            SentryRegister();
            ConfigureFanexData(configuration);
            DependencyInjection(services);
        }

        private static void LoadConfig()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("appsettings.json");
            AppConfigService.Config = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
            LetstalkConfig.Config.LetstalkServiceApi = AppConfigService.Config.LetstalkServiceApi;
        }

        private static void SentryRegister()
        {
            var sentry = new SentryLogging(new SentryEngineOptions
            {
                Dsn = new Dsn(AppConfigService.Config.SentryClientKey)
            });

            LogManager
               .SetDefaultLogCategory(AppConfigService.Config.CategorySentryLog)
               .Use(new List<ILogEngine>
               {
                    sentry
               });
        }

        private static void ConfigureFanexData(IConfiguration Configuration)
        {
            var connections = Configuration.GetSection("ConnectionStrings")
                .GetChildren()
                .AsEnumerable()
                .ToDictionary
                (
                     x => x.Key,
                     y => y.Value
                );

            DbSettingProviderManager
             .StartNewSession()
             .UseConnectionStrings(connections)
             .UseDefaultDbSettingProvider(Configuration.GetSection("StoredProcedurePath").Value)
             .Run();
        }

        private static void DependencyInjection(IServiceCollection services)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            services.AddSingleton<Log, LogImp>();
            services.AddSingleton<MessageService, MessageServiceImp>();
            services.AddSingleton<MailService, MailServiceImp>();
            services.AddSingleton<LetstalkClientService, LetstalkClientServiceImp>();
            services.AddSingleton<DataService, DataServiceImp>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<ImageService, ImageServiceImp>();
            services.AddSingleton<FileService, FileServiceImp>();
            services.AddSingleton<DateRangeService, DateRangeServiceImp>();
            services.AddSingleton<ExcelService, ExcelServiceImp>();
            services.AddSingleton<MainService, MainServiceImp>();
        }
    }
}