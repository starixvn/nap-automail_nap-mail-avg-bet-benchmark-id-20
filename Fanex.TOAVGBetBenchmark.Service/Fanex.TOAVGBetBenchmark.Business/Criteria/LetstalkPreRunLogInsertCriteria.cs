using Fanex.Data.Repository;

namespace Fanex.TOAVGBetBenchmark.Business.Criteria
{
    public class LetstalkPreRunLogInsertCriteria : NonQueryCommand
    {
        public override string GetSettingKey() => "Acc_Rpt_LetstalkPreRunLog_Insert";

        public override bool IsValid() => true;

        public string FolderName { get; set; }

        public string Date { get; set; }

        public string GenerateTime { get; set; }

        public string TargetName { get; set; }

        public string Group { get; set; }

        public int MailId { get; set; }

        public string Subject { get; set; }

        public string ReportType { get; set; } = "0500";

        public bool IsSent { get; set; }
    }
}