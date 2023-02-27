using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using System.Collections.Generic;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface MessageService
    {
        LetstalkModel CreateLetstalkModel(string fileName, string letstalkGroup);
    }

    public class MessageServiceImp : MessageService
    {
        public LetstalkModel CreateLetstalkModel(string fileName, string letstalkGroup)
        {
            var letstalkAttachments = new List<LetstalkAttachment>();
            letstalkAttachments.Add(new LetstalkAttachment
            {
                FileName = fileName,
                Bytes = FileBinaryService.ConvertFileToBinary(fileName)
            });

            return new LetstalkModel
            {
                TargetName = AppConfigService.Config.TargetName,
                Group = letstalkGroup,
                LetstalkAttachments = letstalkAttachments,
                Subject = string.Empty,
                Content = string.Empty
            };
        }
    }
}