using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EDIWEBAPI.Utils;
using EDIWEBAPI.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using EDIWEBAPI.Attributes;
using EDIWEBAPI.Enums;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/[controller]")]
    public class ContractController : Controller
    {

        private readonly OracleDbContext _dbContext;
        readonly ILogger<ContractController> _log;
        private string dboOwner = "edisystem";

        #region Initialize
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
        #endregion


        [HttpGet]
        [Authorize]
        [Route("contracts/{orgId}/{orgType}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetOrganizationContracts(int orgId, int orgType)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var contracts = _dbContext.GET_ORGANIZATION_CONTRACTS(orgId, orgType);

            return contracts != null ?
                ReturnResponce.ListReturnResponce(contracts) :
                ReturnResponce.NotFoundResponce();
        }

        [HttpPost]
        [Authorize]
        [Route("add/{userId}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> AddUserContracts([FromBody]int[] contractIds, int userId)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var result = _dbContext.ADD_USER_CONTRACTS(contractIds, userId);
            return result == 0 ?
                ReturnResponce.NotFoundResponce() :
                ReturnResponce.ListReturnResponce(result);
        }

        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("modify")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> Modify()
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            string regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyReg));
            return Logics.ContractLogic.Modify(_dbContext, _log, UsefulHelpers.STORE_ID, regno);
        }

        [HttpGet]
        [Authorize]
        [Route("contracts")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> Contracts()
        {
            var orgId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));

            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var contracts = _dbContext.GET_CONTRACTS_ORG_ID(orgId);

            return contracts != null ?
                ReturnResponce.ListReturnResponce(contracts) :
                ReturnResponce.NotFoundResponce();
        }
    }
}
