using EDIWEBAPI.Entities.APIModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace EDIWEBAPI.Utils
{
    public static class UsefulHelpers
    {
        public static readonly int STORE_ID = 2;
        public static readonly string STORE_NAME = "Скай Хайпермаркет";
        public static readonly string NO_IMAGE_URL = "https://www.biber.com/dta/themes/biber/core/assets/images/no-featured-175.jpg";
        private static readonly Random RANDOM_NUMBER = new Random();

        public static string ConvertDatetimeToString(DateTime? value)
        {
            return !value.HasValue ? string.Empty : value?.ToString("yyyyMMdd");
        }

        public static FileType GetFileType(IFormFile file)
        {
            var type = FileType.File;
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".jpg" || extension == ".png"
                || extension == ".gif" || extension == ".jpeg")
                type = FileType.Image;
            else if (extension == ".exe" || extension == ".bat")
                type = FileType.Forbidden;

            return type;
        }

        public static int GetRandomNumber(int min, int max)
        {
            lock (RANDOM_NUMBER)
            {
                return RANDOM_NUMBER.Next(min, max);
            }
        }

        public static bool IsNewOrgType(string type)
        {
            return type == "ORG";
        }

        public static string GetUserName(Entities.DBModel.SystemManagement.SYSTEM_USERS user)
        {
            return GetUserName(user.USERMAIL, user.FIRSTNAME);
        }

        public static string GetUserName(string userMail, string userName)
        {
            return $"{userMail} {(string.IsNullOrEmpty(userName) ? string.Empty : "/ " + userName + " /")}";
        }

        public static DateTime ConvertToDatetime(string value)
        {
            return DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static string SubString(string value, int index, int length)
        {
            return string.IsNullOrEmpty(value) || value.Length < index + length ? value : value.Substring(index, length); 
        }

        public static string GetEndDate(int year, string month)
        {
            var intMonth = int.Parse(month);
            return new DateTime(year, intMonth, DateTime.DaysInMonth(year, intMonth)).ToString("yyyyMMdd");
        }

        public static DateTime ConvertDate(DateTime value)
        {
            return value.AddDays(1).AddSeconds(-1);
        }

        public static void WriteErrorLog(ILogger _log, System.Reflection.MethodBase method, Exception ex)
        {
            _log.LogError(1, ex, string.Format("{0}.{1}", method.DeclaringType, method.Name));
        }

        public static string[] GetControllerRoute(HttpContext context)
        {
            var path = context.Request.Path.Value.Replace("/api", "");
            var count = path.Count(p => p == '/');
            var values = path.Split('/');

            return new string[] { FormPascalCase(values[1]), count == 2 ? values[2] : context.Request.Method };
        }

        public static string FormPascalCase(string value)
        {
            return $"{value.Substring(0, 1).ToUpper()}{value.Substring(1, value.Length - 1)}";
        }

        public static ProductImageType GetImageType(string fileName)
        {
            var type = ProductImageType.Product;

            if (fileName.ToUpper().StartsWith("PRO"))
                type = ProductImageType.Product;
            else if (fileName.ToUpper().StartsWith("OFF"))
                type = ProductImageType.Official;
            else if (fileName.ToUpper().StartsWith("ORG"))
                type = ProductImageType.Organization;

            return type;
        }

        public static bool IsValidTwoEmail(string[] emails)
        {
            return emails.Length == 2 && IsActualEmail(emails[0]) && IsActualEmail(emails[1]);
        }

        public static bool CheckSecurityKey(string key)
        {
            return !string.IsNullOrEmpty(key) && key == "N4Z=PccSn__6gMVT";
        }

        public static string GetMethodName(System.Reflection.MethodBase method)
        {
            return $"{method.DeclaringType}.{method.Name}";
        }

        public static Entities.CustomModels.Company GetCompanyInfo(ILogger _log, string registryNo)
        {
            var formatted = registryNo.PadLeft(7, '0');
            var url = string.Format("http://info.ebarimt.mn/rest/merchant/info?regno={0}", formatted);
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = @"text/xml;charset=""utf-8""";

                var response = (HttpWebResponse)request.GetResponse();

                var dataStream = response.GetResponseStream();
                var reader = new StreamReader(dataStream);
                var result = reader.ReadToEnd();

                var company = JsonConvert.DeserializeObject<Entities.CustomModels.Company>(result);

                reader.Close();
                dataStream.Close();

                return company;
            }
            catch (Exception ex)
            {
                _log.LogError($"GetCompany : {registryNo} => {ex.Message}");
                return null;
            }
        }

        public static string GetExceptionMessage(Exception ex)
        {
            return ex.InnerException == null ? ex.Message : GetExceptionMessage(ex.InnerException);
        }

        public static string SerializeLower(object value)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new LowercaseJsonResolver()
            };
            return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        }

        public static bool IsNull(string value)
        {
            return string.IsNullOrEmpty(value) || value.ToUpper() == "NULL";
        }

        public static string ReplaceNull(string value)
        {
            return IsNull(value) ? "%" : value;
        }

        public static bool IsActualPhone(string value)
        {
            return value != null && value.Trim().Length == 8
                && (new Regex("[1-9]{1}[0-9]{7}").Match(value.Trim()).Success);
        }

        public static bool IsActualEmail(string value)
        {
            return value != null && value.Trim().Length > 0;
                //&& value.Contains("@");
        }

        public static string GetUniqueFileName(string extension)
        {
            return $"{Guid.NewGuid()}.{extension.Replace(".", "")}";
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static object GetIdendityValue(HttpContext context, UserProperties userproperties)
        {
            try
            {
                var jsonDataDecedod = context.User.Claims.ToList()[0].Value;

                var jsonData = EncryptionUtils.Decrypt(jsonDataDecedod, EncryptionUtils.ENCRYPTION_KEY);
                string data = Convert.ToString(JsonConvert.DeserializeObject(jsonData));
                dynamic request = JObject.Parse(data.Replace("[", "").Replace("]", ""));
                if (userproperties == UserProperties.UserId)
                {
                    return request.UserId;
                }
                else if (userproperties == UserProperties.CompanyId)
                {
                    return request.CompanyId;
                }
                else if (userproperties == UserProperties.CompanyReg)
                {
                    return request.ComRegNo;
                }
                else if (userproperties == UserProperties.OrgType)
                {
                    return request.OrgType;
                }
                else if (userproperties == UserProperties.IsHeaderCompanyUser)
                {
                    return request.isHeaderCompanyUser;
                }
                else if (userproperties == UserProperties.CompanyName)
                {
                    return request.CompanyName;
                }
                else if (userproperties == UserProperties.Roleid)
                {
                    return request.Roleid;
                }
                else if (userproperties == UserProperties.Isagreement)
                {
                    return request.Isagreement;
                }
                else if(userproperties == UserProperties.IsForeign)
                {
                    return request.IsForeign;
                }
                else
                {
                    return request.UserMail;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Image StampImage(Image imageData)
        {
            Bitmap result_bm = new Bitmap(imageData);
            int x = 0;
            int y = 0;
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("http://api.urto.mn/uploads/resource/sanhuu_tamga.png");
            Bitmap bitmap;
            bitmap = new Bitmap(stream);

            int bheight = imageData.Size.Height * 20 / 100;
            int bwidth = imageData.Size.Height * 20 / 100;
            Image sImage = ResizeImage(bitmap, bwidth, bheight); //Image.FromFile(@"C:\Users\Davkharbayar\Desktop\ZENDMENES.png", true);



            x = result_bm.Size.Width - sImage.Size.Width - 50;
            y = result_bm.Height - sImage.Size.Height - 550;
            DrawWatermark((Bitmap)sImage, result_bm, x, y);
            return result_bm;

        }

        public static Image StampQRImage(Image imageData)
        {
            Bitmap result_bm = new Bitmap(imageData);
            int x = 0;
            int y = 0;
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("http://api.urto.mn/uploads/resource/urto_6464.jpg");
            Bitmap bitmap;
            bitmap = new Bitmap(stream);

            int bheight = imageData.Size.Height * 23 / 100;
            int bwidth = imageData.Size.Height * 23 / 100;
            Image sImage = ResizeImage(bitmap, bwidth, bheight); //Image.FromFile(@"C:\Users\Davkharbayar\Desktop\ZENDMENES.png", true);



            x = (result_bm.Size.Width  / 2) - (bwidth / 2);
            y = (result_bm.Height / 2) - (bheight / 2);
            DrawWatermark((Bitmap)sImage, result_bm, x, y);
            return result_bm;

        }

   



        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private static void DrawWatermark(Bitmap watermark_bm, Bitmap result_bm, int x, int y)
        {
            const byte ALPHA = 255;
            Image retImage;
            Color clr;
            for (int py = 0; py < watermark_bm.Height; py++)
            {
                for (int px = 0; px < watermark_bm.Width; px++)
                {
                    clr = watermark_bm.GetPixel(px, py);
                    watermark_bm.SetPixel(px, py, Color.FromArgb(ALPHA, clr.R, clr.G, clr.B));
                }
            }

            // Set the watermark's transparent color.
            watermark_bm.MakeTransparent(watermark_bm.GetPixel(0, 0));

            // Copy onto the result image.
            using (Graphics gr = Graphics.FromImage(result_bm))
            {
                gr.DrawImage(watermark_bm, x, y);
            }
        }





        //public static String GetIP(this HttpContext httpcontext)
        //{


        //    if (HttpContext.Current == null) return "";
        //    return httpcontext.Current.Request.UserHostAddress;
        //    return ip;
        //}

        public static int  ObjectGetByteLength(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] Array;
            bf.Serialize(ms, obj);
            Array = ms.ToArray();
            return Array.Length;
        }

        public static string GetClientIPAddress(this HttpContext context)
        {
            if (null == context)
            {
                return "";
            }
             var ipaddress =Convert.ToString(context.Request.HttpContext.Connection.RemoteIpAddress);
            return ipaddress;
        }




        public static bool HasImageExtension(this string imagesource)
        {
            string source = imagesource.ToLower();
            return (source.EndsWith(".png") || source.EndsWith(".jpg") || source.EndsWith(".bmp") || source.EndsWith(".gif") || source.EndsWith(".jpeg"));
        }



        public static bool HasSuspiciousFileExtension(string filename)
        {
            string fileextension = filename.Split('.')[1].ToUpper();
            string[] extensions = new string[] { "ACTION", "APK", "APP", "BAT", "BIN", "CMD", "COM", "COMMAND", "CPL", "CSH", "EXE", "GADGET", "INF1", "INS", "INX", "IPA", "ISU", "JOB", "JSE", "KSH", "LNK", "MSC", "MSI", "MSP", "MST", "OSX", "OUT", "PAF", "PIF", "PRG", "PS1", "REG", "RGS", "RUN", "SCR", "SCT", "SHB", "SHS", "U3P", "VB", "VBE", "VBS", "VBSCRIPT", "WORKFLOW", "WS", "WSF", "WSH", "0XE", "73K", "89K", "A6P", "AC", "ACC", "ACR", "ACTM", "AHK", "AIR", "APP", "ARSCRIPT", "AS", "ASB", "AWK", "AZW2", "BEAM", "BTM", "CEL", "CELX", "CHM", "COF", "CRT", "DEK", "DLD", "DMC", "DOCM", "DOTM", "DXL", "EAR", "EBM", "EBS", "EBS2", "ECF", "EHAM", "ELF", "ES", "EX4", "EXOPC", "EZS", "FAS", "FKY", "FPI", "FRS", "FXP", "GS", "HMS", "HPF", "HTA", "IIM", "IPF", "ISP", "JAR", "JS", "JSX", "KIX", "LO", "LS", "MAM", "MCR", "MEL", "MPX", "MRC", "MS", "MS", "MXE", "NEXE", "OBS", "ORE", "OTM", "PEX", "PLX", "POTM", "PPAM", "PPSM", "PPTM", "PRC", "PVD", "PWC", "PYC", "PYO", "QPX", "RBX", "ROX", "RPJ", "S2A", "SBS", "SCA", "SCAR", "SCB", "SCRIPT", "SMM", "SP", "TCP", "THM", "TLB", "TMS", "UDF", "UPX", "URL", "VLX", "VPM", "WCM", "WIDGET", "WIZ", "WPK", "WPM", "XAP", "XBAP", "XLAM", "XLM", "XLSM", "XLTM", "XQT", "XYS", "ZL9" };
            return extensions.Contains(fileextension);

        }

        public static bool HasActivatedFileExtension(string fileextension)
        {
            fileextension = fileextension.Replace(".", "");
            // string[] extensions = new string[] { "ACTION", "APK", "APP", "BAT", "BIN", "CMD", "COM", "COMMAND", "CPL", "CSH", "EXE", "GADGET", "INF1", "INS", "INX", "IPA", "ISU", "JOB", "JSE", "KSH", "LNK", "MSC", "MSI", "MSP", "MST", "OSX", "OUT", "PAF", "PIF", "PRG", "PS1", "REG", "RGS", "RUN", "SCR", "SCT", "SHB", "SHS", "U3P", "VB", "VBE", "VBS", "VBSCRIPT", "WORKFLOW", "WS", "WSF", "WSH", "0XE", "73K", "89K", "A6P", "AC", "ACC", "ACR", "ACTM", "AHK", "AIR", "APP", "ARSCRIPT", "AS", "ASB", "AWK", "AZW2", "BEAM", "BTM", "CEL", "CELX", "CHM", "COF", "CRT", "DEK", "DLD", "DMC", "DOCM", "DOTM", "DXL", "EAR", "EBM", "EBS", "EBS2", "ECF", "EHAM", "ELF", "ES", "EX4", "EXOPC", "EZS", "FAS", "FKY", "FPI", "FRS", "FXP", "GS", "HMS", "HPF", "HTA", "IIM", "IPF", "ISP", "JAR", "JS", "JSX", "KIX", "LO", "LS", "MAM", "MCR", "MEL", "MPX", "MRC", "MS", "MS", "MXE", "NEXE", "OBS", "ORE", "OTM", "PEX", "PLX", "POTM", "PPAM", "PPSM", "PPTM", "PRC", "PVD", "PWC", "PYC", "PYO", "QPX", "RBX", "ROX", "RPJ", "S2A", "SBS", "SCA", "SCAR", "SCB", "SCRIPT", "SMM", "SP", "TCP", "THM", "TLB", "TMS", "UDF", "UPX", "URL", "VLX", "VPM", "WCM", "WIDGET", "WIZ", "WPK", "WPM", "XAP", "XBAP", "XLAM", "XLM", "XLSM", "XLTM", "XQT", "XYS", "ZL9" };
            var extensions = new string[] { "pdf", "jpg", "png", "jpeg", "gif", "xlsx", "doc", "bmp", "xps" };

            foreach (string x in extensions)
            {
                if (fileextension.Contains(x))
                {
                    return true;
                }
            }
            return false;
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }



    }
}
