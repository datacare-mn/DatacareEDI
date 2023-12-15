using EDIWEBAPI.Context;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/dashboardsystem")]
    public class DashboardSystemController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<DashboardController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public DashboardSystemController(OracleDbContext context, ILogger<DashboardController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion


        #region 

        #endregion
        [HttpGet]
        [Route("countstat")]
        [Authorize]
        public ResponseClient GetCountStat()
        {
            var currentdata = _dbContext.DASH_SYSTEM_COUNTDATA().ToList(); 
            if (currentdata != null)
            {
                return ReturnResponce.ListReturnResponce(currentdata);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        [HttpGet]
        [Route("functionusage")]
        [Authorize]
        public ResponseClient GetFunctionUsage()
        {
            var currentdata = _dbContext.DASH_SYSTEM_FUNCTION_USAGE().ToList(); 
            if (currentdata != null)
            {
                return ReturnResponce.ListReturnResponce(currentdata);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        //DASH_SYSTEM_LOGINRATE_MAP


        [HttpGet]
        [Route("loginrate")]
        [Authorize]
        public ResponseClient GetLoginRateMap()
        {
            var currentdata = _dbContext.DASH_SYSTEM_LOGINRATE_MAP().ToList(); ;
            if (currentdata != null)
            {
                return ReturnResponce.ListReturnResponce(currentdata);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }



        //DASH_TOP10_LOGINCOMPANY

        [HttpGet]
        [Route("toplogincompany")]
        [Authorize]
        public ResponseClient GetTopLoginCompany()
        {
            var currentdata = _dbContext.DASH_TOP10_LOGINCOMPANY().ToList();
            if (currentdata != null)
            {
                return ReturnResponce.ListReturnResponce(currentdata);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        //DASH_TOP10_REQUESTCOMPANY
        [HttpGet]
        [Route("toprequestcompany")]
        [Authorize]
        public ResponseClient GetTopRequestCompany()
        {
            var currentdata = _dbContext.DASH_TOP10_REQUESTCOMPANY().ToList();
            if (currentdata != null)
            {
                return ReturnResponce.ListReturnResponce(currentdata);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        //DASH_OS_STAT
        [HttpGet]
        [Route("osstat")]
        [Authorize]
        public ResponseClient GetOSStat()
        {
            var currentdata = _dbContext.DASH_OS_STAT().ToList();
            if (currentdata != null)
            {
                return ReturnResponce.ListReturnResponce(currentdata);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        //DASH_BROWSER_STAT
        [HttpGet]
        [Route("browserstat")]
        [Authorize]
        public ResponseClient GetBrowserStat()
        {
            var currentdata = _dbContext.DASH_BROWSER_STAT().ToList(); ;
            if (currentdata != null)
            {
                return ReturnResponce.ListReturnResponce(currentdata);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }



        [HttpGet]
        [Route("loginrequestdate")]
        [Authorize]
        public ResponseClient GetLoginRequestDateSystem()
        {
            var currentdata = _dbContext.DASH_MONTH_LOGINREQUESTDATE().ToList();
            if (currentdata != null)
            {
                return ReturnResponce.ListReturnResponce(currentdata);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

    }
}
