using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EDIWEBAPI.Attributes;
using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Entities.DBModel.Product;
using EDIWEBAPI.Entities.DBModel.MasterSku;
using EDIWEBAPI.Entities.DBModel.Feedback;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/[controller]")]
    public class FeedbackController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<FeedbackController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public FeedbackController(OracleDbContext context, ILogger<FeedbackController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        [HttpGet]
        [Route("gettypes")]
        [Authorize]
        public ResponseClient GetTypes()
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var values = _dbContext.MST_FEEDBACK_TYPE.Where(a => a.ENABLED == 1).Select(a => a).OrderBy(a => a.VIEWORDER).ToList();
            return ReturnResponce.ListReturnResponce(values);
        }

        [HttpPost]
        [Route("add")]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient Add([FromBody] FeedbackDto param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                var orgRequest = _dbContext.MST_PRODUCT_REQUEST.FirstOrDefault(a => a.CODE == "FEEDBACK");
                if (orgRequest == null)
                    return ReturnResponce.NotFoundResponce();

                var request = new REQ_FEEDBACK()
                {
                    DEPARTMENTID = param.TYPEID,
                    SUBJECT = param.SUBJECT,
                    REQUESTID = orgRequest.ID,
                    STOREID = UsefulHelpers.STORE_ID,
                    ORGID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId)),
                    REQUESTBY = userId,
                    ATTACHMENT = 0
                };

                return Logics.MasterLogic.AddRequest<REQ_FEEDBACK, REQ_FEEDBACK_IMAGE, REQ_FEEDBACK_LOG>(_dbContext, _log, request, new List<IFormFile>(), userId, param.NOTE);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("get/{id}")]
        [Authorize]
        public ResponseClient Get(int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));

            return Logics.MasterLogic.GetRequest<REQ_FEEDBACK, REQ_FEEDBACK_LOG>(_dbContext, _log, id, orgType, userId);
        }

        [HttpPost]
        [Route("getrequests")]
        [Authorize]
        public ResponseClient GetRequests([FromBody] Entities.FilterViews.ProductRequestFilterView filter)
        {
            try
            {
                var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
                Entities.ResultModels.FeedbackListModel response = null;
                if (orgType == (int)ORGTYPE.Бизнес)
                {
                    filter.ORGID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
                    response = Logics.MasterLogic.GetFeedbacks(_dbContext, _log, filter);
                }
                else if (orgType == (int)ORGTYPE.Дэлгүүр)
                {
                    filter.STOREID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
                    filter.USERID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                    response = Logics.MasterLogic.GetStoreFeedbacks(_dbContext, _log, filter);
                }

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
        [Route("addnote")]
        [Authorize]
        public ResponseClient AddNote([FromBody] RequestNoteDto param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));

            return Logics.MasterLogic.AddNote<REQ_FEEDBACK, REQ_FEEDBACK_LOG>(_dbContext, _log, param, 
                orgType, NotifcationType.Санал, userId);
        }

        [HttpGet]
        [Route("requeststatus/{id}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetRequestStatuses(int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            return Logics.MasterLogic.GetStatuses<REQ_FEEDBACK>(_dbContext, _log, id);
        }

        [HttpPost]
        [Route("status")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient ChangeStatus([FromBody] RequestNoteDto param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                var newStatus = Logics.MasterLogic.GetStatus(_dbContext, _log, param.Status);

                Logics.MasterLogic.ChangeStatus<REQ_FEEDBACK, REQ_FEEDBACK_LOG>(_dbContext, _log, 
                    param.Id, param.Note, userId, newStatus);

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
    }
}
