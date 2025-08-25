using System.Collections.Generic;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    using System.IO;
    using LetstalkService.Client.Models;

    public interface MessageService
    {
        LetstalkModel CreateLetstalkModel(string fileName, string letstalkGroup);
    }

    public class MessageServiceImp : MessageService
    {
        public LetstalkModel CreateLetstalkModel(string fileName, string letstalkGroup)
        {
            var letstalkAttachments = new List<LetstalkAttachment>
            {
                new()
                {
                    FileName = fileName,
                    Bytes = File.ReadAllBytes(fileName)
                }
            };

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