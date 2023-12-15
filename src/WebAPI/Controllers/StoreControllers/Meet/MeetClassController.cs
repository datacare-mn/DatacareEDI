using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.CustomModels;
using WebAPI.Models.Entities;
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Controllers.StoreControllers.Meet
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MeetClassController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MeetClassController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MeetClassController(OracleDbContext context, ILogger<MeetClassController> log)
        {
            _dbContext = context;
            _log = log;
        }



        /// <summary>
        /// Уулзалтын ангилал
        /// </summary>
        /// <returns>Ангилалын лист</returns>
        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> GetMeetClass()
        {
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentClass = _dbContext.MEET_CLASS.Where(x => x.STOREID == storeid).SingleOrDefault();
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
        public async Task<ResponseClient> InsertMeetClass([FromBody] MEET_CLASS mclass)
        {
            var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            try
            {

                mclass.CLASSID = Convert.ToInt32(_dbContext.GetTableID("MEET_CLASS"));
                mclass.STOREID = storeid;
               
                if (ModelState.IsValid)
                {
                    _dbContext.MEET_CLASS.Add(mclass);
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

        /// <summary>
        /// Ангилал бүртгэх
        /// </summary>
        /// <param name="mclass">Ангилал</param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> UpdateMeetClass([FromBody]MEET_CLASS mclass)
        {
            ResponseClient response = new ResponseClient();
            try
            {
                if (ModelState.IsValid)
                {
                    _dbContext.Entry(mclass).State = System.Data.Entity.EntityState.Modified;
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
