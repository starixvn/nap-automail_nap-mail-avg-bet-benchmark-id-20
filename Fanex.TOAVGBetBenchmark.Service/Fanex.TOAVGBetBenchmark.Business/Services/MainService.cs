using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.TOAVGBetBenchmark.Business.Models;
using Fanex.TOAVGBetBenchmark.Service.Services;
using System;
using System.Collections.Generic;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface MainService
    {
        JobMessage Execute();
    }

    public class MainServiceImp : MainService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly LetstalkClientService letstalkService;
        private readonly ExcelService excelService;
        private readonly MessageService messageService;
        private readonly FileService fileService;
        private readonly MailService mailService;
        private readonly DateRangeService dateRangeService;

        private readonly List<string> filePaths = new List<string>();
        private Mail mail = new() { Subject = string.Empty, Body = string.Empty };

        public MainServiceImp(
            Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            ExcelService excelService,
            MessageService messageService,
            FileService fileService,
            MailService mailService,
            DateRangeService dateRangeService)
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelService = excelService;
            this.messageService = messageService;
            this.fileService = fileService;
            this.mailService = mailService;
            this.dateRangeService = dateRangeService;
        }

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
            var rawData = dataService.QueryTOAVGBetBenchmark();
            rawData.ForEach(row =>
            {
                row.Benchmark /= 1000;
                row.AVGBenchmark /= 1000;
                row.MTD /= 1000;
                row.AVGMTD /= 1000;
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
            var letstalkFails = new List<string>();
            var setting = AppConfigService.Config.TOAVGBetBenchmarkSettings;

            var excelAllPath = excelService.ExportExcel(data, 0, 0, true, setting);
            SendLetstalk(excelAllPath, setting.LetstalkGroup);
            SendMail(setting, excelAllPath);

            if (imageSentFail.Count > 0)
            {
                letstalkFails.Add($"{string.Join(Environment.NewLine, imageSentFail)} - Letstalk sent Unsuccessfully.");
            }

            return imageSentFail;
        }

        private void SendMail(TOAVGBetBenchmarkSettings setting, string excelAllPath)
        {
            mail = mailService.CreateMail(excelAllPath, setting.Exceltitle, setting.Subject, dateRangeService.RptDate);
        }

        private List<string> SendLetstalkImageNoData()
        {
            var setting = AppConfigService.Config.TOAVGBetBenchmarkSettings;
            var excelPath = excelService.ExportExcelWithNoData(setting);
            var status = SendLetstalk(excelPath, setting.LetstalkGroup);
            SendMail(setting, excelPath);
            return [status ? string.Empty : $"{excelPath} - Letstalk sent Unsuccessfully."];
        }

        private bool SendLetstalk(string imagePath, string group)
        {
            var letstalkModel = messageService.CreateLetstalkModel(imagePath, group);
            bool status;
            if (AppConfigService.Config.IsSentDirectly)
            {
                status = letstalkService.SendMessage(letstalkModel);
            }
            else
            {
                status = true;
            }

            filePaths.Add(imagePath);

            return status;
        }
    }
}