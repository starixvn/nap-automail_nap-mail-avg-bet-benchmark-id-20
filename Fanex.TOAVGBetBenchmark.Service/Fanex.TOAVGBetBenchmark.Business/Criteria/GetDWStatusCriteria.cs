using Fanex.Data.Repository;

namespace Fanex.TOAVGBetBenchmark.Business.Criteria
{
    public class GetDwStatusCriteria : CriteriaBase
    {
        public string TaskIdList => "21";

        public override string GetSettingKey()
        {
            return "DailyJob_CheckTasksStatus";
        }

        public override bool IsValid() => true;
    }
}