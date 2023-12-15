using EDIWEBAPI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EDIWEBAPI.Utils
{
    [AttributeUsage(AttributeTargets.All)]
    public class LicenseAttribute : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly OracleDbContext _dbContext;

        public LicenseAttribute(OracleDbContext context, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("LogActionOneFilter");
            _dbContext = context;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(((ControllerBase)context.Controller).HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            
            var  license = _dbContext.SYSTEM_LICENSE.FirstOrDefault(x => x.COMID == comid && x.ENABLED == "0" && x.STARTDATE < DateTime.Now && x.ENDDATE > DateTime.Now);
            if (license != null)
            {
                int licenseid = license.ID;
                List<int> functionid = _dbContext.SYSTEM_LICENSE_DETAIL.Where(x => x.LICENSEID == licenseid).Select(x => x.FUNCTIONID).ToList<int>();
                var userlicensefunctions = _dbContext.SYSTEM_LICENSE_FUNCTION.Where(x => functionid.Contains(x.ID));

                string controllername = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName;
                string routename = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName; ;
                if (userlicensefunctions.Where(x => x.CONTROLLER == controllername && x.ROUTE == routename).ToList().Count == 0)
                {
                    context.HttpContext.Response.StatusCode = 700;
                    context.Result = new JsonResult("Таны багцад хамаарах мэдээлэл биш байна.");
                }
                else
                {
                    base.OnActionExecuting(context);
                }
            }
            else
            {
                context.HttpContext.Response.StatusCode = 700;
                context.Result = new JsonResult("Та лицензээ сунгана уу!");
            }

        }










    }
}
