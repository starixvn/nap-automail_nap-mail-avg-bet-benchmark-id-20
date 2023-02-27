using Fanex.Data.Repository;
using Fanex.TOAVGBetBenchmark.Business.Criteria;
using Fanex.TOAVGBetBenchmark.Business.Criterias;
using Fanex.TOAVGBetBenchmark.Business.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface DataService
    {
        List<BenchmarkModel> QueryTOAVGBetBenchmark();

        bool IsDataAvailable();

        void LetstalkPreRunLogInsert(LogInternalOneTimeModel oneTimeModel);
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateRangeService dateRangeService;

        public DataServiceImp(DateRangeService dateRangeService, IDynamicRepository repository)
        {
            this.dateRangeService = dateRangeService;
            this.repository = repository;
        }

        public List<BenchmarkModel> QueryTOAVGBetBenchmark()
        {
            var criteria = new TOAVGBetBenchmarkCriteria
            {
                RptDate = dateRangeService.RptDate
            };

            return repository.Fetch<BenchmarkModel>(criteria).ToList();
        }

        public bool IsDataAvailable()
        {
            var result = false;
            if (AppConfigService.Config.IsManual || !AppConfigService.Config.IsCheckEnoughData)
            {
                return true;
            }

            var status = repository.Get<TaskstatusModel>(new GetDWStatusCriteria());
            result = status.DataIsReady;

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