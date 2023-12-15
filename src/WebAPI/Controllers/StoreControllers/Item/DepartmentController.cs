using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.Entities;
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Controllers.StoreControllers.Item
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DepartmentController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<DepartmentController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public DepartmentController(OracleDbContext context, ILogger<DepartmentController> log)
        {
            _dbContext = context;
            _log = log;
        }

        [HttpPost]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient InsertDepart([FromBody]BIZ_DEPART depart)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    depart.DEPID = Convert.ToInt32(_dbContext.GetTableID("BIZ_DEPART"));
                    _dbContext.BIZ_DEPART.Add(depart);
                    _dbContext.SaveChanges(HttpContext);
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                {
                    return ReturnResponce.ModelIsNotValudResponce();
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }

        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]

        public async Task<ResponseClient> Get()
        {
            try
            {
                var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
                var departs = _dbContext.BIZ_DEPART.Where(x => x.COMID == storeid);
                return ReturnResponce.ListReturnResponce(departs.ToList());
                
            }
            catch (Exception ex)
            {
                MethodBase methodbase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, $"{methodbase.DeclaringType}.{methodbase.Name}");
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }

    }




    
}
