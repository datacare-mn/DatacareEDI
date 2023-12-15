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
    public class BizStoreController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<BizStoreController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public BizStoreController(OracleDbContext context, ILogger<BizStoreController> log)
        {
            _dbContext = context;
            _log = log;
        }
        /// <summary>
        /// sdas
        /// asd
        /// as
        /// d
        /// ad
        /// as
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ResponseClient Get()
        {
            var list = _dbContext.BIZ_STORE_COMPANY.Select(x => new { x.BIZ_STORE, x.BIZ_COMPANY }).ToList();
          return  ReturnResponce.ListReturnResponce(list);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseClient BusinessStores([FromBody] List<BIZ_STORE_COMPANY> list)
        {
            List<BIZ_STORE_COMPANY> stores = new List<BIZ_STORE_COMPANY>();
            foreach (BIZ_STORE_COMPANY vdata in list)
            {
                vdata.STORECOMID = Convert.ToInt32(_dbContext.GetTableID("BIZ_STORE_COMPANY"));
                if (ModelState.IsValid)
                {
                    stores.Add(new BIZ_STORE_COMPANY() {  COMID = vdata.COMID, STOREID = vdata.STOREID });

                }
                else
                {
                    return ReturnResponce.ModelIsNotValudResponce();
                }
            }

            _dbContext.BIZ_STORE_COMPANY.AddRange(stores);
            _dbContext.SaveChanges(HttpContext);
            return ReturnResponce.SaveSucessResponce();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("AllStores")]
        public ResponseClient GetAllStores()
        {
            var stores =  _dbContext.BIZ_COMPANY.Where(x => x.COMTYPE == 2).ToList();
             return   ReturnResponce.ListReturnResponce(stores);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("AllCompanies")]
        public ResponseClient GetAllBusinesses()
        {
            var stores = _dbContext.BIZ_COMPANY.Where(x => x.COMTYPE == 1).ToList();
            return ReturnResponce.ListReturnResponce(stores);
        }


    }
}
