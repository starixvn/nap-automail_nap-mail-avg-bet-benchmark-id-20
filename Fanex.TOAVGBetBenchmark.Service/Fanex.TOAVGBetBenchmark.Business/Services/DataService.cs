using Fanex.Data.Repository;
using Fanex.TOAVGBetBenchmark.Business.Criteria;
using System.Collections.Generic;
using System.Linq;
using Fanex.TOAVGBetBenchmark.Business.Model;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface DataService
    {
        List<BenchmarkModel> QueryToavgBetBenchmark();

        bool IsDataAvailable();

        void LetstalkPreRunLogInsert(LogInternalOneTimeModel oneTimeModel);
    }

    public class DataServiceImp(DateRangeService dateRangeService, IDynamicRepository repository)
        : DataService
    {
        public List<BenchmarkModel> QueryToavgBetBenchmark()
        {
            var criteria = new ToavgBetBenchmarkCriteria
            {
                RptDate = dateRangeService.RptDate
            };

            return repository.Fetch<BenchmarkModel>(criteria).ToList();
        }

        public bool IsDataAvailable()
        {
            if (AppConfigService.Config.IsManual || !AppConfigService.Config.IsCheckEnoughData)
            {
                return true;
            }

            var status = repository.Get<TaskStatusModel>(new GetDwStatusCriteria());
            var result = status.DataIsReady;

            return result;
        }

        public void LetstalkPreRunLogInsert(LogInternalOneTimeModel oneTimeModel)
        {
            var criteria = new LetstalkPreRunLogInsertCriteria
            {
                FolderName = oneTimeModel.FolderName,
                Date = oneTimeModel.Date,
                GenerateTime = oneTimeModel.GenerateTime,
                TargetName = oneTimeModel.TargetName,
                Group = oneTimeModel.Group,
                MailId = oneTimeModel.MailId,
                IsSent = false,
            };

            repository.Execute(criteria);
        }
    }
}