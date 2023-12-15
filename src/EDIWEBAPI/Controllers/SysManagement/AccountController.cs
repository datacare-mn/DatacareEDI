using EDIWEBAPI.Context;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/accountdata")]
    public class AccountController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<AccountController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public AccountController(OracleDbContext context, ILogger<AccountController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion


        [HttpGet]
        [AllowAnonymous]
        [Route("licensepayment/{sdate}/{edate}")]
        public async Task<ResponseClient> GetLicenseData(string sdate, string edate)
        {
            try
            {
                var currentdata = _dbContext.SYSTEM_ACCOUNT_LICDATA(sdate, edate);
                if (currentdata != null)
                {
                    return ReturnResponce.ListReturnResponce(currentdata);
                }
                return ReturnResponce.NotFoundResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.NotFoundResponce();
            }
        }
    }
}
