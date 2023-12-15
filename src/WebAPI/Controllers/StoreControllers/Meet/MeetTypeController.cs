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
    public class MeetTypeController : Controller
    {

        private readonly OracleDbContext _dbContext;
        readonly ILogger<MeetTypeController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MeetTypeController(OracleDbContext context, ILogger<MeetTypeController> log)
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
            var currentClass = _dbContext.MEET_TYPE.Where(x => x.STOREID == storeid).SingleOrDefault();
            if (currentClass != null)
            {
                return ReturnResponce.ListReturnResponce(currentClass);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        /// <summary>
        /// Ангилал бүртгэх
        /// </summary>
        /// <param name="mclass">Ангилал</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> InsertMeetClass([FromBody] MEET_TYPE mtype)
        {
            ResponseClient response = new ResponseClient();
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            try
            {

                mtype.MEETTYPEID = Convert.ToInt32(_dbContext.GetTableID("MEET_TYPE"));
                mtype.STOREID = storeid;

                if (ModelState.IsValid)
                {
                    _dbContext.MEET_TYPE.Add(mtype);
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
