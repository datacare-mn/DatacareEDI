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
    public class MeetTypeUsersController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MeetTypeUsersController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MeetTypeUsersController(OracleDbContext context, ILogger<MeetTypeUsersController> log)
        {
            _dbContext = context;
            _log = log;
        }

        /// <summary>
        /// Уулзалтын төрөл
        /// </summary>
        /// <returns>Уулзалтын төрлийн лист</returns>
        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> GetMeettypeUsers()
        {
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentMeetTypeUsers = _dbContext.MEET_TYPE_USERS.Where(x => x.STOREID == storeid).SingleOrDefault();
            if (currentMeetTypeUsers != null)
            {
                return ReturnResponce.ListReturnResponce(currentMeetTypeUsers);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }


        [HttpPost]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> InsertMeetClass([FromBody] MEET_TYPE_USERS musers)
        {
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            try
            {

                musers.TYPEUSERID = Convert.ToInt32(_dbContext.GetTableID("MEET_TYPE_USERS"));
                musers.STOREID = storeid;

                if (ModelState.IsValid)
                {
                    _dbContext.MEET_TYPE_USERS.Add(musers);
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
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

    }
}
