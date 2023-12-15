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

namespace WebAPI.Controllers.StoreControllers.Meet
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MeetBusinessDays : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MeetBusinessDays> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MeetBusinessDays(OracleDbContext context, ILogger<MeetBusinessDays> log)
        {
            _dbContext = context;
            _log = log;
        }



        /// <summary>
        /// Уулзалтын ажлын өдрүүд
        /// </summary>
        /// <returns>Уулзалтын төрлийн лист</returns>
        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> GetBusinessDays()
        {
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentBusinessDays = _dbContext.MEET_BUSINESS_DAYS.Where(x => x.STORID == storeid).SingleOrDefault();
            if (currentBusinessDays != null)
            {
                return ReturnResponce.ListReturnResponce(currentBusinessDays);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        /// <summary>
        /// Уулзалтын ажлын өдрүүд
        /// </summary>
        /// <returns>Уулзалтын өдрийн лист</returns>
        [HttpPost]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> InsertBusinessDays([FromBody]MEET_BUSINESS_DAYS BusinessDays)
        {
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var day = _dbContext.MEET_BUSINESS_DAYS.FirstOrDefault(x => x.STORID == storeid && x.DAYINDEX == BusinessDays.DAYINDEX);
            if (day == null)
            {
                BusinessDays.DAYID = Convert.ToInt32(_dbContext.GetTableID("MEET_BUSINESS_DAYS"));
                BusinessDays.STORID = storeid;
                _dbContext.MEET_BUSINESS_DAYS.Add(BusinessDays);
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
            {
                return ReturnResponce.SaveFailureResponce();
            }
        }



    }
}
