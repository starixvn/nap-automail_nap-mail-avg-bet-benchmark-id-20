using Fanex.Data.Repository;
using System;

namespace Fanex.TOAVGBetBenchmark.Business.Criterias
{
    public class TOAVGBetBenchmarkCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_Benchmark";

        public override bool IsValid() => true;
    }
}