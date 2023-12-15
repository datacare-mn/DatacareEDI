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
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Controllers.BusinessControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ContractTypeController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<ContractTypeController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public ContractTypeController(OracleDbContext context, ILogger<ContractTypeController> log)
        {
            _dbContext = context;
            _log = log;
        }
        /// <summary>
        /// Гэрээний төрөл
        /// </summary>
        /// <returns>Гэрээний төрөл</returns>
        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> GetAll()
        {
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentContracttype = _dbContext.BIZ_CONTRACT_TYPE.Where(x => x.STOREID == storeid);
            if (currentContracttype != null)
            {
                return ReturnResponce.ListReturnResponce(currentContracttype.ToList());
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        /// <summary>
        /// Гэрээний төрөл
        /// </summary>
        /// <returns>Гэрээний төрөл</returns>
        [HttpPost]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> Post([FromBody]BIZ_CONTRACT_TYPE contracttype)
        {
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            contracttype.CONTRACTTYPEID =Convert.ToInt32(_dbContext.GetTableID("BIZ_CONTRACT_TYPE"));
            contracttype.STOREID = storeid;
            _dbContext.BIZ_CONTRACT_TYPE.Add(contracttype);
            _dbContext.SaveChanges(HttpContext);
            return ReturnResponce.SaveSucessResponce();
            
        }
    }
}
