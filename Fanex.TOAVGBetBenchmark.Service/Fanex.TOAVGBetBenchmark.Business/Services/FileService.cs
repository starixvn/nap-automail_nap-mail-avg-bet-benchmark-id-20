using System;
using System.Collections.Generic;
using System.IO;
using Fanex.TOAVGBetBenchmark.Business.Model;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface FileService
    {
        void CopyFiles(List<string> fileNames);
    }

    public class FileServiceImp(DataService dataService) : FileService
    {
        private void LogOneTime(DateTime date)
        {
            var fileInfo = new FileInfo(AppConfigService.Config.OneTimePath);
            var oneTimeModel = new LogInternalOneTimeModel
            {
                Date = $"{date:yyyyMMdd}",
                FolderName = fileInfo.Name,
                GenerateTime = $"{DateTime.Now:yyyy-MM-dd hh:mm:ss tt}",
                TargetName = AppConfigService.Config.TargetName,
                Group = AppConfigService.Config.ToavgBetBenchmarkSettings.LetstalkGroup
            };

            dataService.LetstalkPreRunLogInsert(oneTimeModel);
        }

        public void CopyFiles(List<string> fileNames)
        {
            var date = DateTime.Today.AddDays(-1);
            var path = $"{AppConfigService.Config.OneTimePath}/{date:yyyyMMdd}";
            Directory.CreateDirectory(path);
            var index = 1;
            fileNames.ForEach(fileName =>
            {
                var sortName = $"{index}_{new FileInfo(fileName).Name}";
                var destinationPath = $"{path}/{sortName}";
                File.Copy(fileName, destinationPath, true);
                index++;
            });

            LogOneTime(date);
        }
    }
}