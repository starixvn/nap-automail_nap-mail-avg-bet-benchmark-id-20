using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using Fanex.TOAVGBetBenchmark.Business.Model;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface ExcelService
    {
        string ExportExcel(List<BenchmarkModel> data, int batchIndex, int totalBatch, bool isAllInfo, ToavgBetBenchmarkSettings setting);

        string ExportExcelWithNoData(ToavgBetBenchmarkSettings setting);
    }

    public class ExcelServiceImp(DateRangeService dateRangeService) : ExcelService
    {
        public string ExportExcel(List<BenchmarkModel> data, int batchIndex, int totalBatch, bool isAllInfo, ToavgBetBenchmarkSettings setting)
        {
            using var excelPackage = new ExcelPackage(new FileInfo("Templates/TOAVGBetBenchmark.xlsx"));
            using var template = excelPackage.Workbook.Worksheets["Data"];
            template.Cells["B4"].LoadFromCollection(data, false);

            RenderTitle(batchIndex, totalBatch, template, isAllInfo, setting);
            RenderData(excelPackage, data, template);

            return Save(excelPackage, batchIndex, isAllInfo);
        }

        private void RenderTitle(int batchIndex, int totalBatch, ExcelWorksheet template, bool isAllInfo, ToavgBetBenchmarkSettings setting)
        {
            var title = isAllInfo
                ? setting.ExcelTitle
                : $"{setting.ExcelTitle} - Page {batchIndex + 1} of {totalBatch}";

            title += $" (Sent Date: {dateRangeService.RptDate:dd-MMM-yyyy})";
            template.Cells["B2"].Value = title;
        }

        private void RenderData(ExcelPackage excelPackage, List<BenchmarkModel> data, ExcelWorksheet template)
        {
            var startRowIndex = 4;
            var maxRowIndex = data.Count + startRowIndex - 1;
            var templateNote = excelPackage.Workbook.Worksheets["Note"];

            var preAgentName = string.Empty;
            var rowCountGroup = 0;
            var rowOddEven = 0;

            for (int rowIndex = startRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                var row = data[rowIndex - startRowIndex];

                if (row.AgentName != preAgentName)
                {
                    preAgentName = row.AgentName;
                    rowCountGroup++;
                    rowOddEven = rowCountGroup % 2 == 0 ? 6 : 7;
                }

                for (int columnIndex = 1; columnIndex <= 9; columnIndex++)
                {
                    template.Cells[rowIndex, columnIndex].StyleID = templateNote.Cells[rowOddEven, columnIndex].StyleID;
                }

                template.Row(rowIndex).Height = 19.5;
            }

            templateNote.Cells["B2:B3"].Copy(template.Cells[maxRowIndex + 1, 2]);
        }

        private string Save(ExcelPackage excelPackage, int batchIndex, bool isAllInfo)
        {
            var date = dateRangeService.RptDate;
            var fileName = isAllInfo
                ? $"TO-AVG-Bet-Benchmark-{date:yyyyMMdd}.xlsx"
                : $"TO-AVG-Bet-Benchmark-{batchIndex + 1}-{date:yyyyMMdd}.xlsx";
            var path = $"{AppConfigService.Config.Path}/{date:yyyy}/{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $"{path}/{fileName}";
            excelPackage.Workbook.Worksheets.Delete("Note");
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }

        public string ExportExcelWithNoData(ToavgBetBenchmarkSettings setting)
        {
            using var excelPackage = new ExcelPackage(new FileInfo("Templates/TOAVGBetBenchmark-NoData.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];
            RenderTitleWithNoData(setting.ExcelTitle, template);

            return Save(excelPackage, 0, true);
        }

        private void RenderTitleWithNoData(string excelTitle, ExcelWorksheet template)
        {
            excelTitle += $" (Sent Date: {dateRangeService.RptDate:dd-MMM-yyyy})";
            template.Cells["B2"].Value = excelTitle;
        }
    }
}