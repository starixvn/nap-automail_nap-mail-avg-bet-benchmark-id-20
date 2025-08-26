using System;
using Fanex.Data.Repository;

namespace Fanex.TOAVGBetBenchmark.Business.Criteria
{
    public class ToavgBetBenchmarkCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_Benchmark";

        public override bool IsValid() => true;
    }
}