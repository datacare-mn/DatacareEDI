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

namespace WebAPI.Controllers.BusinessControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BusinessSkuController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<BusinessSkuController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public BusinessSkuController(OracleDbContext context, ILogger<BusinessSkuController> log)
        {
            _dbContext = context;
            _log = log;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient Get()
        {
            return   ReturnResponce.ListReturnResponce(_dbContext.SKU_BUSINESS.ToList());
        }


        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient Post([FromBody]List<SKU_BUSINESS> param)
        {
            List<SKU_BUSINESS> uData = new List<SKU_BUSINESS>();
            foreach (SKU_BUSINESS vdata in param)
            {
                vdata.BUSINESSSKUID = Convert.ToInt32(_dbContext.GetTableID("SKU_BUSINESS"));
                if (ModelState.IsValid)
                {
                    uData.Add(new SKU_BUSINESS()
                    {
                        BUSINESSSKUID = vdata.BUSINESSSKUID,
                        COMID = vdata.COMID,
                        SKUID = vdata.SKUID,
                        ISOWNER = vdata.ISOWNER,
                        ISACTIVE = vdata.ISACTIVE,
                        BALANCE = vdata.BALANCE
                    });
                }
                else
                    return ReturnResponce.ModelIsNotValudResponce();
            }
            _dbContext.SKU_BUSINESS.AddRange(uData);
            _dbContext.SaveChanges(HttpContext);
            return ReturnResponce.SaveSucessResponce();
        }

        [HttpPut]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient Put([FromBody]SKU_BUSINESS param)
        {
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.SKU_BUSINESS.FirstOrDefault(x => x.BUSINESSSKUID == param.BUSINESSSKUID);
                if (currentdata != null)
                {
                    currentdata.BUSINESSSKUID = param.BUSINESSSKUID;
                    currentdata.COMID = param.COMID;
                    currentdata.SKUID = param.SKUID;
                    currentdata.ISOWNER = param.ISOWNER;
                    currentdata.ISACTIVE = param.ISACTIVE;
                    currentdata.BALANCE = param.BALANCE;
                    _dbContext.Entry(currentdata).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges(HttpContext);
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                    return ReturnResponce.NotFoundResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }





    }
}
