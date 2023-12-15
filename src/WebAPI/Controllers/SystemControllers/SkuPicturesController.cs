using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.Entities;

namespace WebAPI.Controllers.SystemControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SkuPicturesController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<SkuPicturesController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public SkuPicturesController(OracleDbContext context, ILogger<SkuPicturesController> log)
        {
            _dbContext = context;
            _log = log;
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("Get/{skuid}")]
        public ResponseClient Get(int skuid)
        {
            return ReturnResponce.ListReturnResponce(_dbContext.SYS_SKU_PUCTURES.Where(x=> x.SKUID == skuid).ToList());
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseClient Post([FromBody]List<SYS_SKU_PUCTURES> pictures )
        {
            try
            {

                List<SYS_SKU_PUCTURES> upics = new List<SYS_SKU_PUCTURES>();
                foreach (SYS_SKU_PUCTURES pic in pictures)
                {
                    pic.PICTUREID = Convert.ToInt32(_dbContext.GetTableID("SYS_SKU_PUCTURES"));
                    if (ModelState.IsValid)
                    {
                        upics.Add(new SYS_SKU_PUCTURES() { PICTUREID = pic.PICTUREID, PICURL = pic.PICURL, SKUID = pic.SKUID, LETTERIMAGE = pic.LETTERIMAGE });
                    }
                    else
                    {
                        return ReturnResponce.ModelIsNotValudResponce();
                    }
                }
                _dbContext.SYS_SKU_PUCTURES.AddRange(upics);
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
              //  _log.LogError(ex.ToString(), null);
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }



    }
}
