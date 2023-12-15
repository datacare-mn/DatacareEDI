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

    public class MeetClassUsersController : Controller
    {

        private readonly OracleDbContext _dbContext;
        readonly ILogger<MeetClassUsersController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MeetClassUsersController(OracleDbContext context, ILogger<MeetClassUsersController> log)
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
        public async Task<ResponseClient> GetMeetType()
        {
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var stroremeettypes = _dbContext.MEET_TYPE.Where(x => x.STOREID == storeid).Select(s=> s.MEETTYPEID).ToArray();
            if (stroremeettypes.Length > 0)
            {
                var currenttypes = _dbContext.MEET_TYPE_USERS.Where(x => stroremeettypes.Contains(x.MEETTYPEID)).ToList();
                if (currenttypes != null)
                {
                    return ReturnResponce.ListReturnResponce(currenttypes);
                }
                else
                {
                    return ReturnResponce.NotFoundResponce();
                }
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> InsertMeetClass([FromBody] MEET_CLASS_USERS user)
        {
            ResponseClient response = new ResponseClient();
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            try
            {

                user.CLASSUSERID = Convert.ToInt32(_dbContext.GetTableID("MEET_CLASS_USERS"));
                if (ModelState.IsValid)
                {
                    _dbContext.MEET_CLASS_USERS.Add(user);
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
