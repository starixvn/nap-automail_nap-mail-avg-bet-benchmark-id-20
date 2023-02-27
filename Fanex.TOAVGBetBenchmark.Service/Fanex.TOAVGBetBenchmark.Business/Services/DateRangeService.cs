using System;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface DateRangeService
    {
        DateTime RptDate { get; }
    }

    public class DateRangeServiceImp : DateRangeService
    {
        public DateTime RptDate
        {
            get
            {
                if (AppConfigService.Config.IsManual)
                {
                    return AppConfigService.Config.RptDate;
                }

                var date = DateTime.Now.AddDays(-1);
                return Convert.ToDateTime($"{date:yyyy-MM-dd}");
            }
        }
    }
}