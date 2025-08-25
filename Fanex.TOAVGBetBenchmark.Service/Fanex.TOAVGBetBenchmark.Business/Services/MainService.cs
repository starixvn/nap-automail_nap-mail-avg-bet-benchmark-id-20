using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using Fanex.TOAVGBetBenchmark.Business.Model;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface MainService
    {
        JobMessage Execute();
    }

    public class MainServiceImp(
        Log log,
        DataService dataService,
        LetstalkClientService letstalkService,
        ExcelService excelService,
        MessageService messageService,
        FileService fileService,
        MailService mailService,
        DateRangeService dateRangeService)
        : MainService
    {
        private readonly List<string> filePaths = [];
        private Mail mail = new() { Subject = string.Empty, Body = string.Empty };

        public JobMessage Execute()
        {
            filePaths.Clear();
            var result = new ExecuteResultModel();
            JobMessage jobMessage = new();
            try
            {
                result = SendNotification();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
                log.Log(e);
            }
            finally
            {
                jobMessage.Mail = result.Mail;
                jobMessage.JobStatus = result.Success ? JobStatus.Success : JobStatus.DataIsNotReady;
                jobMessage.TaskLog = new TaskLog
                {
                    LogTime = DateTime.Now,
                    Message = result.Message
                };
            }

            return jobMessage;
        }

        private ExecuteResultModel SendNotification()
        {
            if (!dataService.IsDataAvailable())
            {
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady, TaskID = 21" };
            }

            var letstalkFails = new List<string>();
            var rawData = dataService.QueryToavgBetBenchmark();
            rawData.ForEach(row =>
            {
                row.Benchmark /= 1000;
                row.AvgBenchmark /= 1000;
                row.Mtd /= 1000;
                row.AvgMtd /= 1000;
            });

            var errFiles = rawData.Count > 0
                ? SendLetstalkImageData(rawData)
                : SendLetstalkImageNoData();

            errFiles.ForEach(file =>
            {
                if (file.Length > 1)
                {
                    letstalkFails.Add(file);
                }
            });

            if (!AppConfigService.Config.IsSentDirectly)
            {
                fileService.CopyFiles(filePaths);
            }

            return new ExecuteResultModel
            {
                Success = letstalkFails.Count == 0,
                Message = string.Join(Environment.NewLine, letstalkFails),
                Mail = mail
            };
        }

        private List<string> SendLetstalkImageData(List<BenchmarkModel> data)
        {
            var imageSentFail = new List<string>();
            var setting = AppConfigService.Config.ToavgBetBenchmarkSettings;

            var excelAllPath = excelService.ExportExcel(data, 0, 0, true, setting);
            SendLetstalk(excelAllPath, setting.LetstalkGroup);
            SendMail(setting, excelAllPath);

            return imageSentFail;
        }

        private void SendMail(ToavgBetBenchmarkSettings setting, string excelAllPath)
        {
            mail = mailService.CreateMail(excelAllPath, setting.ExcelTitle, setting.Subject, dateRangeService.RptDate);
        }

        private List<string> SendLetstalkImageNoData()
        {
            var setting = AppConfigService.Config.ToavgBetBenchmarkSettings;
            var excelPath = excelService.ExportExcelWithNoData(setting);
            var status = SendLetstalk(excelPath, setting.LetstalkGroup);
            SendMail(setting, excelPath);
            return [status ? string.Empty : $"{excelPath} - Letstalk sent Unsuccessfully."];
        }

        private bool SendLetstalk(string imagePath, string group)
        {
            var letstalkModel = messageService.CreateLetstalkModel(imagePath, group);
            var status = !AppConfigService.Config.IsSentDirectly || letstalkService.SendMessage(letstalkModel);

            filePaths.Add(imagePath);

            return status;
        }
    }
}