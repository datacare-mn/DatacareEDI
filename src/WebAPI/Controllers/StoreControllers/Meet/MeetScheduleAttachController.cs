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
    public class MeetScheduleAttachController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MeetScheduleAttachController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MeetScheduleAttachController(OracleDbContext context, ILogger<MeetScheduleAttachController> log)
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
        [Route("{reqid}")]
        public async Task<ResponseClient> Get(int reqid)
        {
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentfiles = _dbContext.MEET_SCHEREQ_ATTACH.Select(x => x.REQID == reqid);
            if (currentfiles != null)
            {
                return ReturnResponce.ListReturnResponce(currentfiles);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        /// <summary>
        /// Уулзалтын хавсралт файл хадгалах
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ResponseClient> Post([FromBody]MEET_SCHEREQ_ATTACH data)
        {
            ResponseClient response = new ResponseClient();
            data.ATACHID = Convert.ToInt32(_dbContext.GetTableID("MEET_SCHEREQ_ATTACH"));
            if (ModelState.IsValid)
            {
                _dbContext.MEET_SCHEREQ_ATTACH.Add(data);
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
            {
                return ReturnResponce.ModelIsNotValudResponce();
            }
        }


    }
}
