using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Controllers.StoreControllers.Meet
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MeetScheduleRequestController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MeetScheduleRequestController> _log;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MeetScheduleRequestController(OracleDbContext context, ILogger<MeetScheduleRequestController> log)
        {
            _dbContext = context;
            _log = log;
        }


        /// <summary>
        /// Уулзалтын файлууд
        /// </summary>
        /// <returns>Уулзалтын хүсэлтийн файлууд</returns>
        [HttpGet]
        [Authorize]
        public async Task<ResponseClient> Get(int reqid)
        {
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentfiles = _dbContext.MEET_SCHEDULE_REQ.Select(x => x.MEETTYPEID == reqid);
            if (currentfiles != null)
            {
               return ReturnResponce.ListReturnResponce(currentfiles);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }




    }

}
