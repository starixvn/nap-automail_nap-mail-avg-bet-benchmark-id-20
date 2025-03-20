using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.TOAVGBetBenchmark.Business.Models;
using Fanex.TOAVGBetBenchmark.Business.Services;
using Fanex.TOAVGBetBenchmark.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.IO;
using LetstalkConfig = Fanex.LetstalkService.Client.AppConfigService;
namespace Fanex.TOAVGBetBenchmark.Service
{
    using AppConfigService = Business.Services.AppConfigService;

    public static class ServiceCollectionExtensions
    {
        public static void AddConfig(this IServiceCollection services)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("appsettings.json");
            AppConfigService.Config = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
            LetstalkConfig.Config.LetstalkServiceApi = AppConfigService.Config.LetstalkServiceApi;
        }

        public static void AddInternalServices(this IServiceCollection services)
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