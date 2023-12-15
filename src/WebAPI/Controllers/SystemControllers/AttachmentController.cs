using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Enums;
using WebAPI.Helpers;
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Controllers.AttachmentControllers
{
    [ApiVersion("1.0")]
    [Authorize(Policy = "StoreApiUser")]
    [Route("api/[controller]")]
    public class AttachmentController : Controller
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("file")]
        
        public  ResponseClient PostFile(IFormFile uploadedFile, AttachFileType ftype)
        {
                var file = uploadedFile;
                var uploads = Path.Combine("uploads\\attachedfiles");
            if (file.Length > 0)
            {
                var fileName = UsefulHelpers.GetUniqueFileName(file.FileName.Split('.')[file.FileName.Split('.').Length - 1]);
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                return ReturnResponce.ListReturnResponce(Path.Combine(uploads + "\\" + fileName));
            }
            else
            {
                return ReturnResponce.ModelIsNotValudResponce();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("logo")]
        public ResponseClient PostLogo(IFormFile uploadedFile)
        {
            var file = uploadedFile;
            var   uploads = Path.Combine("uploads\\logos");
            if (file.Length > 0)
            {
                var fileName = UsefulHelpers.GetUniqueFileName(file.FileName.Split('.')[file.FileName.Split('.').Length - 1]);
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                return ReturnResponce.ListReturnResponce(Path.Combine(uploads + "\\" + fileName));
            }
            else
            {
                return ReturnResponce.ModelIsNotValudResponce();
            }

        }


        [HttpPost]
        [AllowAnonymous]
        [Route("skuimage")]
        public ResponseClient PostSkuImage(IFormFile uploadedFile)
        {
            var file = uploadedFile;

            Image image = Image.FromStream(file.OpenReadStream(), true, true);

           
           image = UsefulHelpers.StampImage(image);


            var uploads = Path.Combine("uploads\\skuimage");
            if (file.Length > 0)
            {
                var fileName = UsefulHelpers.GetUniqueFileName(file.FileName.Split('.')[file.FileName.Split('.').Length - 1]);
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    //image.Save(fileStream, System.Drawing.Imaging.ImageFormat.Png);
                    file.CopyTo(fileStream);
                }

                return ReturnResponce.ListReturnResponce(Path.Combine(uploads + "\\" + fileName));
            }
            else
            {
                return ReturnResponce.ModelIsNotValudResponce();
            }
        }
        /// <summary>
        /// Шинжилгээний хавсралт файл
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("letter")]
        public ResponseClient PostLetterImage(IFormFile uploadedFile)
        {
            var file = uploadedFile;
            Image image = Image.FromStream(file.OpenReadStream(), true, true);
            var uploads = Path.Combine("uploads\\attachedfiles");
            if (file.Length > 0)
            {
                var fileName = UsefulHelpers.GetUniqueFileName(file.FileName.Split('.')[file.FileName.Split('.').Length - 1]);
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    image.Save(fileStream, System.Drawing.Imaging.ImageFormat.Png);
                }
                return ReturnResponce.ListReturnResponce(Path.Combine(uploads + "\\" + fileName));
            }
            else
            {
                return ReturnResponce.ModelIsNotValudResponce();
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("profilepic")]
        public ResponseClient PostProfilePic(IFormFile uploadedFile)
        {
            ResponseClient response = new ResponseClient();
            var file = uploadedFile;
            var  uploads = Path.Combine("uploads\\profilepic");
            if (file.Length > 0)
            {
                var fileName = UsefulHelpers.GetUniqueFileName(file.FileName.Split('.')[file.FileName.Split('.').Length - 1]);
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                return ReturnResponce.ListReturnResponce(Path.Combine(uploads + "\\" + fileName));
            }
            else
            {
                return ReturnResponce.ModelIsNotValudResponce();
            }
        }









    }
}
