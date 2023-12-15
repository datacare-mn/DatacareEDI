using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;
using System.Drawing;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Attributes;

namespace EDIWEBAPI.Controllers.SysManagement
{

    [Route("api/[controller]")]
    public class AttachmentController : Controller
    {

        ILogger _log { get; } =
            ApplicationLogging.CreateLogger<AttachmentController>();    

        [HttpPost]
        [Authorize]
        [Route("file")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient PostFile(IFormFile uploadedFile)
        {            
            return Attacher.File(_log, uploadedFile, "attachedfiles");
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("filetest")]
        //   [ServiceFilter(typeof(LogFilter))]
        public ResponseClient PostFileTest(IFormFile uploadedFile)
        {
            return Attacher.Picture(_log, uploadedFile, "skuimage", null);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("skuimage")]
        [ServiceFilter(typeof(LogFilter))]

        public ResponseClient PostSkuImage(IFormFile uploadedFile)
        {
            return Attacher.Picture(_log, uploadedFile, "skuimage", "skuimage");
        }

        [HttpPost]
        [Authorize]
        [Route("logo")]
        [ServiceFilter(typeof(LogFilter))]

        public ResponseClient PostLogo(IFormFile uploadedFile)
        {
            return Attacher.Picture(_log, uploadedFile, "logos", null);
        }

    }
}
