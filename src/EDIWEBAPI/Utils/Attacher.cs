using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Drawing;

namespace EDIWEBAPI.Utils
{
    public class Attacher
    {
        private static readonly string UPLOAD_FOLDER_NAME = "uploads";

        private static bool FileSizeCheck(IFormFile file)
        {
            return !(file.Length / 1024 / 1024 > 10);
        }

        public static ResponseClient File(ILogger _log, IFormFile uploadedFile, string folderName, string filePath = null)
        {
            if (uploadedFile == null)
                return ReturnResponce.ModelIsNotValudResponce();

            var logMethod = "Attacher.File";
            try
            {
                string extension = Path.GetExtension(uploadedFile.FileName).ToLower();

                var file = uploadedFile;
                if (file.Length == 0)
                    return ReturnResponce.ModelIsNotValudResponce();

                if (!FileSizeCheck(uploadedFile))
                    return ReturnResponce.FailedMessageResponce("Зөвшөөрөгдөх хэмжээнээс хэтэрсэн файл байна.");

                if (!UsefulHelpers.HasActivatedFileExtension(extension))
                    return ReturnResponce.FailedMessageResponce("Зөвшөөрөгдөөгүй файл байна.");
                
                var folder = Path.Combine($"{UPLOAD_FOLDER_NAME}/{folderName}");
                var fileName = string.IsNullOrEmpty(filePath) ? 
                    UsefulHelpers.GetUniqueFileName(extension) : filePath;

                if (_log != null)
                    _log.LogInformation($"{logMethod} FILEPATH : {fileName}");

                using (var fileStream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                return ReturnResponce.ListReturnResponce(Path.Combine(string.Format("{0}/{1}", folder, fileName)));
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.LogError($"{logMethod} ERROR : {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static ResponseClient Picture(ILogger _log, IFormFile uploadedFile, string folderName, string thumbnailFolder)
        {
            if (uploadedFile == null)
                return ReturnResponce.ModelIsNotValudResponce();

            var logMethod = "Attacher.Picture";
            try
            {
                string extension = Path.GetExtension(uploadedFile.FileName).ToLower();

                var file = uploadedFile;
                if (file.Length == 0)
                    return ReturnResponce.ModelIsNotValudResponce();

                //if (!FileSizeCheck(uploadedFile))
                //    return ReturnResponce.FailedMessageResponce("Зөвшөөрөгдөх хэмжээнээс хэтэрсэн файл байна.");

                //if (!UsefulHelpers.HasActivatedFileExtension(extension))
                //    return ReturnResponce.FailedMessageResponce("Зөвшөөрөгдөөгүй файл байна.");

                Image image = Image.FromStream(file.OpenReadStream(), true, true);
                // image = UsefulHelpers.StampImage(image);

                var folder = Path.Combine($"uploads/{folderName}");
                var fileName = UsefulHelpers.GetUniqueFileName(extension);

                if (_log != null)
                    _log.LogInformation($"{logMethod} FILEPATH : {fileName}");

                using (var fileStream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                if (!string.IsNullOrEmpty(thumbnailFolder))
                {
                    var thumbnail = Path.Combine($"{UPLOAD_FOLDER_NAME}/{thumbnailFolder}");
                    var thumbimage = image.GetThumbnailImage(128, 128, null, IntPtr.Zero);
                    using (var filestreamthumb = new FileStream(Path.Combine(thumbnail, fileName), FileMode.Create))
                    {
                        thumbimage.Save(filestreamthumb, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }

                return ReturnResponce.ListReturnResponce(Path.Combine(string.Format("{0}/{1}", folder, fileName)));
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.LogError($"{logMethod} ERROR : {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
    }
}
