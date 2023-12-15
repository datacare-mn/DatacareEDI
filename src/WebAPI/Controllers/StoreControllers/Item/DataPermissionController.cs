using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.CustomModels;
using WebAPI.Models.Entities;
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Controllers.StoreControllers.Item
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DataPermissionController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<DataPermissionController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public DataPermissionController(OracleDbContext context, ILogger<DataPermissionController> log)
        {
            _dbContext = context;
            _log = log;
        }

        /// <summary>
        /// Дата эрхийн тохиргооны хэсэг дэлгүүр талын хэрэглэгч хандана.
        /// хэрэв тухайн дэлгүүрийн datepermission = 1 бол гэрээгээр
        /// datepermission = 0 бол барааны ангилалаар тохруулагдана 
        /// үүнийг шалгаад тохирох ангилалыг буцааж байгаа хэсэг
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetStoreInfo()
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentStore = _dbContext.BIZ_COMPANY.Where(x => x.COMID == comid && x.COMTYPE == 1).SingleOrDefault();
            if (currentStore != null)
            {
                if (currentStore.DATAPERMISSION == 1)
                {
                    var data = _dbContext.BIZ_CONTRACT.Where(x => x.COMID == comid);
                    return ReturnResponce.ListReturnResponce(data.ToList());

                }
                else
                {
                    var data = _dbContext.BIZ_DEPART.Where(x => x.COMID == comid);
                    return ReturnResponce.ListReturnResponce(data.ToList());
                }
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }


        [HttpPost]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient InsertDataPermission([FromBody] List<DataPermissionValue> pdata)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentStore = _dbContext.BIZ_COMPANY.Where(x => x.COMID == comid && x.COMTYPE == 1).SingleOrDefault();
            if (currentStore.DATAPERMISSION == 1)
            {
                List<BIZ_USER_CONT> uCont = new List<BIZ_USER_CONT>();
                foreach (DataPermissionValue vdata in pdata)
                {


                    uCont.Add(new BIZ_USER_CONT() { USERID = vdata.UserID, CONTRACTID = vdata.PermissionTypeID, USERCONTID = Convert.ToInt32(_dbContext.GetTableID("BIZ_USER_CONT")) });
                }
                _dbContext.BIZ_USER_CONT.AddRange(uCont);

                _dbContext.SaveChanges(HttpContext);
            }
            else
            {
                List<BIZ_USER_DEPART> uDep = new List<BIZ_USER_DEPART>();
                foreach (DataPermissionValue vdata in pdata)
                {

                    uDep.Add(new BIZ_USER_DEPART() { USERID = vdata.UserID, DEPARTID = vdata.PermissionTypeID, USERDEPID = Convert.ToInt32(_dbContext.GetTableID("BIZ_USER_DEPART")) });
                }
                _dbContext.BIZ_USER_DEPART.AddRange(uDep);
                _dbContext.SaveChanges(HttpContext);
            }
            return ReturnResponce.SaveSucessResponce();
        }




    }
}
