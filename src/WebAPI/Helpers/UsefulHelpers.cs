using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI.Enums;
using WebAPI.Models.CustomModels;
using WebAPI.SystemResource;
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Helpers
{
    public static class UsefulHelpers
    {
        public const string FILEPREVIEWMAINURL = "http://10.0.0.10:8089/";

        public static string GetUniqueFileName(string extension)
        {
            string filename = $"{Guid.NewGuid()}.{extension.Replace(".","")}";
            return filename;
        }

        private static bool IsMultipartContentType(string contentType)
        {
            return
                !string.IsNullOrEmpty(contentType) &&
                contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string GetBoundary(string contentType)
        {
            var elements = contentType.Split(' ');
            var element = elements.Where(entry => entry.StartsWith("boundary=")).First();
            var boundary = element.Substring("boundary=".Length);
            // Remove quotes
            if (boundary.Length >= 2 && boundary[0] == '"' &&
                boundary[boundary.Length - 1] == '"')
            {
                boundary = boundary.Substring(1, boundary.Length - 2);
            }
            return boundary;
        }


        public static string DayOfWeekShortName(this DayOfWeekMN name)
        {
            return Convert.ToString(name).Substring(0, 3);
        }


        static public int[] toListArray(IList strArray)
        {
            int[] intArray = new int[strArray.Count];
            for (int index = 0; index < strArray.Count; index++)
            {
                intArray[index] = Convert.ToInt32(Convert.ToString(strArray[0]).Split('=')[1].Replace("}", "").Trim());
            }
            return intArray;
        }



        public static object GetIdendityValue(HttpContext context,   UserProperties userproperties)
        {
            try
            {
                var jsonData = context.User.Claims.ToList()[0].Value;
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
                else
                {
                    return request.UserMail;
                }
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }


        private static void DrawWatermark(Bitmap watermark_bm, Bitmap result_bm, int x, int y)
        {
            const byte ALPHA = 128;
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

        public static Image StampImage(Image imageData)
        {
            Bitmap result_bm = new Bitmap(imageData);
            int x = 0;
            int y = 0;
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://image.ibb.co/m10PGQ/zendstamp.png");
            Bitmap bitmap;
            bitmap =  new Bitmap(stream);

            int bheight = imageData.Size.Height * 10 / 100;
            int bwidth = imageData.Size.Height * 10 / 100;
            Image sImage = ResizeImage(bitmap, bwidth, bheight); //Image.FromFile(@"C:\Users\Davkharbayar\Desktop\ZENDMENES.png", true);



            x = result_bm.Size.Width - sImage.Size.Width;
            y = result_bm.Height - sImage.Size.Height;
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




    }
}
