using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StoreAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI.Controllers
{

    [Route("api/[controller]")]
    public class EDIController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<EDIController> _log;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public EDIController(OracleDbContext context, ILogger<EDIController> log)
        {
            _dbContext = context;
            _log = log;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [AllowAnonymous]
     //   [Authorize(Policy = "BIApiUser")]
        public ResponceClient Get(int comid)
        {
            ResponceClient res = new ResponceClient();
            res.Success = true;
            res.Value = _dbContext.BIZ_COM_USER.ToList();
            res.Message = "Амжилттай!";
            return res;
            //  return ReturnResponce.ListReturnResponce(_dbContext.GetSkuList(comid));
        }

    }
}
