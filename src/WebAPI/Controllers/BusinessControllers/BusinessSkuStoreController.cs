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
    public class BusinessSkuStoreController : Controller
    {
        readonly ILogger<BusinessSkuStoreController> _log;
        private readonly OracleDbContext _dbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public BusinessSkuStoreController(OracleDbContext context, ILogger<BusinessSkuStoreController> log)
        {
            _dbContext = context;
            _log = log;
        }

        [HttpGet]
        [Authorize(Policy ="BizApiUser")]
        public ResponseClient Get()
        {
            return ReturnResponce.ListReturnResponce(_dbContext.BIZ_STORE_SKU.ToList());
        }

        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient Post([FromBody]List<BIZ_STORE_SKU> param)
        {
            List<BIZ_STORE_SKU> uData = new List<BIZ_STORE_SKU>();
            foreach (BIZ_STORE_SKU vdata in param)
            {
                vdata.ID = Convert.ToInt32(_dbContext.GetTableID("BIZ_STORE_SKU"));
                if (ModelState.IsValid)
                {
                    uData.Add(new BIZ_STORE_SKU()
                    {
                        ID = vdata.ID,
                        STOREID = vdata.STOREID,
                        COMID = vdata.COMID,
                        SUPPLYPRICE = vdata.SUPPLYPRICE,
                        BRANCHID = vdata.BRANCHID,
                        SALEPRICE = vdata.SALEPRICE,
                        SKUID = vdata.SKUID,
                    });
                }
                else
                    return ReturnResponce.ModelIsNotValudResponce();
            }
            _dbContext.BIZ_STORE_SKU.AddRange(uData);
            _dbContext.SaveChanges(HttpContext);
            return ReturnResponce.SaveSucessResponce();
        }
    }
}
