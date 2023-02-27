using Fanex.Data.Repository;

namespace Fanex.TOAVGBetBenchmark.Business.Criterias
{
    public class GetDWStatusCriteria : CriteriaBase
    {
        public string TaskIDList => "21";

        public override string GetSettingKey()
        {
            return "DailyJob_CheckTasksStatus";
        }

        public override bool IsValid() => true;
    }
}