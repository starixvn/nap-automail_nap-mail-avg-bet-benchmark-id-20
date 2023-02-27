using Fanex.MailDelivery.Core.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface MailService
    {
        Mail CreateMail(string fileName, string excelTitle, string subject, DateTime rptDate);
    }

    public class MailServiceImp : MailService
    {
        public Mail CreateMail(string fileName, string excelTitle, string subject, DateTime rptDate)
        {
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            return new Mail
            {
                Title = "ONEworks Auto Generated Report Administrator",
                Subject = $"{subject} On {rptDate:yyyy/MM/dd}",
                Body = Content(fileName, excelTitle, rptDate),
                Attachments = attachments,
            };
        }

        private string Content(string fileName, string excelTitle, DateTime rptDate)
        {
            var tuple = new Tuple<string, string, DateTime>(new FileInfo(fileName).Name, excelTitle, rptDate);
            var template = File.ReadAllText("Templates/TOAVGBetBenchmark.cshtml");
            var result = Engine.Razor.RunCompile(template, $"Template-TOAVGBetBenchmark", model: tuple);

            return result;
        }
    }
}