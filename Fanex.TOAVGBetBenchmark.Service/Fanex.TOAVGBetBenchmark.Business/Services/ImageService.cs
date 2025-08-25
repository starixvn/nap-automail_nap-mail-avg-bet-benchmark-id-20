using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Spire.Xls;
using System;
using System.IO;
using Fanex.TOAVGBetBenchmark.Business.Model;
using Image = SixLabors.ImageSharp.Image;
using Rectangle = SixLabors.ImageSharp.Rectangle;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface ImageService
    {
        string ExportImage(string excelPath, int batchIndex, string sheetNameNeedToExport, ImageProperty imageProperty);
    }

    public class ImageServiceImp(Log log, DateRangeService dateService) : ImageService
    {
        public string ExportImage(string excelPath, int batchIndex, string sheetNameNeedToExport,
            ImageProperty imageProperty)
        {
            var date = dateService.RptDate;
            var imageName = $"{AppConfigService.Config.Path}/{date:yyyyMMdd}";
            if (!Directory.Exists(imageName))
            {
                Directory.CreateDirectory(imageName);
            }

            imageName = $"{imageName}/{imageProperty.ImageName}_{batchIndex + 1}_On_{date:yyyyMMdd}.jpg";
            var imageNameFinal =
                $"{AppConfigService.Config.Path}/{date:yyyyMMdd}/{imageProperty.ImageName}_{batchIndex + 1}_F_{date:yyyyMMdd}.jpg";

            try
            {
                using var workbook = new Workbook();
                workbook.OpenPassword = AppConfigService.Config.Password;
                workbook.LoadFromFile(excelPath);
                using var sheet = workbook.Worksheets[sheetNameNeedToExport];
                using var stream = sheet.ToImage(imageProperty.FirstRow, imageProperty.FirstColumn,
                    imageProperty.LastRow, imageProperty.LastColumn);
                using var ms = new MemoryStream();
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);

                var arr = ms.ToArray();
                File.WriteAllBytes(imageName, arr);

                var cropArea = new Rectangle(imageProperty.X, imageProperty.Y, imageProperty.CropWidth,
                    imageProperty.CropHeight);
                CropImage(imageName, imageNameFinal, cropArea);

                if (File.Exists(imageName))
                {
                    File.Delete(imageName);
                }

                return imageNameFinal;
            }
            catch (Exception ex)
            {
                log.Log(ex);
            }

            return imageNameFinal;
        }

        private static void CropImage(string inputPath, string outputPath, Rectangle cropArea)
        {
            using var image = Image.Load<Rgba64>(inputPath);
            image.Mutate(x => x.Crop(cropArea));
            image.Save(outputPath);
        }
    }
}