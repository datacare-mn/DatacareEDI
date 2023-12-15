using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Entities.FilterViews;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/[controller]")]
    public class SystemController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<SystemController> _log;

        public SystemController(OracleDbContext context, ILogger<SystemController> log)
        {
            _dbContext = context;
            _log = log;
        }

        [HttpPost]
        [Route("announcements")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient Get([FromBody] AnnouncementFilterView filter)
        {
            try
            {
                var response = Logics.ManagementLogic.GetAnnouncements(_dbContext, _log, filter);

                return response == null ?
                    ReturnResponce.NotFoundResponce() :
                    ReturnResponce.ListReturnResponce(response);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Route("announcement")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient SetAnnouncement([FromBody] AnnouncementRequestDto dto) // IFormFile uploadedFile, string json
        {
            try
            {
                if (dto == null) //string.IsNullOrEmpty(json)
                    return ReturnResponce.ModelIsNotValudResponce();

                var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
                //var param = JsonConvert.DeserializeObject<AnnouncementRequestDto>(json);

                return Logics.ManagementLogic.SetAnnouncement(_dbContext, _log, null, dto, userid);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        //[HttpPost]
        //[Route("turgargaw/{id}")]
        //[AllowAnonymous]
        //public ResponseClient SetAnnouncement([FromBody] List<string> model, decimal id)
        //{
        //    try
        //    {
        //        if (model == null) //string.IsNullOrEmpty(json)
        //            return ReturnResponce.ModelIsNotValudResponce();
        //        return Logics.ManagementLogic.Turgargaw(_dbContext, _log, model, id);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ReturnResponce.GetExceptionResponce(ex);
        //    }
        //}

        [HttpGet]
        [Route("announcements")]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient GetAnnouncements()
        {
            try
            {
                var comId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                var announcements = Logics.ManagementLogic.GetUserAnnouncements(_dbContext, comId);
                return announcements == null || !announcements.Any() ?
                    ReturnResponce.NotFoundResponce() :
                    ReturnResponce.ListReturnResponce(announcements);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("announcement/{id}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetAnnouncement(decimal id)
        {
            try
            {
                return Logics.ManagementLogic.GetAnnouncement(_dbContext, _log, id);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpDelete]
        [Route("announcement/{id}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient DeleteAnnouncement(decimal id)
        {
            try
            {
                return Logics.ManagementLogic.DeleteAnnouncement(_dbContext, _log, id);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

    }
}
