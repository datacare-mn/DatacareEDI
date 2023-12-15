using EDIWEBAPI.Context;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/dasboard")]
    public class DashboardController : Controller
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
        public DashboardController(OracleDbContext context, ILogger<DashboardController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        [HttpGet]
        [Route("routerequest")]
        [AllowAnonymous]
        public ResponseClient GetBrand()
        {
            var currentdata = _dbContext.DASH_MONTH_ROUTEREQUEST().ToList(); ;
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
        [AllowAnonymous]
        public ResponseClient GetLoginRequestDate()
        {
            var currentdata = _dbContext.DASH_MONTH_LOGINREQUESTDATE().ToList(); ;
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
        [Route("loginrequestcountry")]
        [AllowAnonymous]
        public ResponseClient GetLoginRequestCountry()
        {
            var currentdata = _dbContext.DASH_MONTH_LOGINREQUESTCOUNTRY().ToList(); ;
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
        [Route("loginrequestcity")]
        [AllowAnonymous]
        public ResponseClient GetLoginRequestCity()
        {
            var currentdata = _dbContext.DASH_MONTH_LOGINREQUESTCITY().ToList(); ;
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
        [Route("os")]
        [AllowAnonymous]
        public ResponseClient GetLoginRequestOS()
        {
            var currentdata = _dbContext.DASH_MONTH_OS().ToList(); ;
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
        [Route("osversion")]
        [AllowAnonymous]
        public ResponseClient GetLoginRequestOSVersion()
        {
            var currentdata = _dbContext.DASH_MONTH_OS_VERSION().ToList(); ;
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
        [Route("browser")]
        [AllowAnonymous]
        public ResponseClient GetLoginRequestBrowser()
        {
            var currentdata = _dbContext.DASH_MONTH_BROWSER().ToList(); ;
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
        [Route("userlic")]
        [AllowAnonymous]
        public ResponseClient GetUserLicenseInfo()
        {

            var currentdata = _dbContext.DASH_USER_LICINFO().ToList(); ;
            if (currentdata != null)
            {
                return ReturnResponce.ListReturnResponce(currentdata);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        //DASH_LIC_STAT
        [HttpGet]
        [Route("licstat")]
        [AllowAnonymous]
        public ResponseClient GetLicenseInfo()
        {

            var currentdata = _dbContext.DASH_LIC_STAT().ToList(); ;
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
