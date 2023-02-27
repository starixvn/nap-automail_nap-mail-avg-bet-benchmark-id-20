using System;
using System.Collections.Generic;

namespace Fanex.TOAVGBetBenchmark.Business.Models
{
    public class ConfigSettingModel
    {
        public bool IsCheckEnoughData { get; set; }

        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string LetstalkServiceApi { get; set; }

        public string Path { get; set; }

        public string TargetName { get; set; }

        public string Password { get; set; }

        public int FunctionIdNodata { get; set; }

        public bool IsManual { get; set; }

        public DateTime RptDate { get; set; }

        public Dictionary<string, ImageProperty> ImageSettings { get; set; } = new Dictionary<string, ImageProperty>();

        public TOAVGBetBenchmarkSettings TOAVGBetBenchmarkSettings { get; set; }

        public bool IsSentDirectly { get; set; }

        public string OneTimePath { get; set; }
    }
}