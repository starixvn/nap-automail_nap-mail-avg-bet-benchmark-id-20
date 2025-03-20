using Fanex.TOAVGBetBenchmark.Business.Models;
using Fanex.TOAVGBetBenchmark.Service.Services;
using Spire.Xls;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface ImageService
    {
        string ExportImage(string excelPath, int batchIndex, string sheetNameNeedToExport, ImageProperty imageProperty);

        void ResizeImage(string org, string des, int finalWidth, int finalHeight, int imageQuality);
    }

    public class ImageServiceImp : ImageService
    {
        private readonly Log log;
        private readonly DateRangeService dateService;

        public ImageServiceImp(DateRangeService dateService)
        {
            this.dateService = dateService;
        }

        public ImageServiceImp(Log log, DateRangeService dateService)
        {
            this.log = log;
            this.dateService = dateService;
        }

        public string ExportImage(string excelPath, int batchIndex, string sheetNameNeedToExport, ImageProperty imageProperty)
        {
            var date = dateService.RptDate;
            var imageName = $@"{AppConfigService.Config.Path}/{date:yyyy}/{date:MM}";
            if (!Directory.Exists(imageName))
            {
                Directory.CreateDirectory(imageName);
            }

            imageName = $@"{imageName}/{imageProperty.ImageName}_On_{date:yyyyMMdd}.jpg";
            var imageNameFinal = $@"{AppConfigService.Config.Path}/{date:yyyy}/{date:MM}/{imageProperty.ImageName}_F_{batchIndex + 1}_{date:yyyyMMdd}.jpg";
            var imageNameResizeWidth = $@"{AppConfigService.Config.Path}/{date:yyyy}/{date:MM}/{imageProperty.ImageName}_LQ_{batchIndex + 1}_{date:yyyyMMdd}.jpg";

            try
            {
                using var workbook = new Workbook();
                workbook.OpenPassword = AppConfigService.Config.Password;
                workbook.LoadFromFile(excelPath, ExcelVersion.Version2013);
                workbook.DocumentProperties.Clear();
                using var sheet = workbook.Worksheets[sheetNameNeedToExport];
                using var ms = new MemoryStream();
                var stream = sheet.ToImage(imageProperty.FirstRow, imageProperty.FirstColumn, imageProperty.LastRow, imageProperty.LastColumn);
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                using var img = Image.FromStream(ms);
                img.Save(imageName, ImageFormat.Jpeg);

                Crop(imageName, imageNameFinal, imageProperty.CropWidth, imageProperty.CropHeight, imageProperty.X, imageProperty.Y);

                if (imageProperty.IsResize)
                {
                    ResizeImage(imageNameFinal, imageNameResizeWidth, imageProperty.ResizeNewWidth, 0, 100);
                    return imageNameResizeWidth;
                }

                return imageNameFinal;
            }
            catch (Exception ex)
            {
                log.Log(ex);
            }

            return imageNameFinal;
        }

        private void Crop(string sourceImg, string newImage, int width, int height, int x, int y)
        {
            using var originalImage = Image.FromFile(sourceImg);
            using var bmp = new Bitmap(width, height);

            bmp.SetResolution(300, 300);
            using (Graphics graphic = Graphics.FromImage(bmp))
            {
                graphic.CompositingMode = CompositingMode.SourceOver;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphic.DrawImage(originalImage, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
                using var ms = new MemoryStream();
                bmp.Save(ms, originalImage.RawFormat);

                if (File.Exists(newImage))
                {
                    File.Delete(newImage);
                }

                File.WriteAllBytes(newImage, ms.GetBuffer());
            }

            if (File.Exists(sourceImg))
            {
                File.Delete(sourceImg);
            }
        }

        public void ResizeImage(string org, string des, int finalWidth, int finalHeight, int imageQuality)
        {
            var bmp = new Bitmap(org);

            int iWidth;
            int iHeight;
            if ((finalHeight == 0) && (finalWidth != 0))
            {
                iWidth = finalWidth;
                iHeight = (bmp.Size.Height * iWidth / bmp.Size.Width);
            }
            else if ((finalHeight != 0) && (finalWidth == 0))
            {
                iHeight = finalHeight;
                iWidth = (bmp.Size.Width * iHeight / bmp.Size.Height);
            }
            else
            {
                iWidth = finalWidth;
                iHeight = finalHeight;
            }

            using var newBMP = new Bitmap(iWidth, iHeight);
            using var graphicTemp = Graphics.FromImage(newBMP);
            graphicTemp.CompositingMode = CompositingMode.SourceOver;
            graphicTemp.CompositingQuality = CompositingQuality.HighQuality;
            graphicTemp.SmoothingMode = SmoothingMode.HighQuality;
            graphicTemp.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphicTemp.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphicTemp.DrawImage(bmp, 0, 0, iWidth, iHeight);
            EncoderParameters encoderParams = new EncoderParameters();
            EncoderParameter encoderParam = new EncoderParameter(Encoder.Quality, imageQuality);
            encoderParams.Param[0] = encoderParam;
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            for (int fwd = 0; fwd <= arrayICI.Length - 1; fwd++)
            {
                if (arrayICI[fwd].FormatDescription.Equals("JPEG"))
                {
                    newBMP.SetResolution(300, 300);
                    newBMP.Save(des, arrayICI[fwd], encoderParams);
                }
            }
        }
    }
}