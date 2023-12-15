using EDIWEBAPI.Context;
using EDIWEBAPI.Controllers.SendData;
using EDIWEBAPI.Entities.License;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Attributes;
using static EDIWEBAPI.Enums.SystemEnums;
using EDIWEBAPI.Entities.RequestModels;

namespace EDIWEBAPI.Controllers.License
{
    [Route("api/license")]
    public class LicenseController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<LicenseController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public LicenseController(OracleDbContext context, ILogger<LicenseController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion


        [HttpGet]
        [Authorize]
        [Route("getContract")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetContract()
        {
            try
            {
                var contract = Logics.LicenseLogic.GetContract(_dbContext, UsefulHelpers.STORE_ID, DateTime.Today);
                if (contract == null)
                    return ReturnResponce.NotFoundResponce();

                return ReturnResponce.ListReturnResponce(contract);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        #region ХЭРЭГЛЭЭНИЙ ТОХИРГОО

        [HttpGet]
        [Authorize]
        [Route("getStoreLicenses/{value}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetStoreLicenses(DateTime value)
        {
            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            try
            {
                if ((ORGTYPE)orgType == ORGTYPE.Бизнес)
                    return ReturnResponce.AccessDeniedResponce();
                
                return ReturnResponce.ListReturnResponce(Logics.LicenseLogic.GetStoreLicenses(_dbContext, _log, UsefulHelpers.STORE_ID, value, true));
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "EdiApiUser")]
        [Route("License")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> SetLicenseAmount([FromBody] LicenseChangeRequest request)
        {
            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            return Logics.LicenseLogic.ChangeLicense(_dbContext, _log, request, userid);
        }

        [HttpGet]
        [Authorize]
        [Route("LicenseLog/{businessId}/{value}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetLicenseLog(decimal businessId, DateTime value)
        {
            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            try
            {
                if ((ORGTYPE)orgType == ORGTYPE.Бизнес)
                    return ReturnResponce.AccessDeniedResponce();

                return Logics.LicenseLogic.GetLicenseLogs(_dbContext, _log, businessId, value);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getLastLicense")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetLastLicense()
        {
            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            try
            {
                var license = Logics.LicenseLogic.GetLastLicense(_dbContext, userid);
                return license == null ? 
                    ReturnResponce.NotFoundResponce() : 
                    ReturnResponce.ListReturnResponce(license);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("addMonthlyLicense")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> AddMonthlyLicense()
        {
            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            try
            {
                Logics.LicenseLogic.CheckMonthFees(_dbContext, _log, comid, userid, DateTime.Today);
                return ReturnResponce.SuccessMessageResponce("Амжилттай");
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getCompanyLicenses/{date}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetCompanyLicenses(DateTime date)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            try
            {
                return ReturnResponce.ListReturnResponce(Logics.LicenseLogic.GetCompanyLicenses(_dbContext, comid, date));
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getLicense/{date}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetLicense(DateTime date)
        {
            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            try
            {
                return ReturnResponce.ListReturnResponce(Logics.LicenseLogic.GetUserLicense(_dbContext, _log, userid, date));
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPut]
        [Authorize(Policy = "BizApiUser")]
        [Route("setLicense")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> SetLicense(string json, DateTime date)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var licenses = JsonConvert.DeserializeObject<List<Entities.RequestModels.UserLicenseRequest>>(json);
            if (licenses == null || !licenses.Any())
                return ReturnResponce.NotFoundResponce();

            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));

            try
            {
                Logics.LicenseLogic.SetUserLicenses(_dbContext, licenses, comid, userid, date);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        #endregion

        [HttpGet]
        [Route("packages")]
        [Authorize]
        public ResponseClient GetPackage()
        {
            try
            {
                List<PackageList> packageList = new List<PackageList>();
                foreach (SYSTEM_LICENSE_PACKAGE pack in _dbContext.SYSTEM_LICENSE_PACKAGE)
                {
                    PackageList pckg = new PackageList();
                    pckg.package = pack;
                    var pkcgfunctions = _dbContext.SYSTEM_LICENSE_PACK_FUNC.Where(x => x.PACKAGEID == pack.PACKAGEID).Select(x=> x.FUNCTIONID);
                    pckg.functions = _dbContext.SYSTEM_LICENSE_FUNCTION.Where(x => pkcgfunctions.Contains(x.ID)).ToList();
                    packageList.Add(pckg);
                }
                return ReturnResponce.ListReturnResponce(packageList);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpPost]
        [Route("createlicense/{mail}")]
        [Authorize]
        public Task<ResponseClient> CreateLicense([FromBody] LicenseData licenseData, string mail)
        {
            try
            {
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                licenseData.license.COMID = comid;
                licenseData.license.CREATEUSER = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
                licenseData.license.CREATEDDATE = DateTime.Now;
                licenseData.license.ENABLED = "0";
                licenseData.license.UPDATEUSER = null;
                licenseData.license.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_LICENSE"));
                licenseData.license.LICENSEKEY = GetLicenseKey();
                var data = licenseData;
                List<SYSTEM_LICENSE_DETAIL> licenseDetail = new List<SYSTEM_LICENSE_DETAIL>();
                foreach (int id in licenseData.funcionid)
                {
                    SYSTEM_LICENSE_DETAIL detail = new SYSTEM_LICENSE_DETAIL();
                    detail.FUNCTIONID =id;
                    detail.ID = _dbContext.GetTableID("SYSTEM_LICENSE_DETAIL");
                    detail.LICENSEID = licenseData.license.ID;
                    licenseDetail.Add(detail);
                }

                _dbContext.Database.BeginTransaction();
                _dbContext.SYSTEM_LICENSE.Add(licenseData.license);
                _dbContext.SYSTEM_LICENSE_DETAIL.AddRange(licenseDetail);
                _dbContext.SaveChanges();
                _dbContext.Database.CurrentTransaction.Commit();
                
                var req = GetQpayData(licenseData.license.LICENSEKEY, Convert.ToDecimal(licenseData.license.PRICE));
                using (var mailcontroller = new MailSendController(_dbContext, null))
                    mailcontroller.SendLicenseRequest(mail, comid, licenseData.license.LICENSEKEY, licenseData.license.PRICE, req.qrcode);

                return Task.FromResult(ReturnResponce.ListReturnResponce(req));
            }
            catch (Exception ex)
            {
                _dbContext.Database.CurrentTransaction.Rollback();
                return Task.FromResult(ReturnResponce.GetExceptionResponce(ex));
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="statementdata">Банк статемент сонсдог хэсэг</param>
        /// <returns></returns>
        [HttpPost]
        [Route("chargemoney")]
        [AllowAnonymous]
        public Task<ResponseClient> UpdateLicense([FromBody] BankStatement statementdata)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = statementdata;
                 var currentlicense =   _dbContext.SYSTEM_LICENSE.FirstOrDefault(x => x.ENABLED == "0" && x.LICENSEKEY == statementdata.DESCVALUE && x.PRICE == statementdata.CREDIT);
                    if (currentlicense != null)
                    {
                        DateTime sdate = Convert.ToDateTime(currentlicense.STARTDATE);
                        DateTime edate = Convert.ToDateTime(currentlicense.ENDDATE);
                        int totaldays = Convert.ToInt32(edate.Subtract(sdate).TotalDays);
                        currentlicense.STARTDATE = DateTime.Today;
                        currentlicense.ENDDATE = DateTime.Today.AddDays(totaldays);
                        currentlicense.ENABLED = "1";
                        _dbContext.Entry(currentlicense).State = System.Data.Entity.EntityState.Modified;
                        _dbContext.SaveChanges();

                    }
                    else
                    {
                        SYSTEM_BANKREQLIST bl = new SYSTEM_BANKREQLIST();
                        bl.ID =Convert.ToInt32(statementdata.STATEMENTID);
                        bl.CREDIT = statementdata.CREDIT;
                        bl.DEBIT = statementdata.DEBIT;
                        bl.TRANSACDATE = statementdata.TRANSACDATE;
                        bl.DESCRIPTION = statementdata.DESCRIPTION;
                        bl.DESCVALUE = statementdata.DESCVALUE;
                        bl.ACCOUNTNO = statementdata.ACCOUNTNO;
                        bl.ACCOUNTNAME = statementdata.ACCOUNTNAME;
                        _dbContext.Entry(bl).State = System.Data.Entity.EntityState.Added;
                        _dbContext.SaveChanges();
                    }
                    return Task.FromResult(ReturnResponce.SuccessMessageResponce("Амжилттай"));
                }
                else
                    return Task.FromResult(ReturnResponce.NotFoundResponce());
            }
            catch (Exception ex)
            {
                _dbContext.Database.CurrentTransaction.Rollback();
                return Task.FromResult(ReturnResponce.GetExceptionResponce(ex));
            }
        }


        private QPayData GetQpayData(string description, decimal amount)
        {
            var rstapp = new SystemRestAppUtils(_dbContext, _log, "DCPAYMENT", UsefulHelpers.STORE_ID);
            if (rstapp.ServerConnected)
            {
                //    return rstapp.Get($"/api/Accounting/getcustbaldetailed/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}").Result;
                QPayResult qr = new QPayResult();

                string value = rstapp.GetQR(_log, $"/api/qpay/qrgenerate/{amount}/{description}").Result.ToString();

                qr = JsonConvert.DeserializeObject<QPayResult>(value);
                QPayData qdata = new QPayData();
                qdata.licenseKey = description;
                qdata.qrcode = qr.json_data.qPay_QRcode;
                qdata.qrimage = qr.json_data.qPay_QRimage;
                qdata.price = amount;
                return qdata;
            }

            return null;


        }

        private string GetQrCodeData(string qrimage)
        {
            Image img = UsefulHelpers.Base64ToImage(qrimage);
            Image stampedimage =  UsefulHelpers.StampQRImage(img);
            return UsefulHelpers.ImageToBase64(stampedimage, ImageFormat.Png);

        }





        private string GetLicenseKey()
        {
            string key = Cryptography.CreateLicenseKey();
            if (_dbContext.SYSTEM_LICENSE.FirstOrDefault(x => x.LICENSEKEY == key) == null)
            {
                return key;
            }
             return GetLicenseKey();
        }

    }
}
