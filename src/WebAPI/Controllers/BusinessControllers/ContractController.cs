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

namespace WebAPI.Controllers.BusinessControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ContractController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<ContractController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public ContractController(OracleDbContext context, ILogger<ContractController> log)
        {
            _dbContext = context;
            _log = log;
        }



        /// <summary>
        /// Дэлгүүр талын өөрийн бүртгэсэн гэрээ 
        /// </summary>
        /// <returns>Гэрээнүүд</returns>
        [HttpGet]
        [Route("AllContract")]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> GetAll()
        {
            try
            {
                var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
                var currentContracttype = _dbContext.BIZ_CONTRACT.Where(x => x.STOREID == storeid);
                if (currentContracttype != null)
                {
                    return ReturnResponce.ListReturnResponce(currentContracttype.ToList());
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

        /// <summary>
        /// Дэлгүүр талаас бүртгэгдсэн гэрээг бизнес талд харуулах хэсэг
        /// </summary>
        /// <returns>Гэрээнүүд</returns>
        [HttpGet]
        [Route("BizAllContract")]
        [Authorize(Policy = "BizApiUser")]
        public async Task<ResponseClient> BizContractGetAll()
        {
            try
            {
                var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
                var currentContracttype = _dbContext.BIZ_CONTRACT.Where(x => x.COMID == comid);
                if (currentContracttype != null)
                {
                    return ReturnResponce.ListReturnResponce(currentContracttype.ToList());
                }
                else
                {
                    return ReturnResponce.NotFoundResponce();
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        /// Дэлгүүр талын гэрээ бүртгэх хэсэг
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient InsertDepart([FromBody]BIZ_CONTRACT contract)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    contract.CONTRACTID = Convert.ToInt32(_dbContext.GetTableID("BIZ_CONTRACT"));
                    _dbContext.BIZ_CONTRACT.Add(contract);
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

    }
}
