using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using EDIWEBAPI.Utils.UserAgent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace EDIWEBAPI.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class LogFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly OracleDbContext _dbContext;

        public LogFilter(OracleDbContext context, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("LogActionOneFilter");
            _dbContext = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                //var usermail = Convert.ToString(UsefulHelpers.GetIdendityValue(((ControllerBase)context.Controller).HttpContext, Enums.SystemEnums.UserProperties.UserMail));
                var arg = context.ActionArguments.ToList();
                var json = JsonConvert.SerializeObject(arg);

                var log = new SYSTEM_USER_ACTION_LOG()
                {
                    ID = Convert.ToString(Guid.NewGuid()),
                    IPADDRESS = Convert.ToString(context.HttpContext.Connection.RemoteIpAddress),
                    LOGDATE = DateTime.Now,
                    USERID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(((ControllerBase)context.Controller).HttpContext, Enums.SystemEnums.UserProperties.UserId)),
                    COMID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(((ControllerBase)context.Controller).HttpContext, Enums.SystemEnums.UserProperties.CompanyId)),
                    CONTROLLER = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName,
                    ROUTE = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName,
                    ARGUMENT = GetJson(json)
                };

                _dbContext.SYSTEM_USER_ACTION_LOG.Add(log);
                _dbContext.SaveChanges();

                base.OnActionExecuting(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"LogFilter.OnActionExecuting : {UsefulHelpers.GetExceptionMessage(ex)}");
            }
        }

        private string GetJson(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Length > 2000 ? value.Substring(0, 2000) : value;
        }
        
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            try
            {
                var user_data = ((Microsoft.AspNetCore.Server.Kestrel.Internal.Http.FrameRequestHeaders)context.HttpContext.Request.Headers).HeaderUserAgent.ToString();
                var data = (ResponseClient)(((ObjectResult)context.Result).Value);
                var ua = new UserAgent(user_data);
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(((ControllerBase)context.Controller).HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                int userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(((ControllerBase)context.Controller).HttpContext, Enums.SystemEnums.UserProperties.UserId));

                var parameters = new List<string>();
                for (int index = 0; index < context.RouteData.Values.Count(); index++)
                {
                    var key = context.RouteData.Values.Keys.ToArray()[index];
                    if (key == "action" || key == "controller") continue;
                    parameters.Add($"{key} : {context.RouteData.Values.Values.ToArray()[index]}");
                }

                var json = JsonConvert.SerializeObject(parameters);

                var actionLog = new SYSTEM_REQUEST_ACTION_LOG()
                {
                    ID = Guid.NewGuid().ToString(),
                    OSNAME = ua.OS.Name,
                    OSVERSION = ua.OS.Version,
                    BROWSER = ua.Browser.Name,
                    BROWSERVERSION = ua.Browser.Version,
                    COMID = comid,
                    USERID = userid,
                    ROUTE = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName,
                    CONTROLLER = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName,
                    REQUESTDATA = JsonConvert.SerializeObject(data.Value).Length,
                    REQUESTDATE = DateTime.Now,
                    REQUESTYEARMONTH = DateTime.Now.ToString("yyyyMM"),
                    PARAMETER = GetJson(json),
                    SUCCESS = (byte) (data.Success ? 1 : 0),
                    MESSAGE = data.Message
                };

                _dbContext.Entry(actionLog).State = System.Data.Entity.EntityState.Added;
                _dbContext.SaveChanges();

                base.OnResultExecuted(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"LogFilter.OnResultExecuted : {UsefulHelpers.GetExceptionMessage(ex)}");
                base.OnResultExecuted(context);
            }
        }


    }
}
     