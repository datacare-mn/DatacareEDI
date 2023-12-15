using EDIWEBAPI.Attributes;
using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/paymentstoredata")]

    public class PaymentStoreController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<PaymentStoreController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public PaymentStoreController(OracleDbContext context, ILogger<PaymentStoreController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        #region Get
        /// <summary>
        ///	#Төлбөр тооцооны үндсэн хэсэг
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-14
        /// </remarks>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="regno">Нэвтэрсэн байгууллагын регистер</param>
        /// <param name="contractno">Гэрээ</param>
        /// <param name="approved">Батлагдсан эсэх</param>
        /// <param name="attached">Нэхэмжлэгдсэн эсэх</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("{regno}/{sdate}/{edate}/{contractno}/{approved}/{attached}")]
        public async Task<ResponseClient> Get(string regno, DateTime sdate, DateTime edate, string contractno, bool approved, bool attached)
        {
            try
            {
                edate = edate.AddDays(1).AddSeconds(-1);
                var storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));

                return Logics.PaymentLogic.GetStorePayment(_dbContext, _log, storeid, regno, contractno, sdate, edate, attached, approved);
            }
            catch (Exception ex)
            {
                var methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));

                Logics.ManagementLogic.ExceptionLog(_dbContext, _log, HttpContext, 
                    new string[] { $"regno : {regno}", $"sdate : {sdate}", $"edate : {edate}", $"contractno : {contractno}", $"approved : {approved}", $"attached : {attached}" }, 
                    "PaymentStore", "Get", ex);

                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        /// <summary>
        ///	#Дэлгүүрийн хэрэглэгч нэмэхжлэлийг хэвлэх үед энэ төлвийг өөрчлөх ёстой! 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-03-05
        /// </remarks>
        /// <param name="contractno">Гэрээ №</param>
        /// <param name="stpymd">stpymd /20180101/</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("printstatuschange/{contractno}/{stpymd}")]
        public async Task<ResponseClient> PrintStatusChange(string contractno, string stpymd)
        {
            if (contractno != null && stpymd != null)
            {
                var currentpayment = _dbContext.REQ_PAYMENT.FirstOrDefault(x => x.CONTRACTNO == contractno && x.STPYMD == stpymd);
                if (currentpayment != null)
                {
                    currentpayment.STOREPRINT = 1;
                }

                _dbContext.Entry(currentpayment).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges();
                return ReturnResponce.ListReturnResponce(currentpayment);
            }
            return ReturnResponce.NotFoundResponce();
        }



        /// <summary>
        ///	#Бараа материалын үнэ
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-14
        /// </remarks>
        /// <param name="regno">Рег №</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="regno">Нэвтэрсэн байгууллагын регистер</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <param name="pbgb">payment төрөл</param>

        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("amtdetail/{regno}/{sdate}/{edate}/{ctrcd}/{pbgb}")]
        public async Task<ResponseClient> GetAmdDetail(string regno, DateTime sdate, DateTime edate, string ctrcd, int pbgb)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype != 2)
                    return ReturnResponce.AccessDeniedResponce();

                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                if (regno == "null")
                {
                    var currentbusiness = Logics.ContractLogic.GetContract(_dbContext, ctrcd);
                    if (currentbusiness != null)
                        regno = Logics.ManagementLogic.GetOrganization(_dbContext, currentbusiness.BUSINESSID.Value).REGNO;
                }

                var restUtils = new HttpRestUtils(comid, _dbContext);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

                return await restUtils.Get($"/api/paymentdata/paymenamt{pbgb}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}/{ctrcd}");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        /// <summary>
        /// Торгуулийн дэлгэрэнгүй /Direct special commision хамтдаа ашиглана.../
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-12-01
        /// </remarks>
        /// <param name="regno">Рег №</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("penamtdetail/{regno}/{sdate}/{edate}/{ctrcd}")]
        public async Task<ResponseClient> GetPenAmtDetail(string regno, DateTime sdate, DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 2)
                {
                    int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));

                    HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                    if (restUtils.StoreServerConnected)
                    {
                        return await restUtils.Get($"/api/paymentdata/penamt/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}/{ctrcd}");
                    }
                    else
                        return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
                }
                else
                    return ReturnResponce.AccessDeniedResponce();
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);

            }
        }


        /// <summary>
        ///	#Гэрээний нэмэлт дүн /Direct, Special, Commission дундаа ашиглана/
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-12-01
        /// </remarks>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("frbtamt/{edate}/{ctrcd}")]
        public async Task<ResponseClient> GetFrbtAmtDetail(DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                if (usercomtype == 2)
                {
                    HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                    if (restUtils.StoreServerConnected)
                    {
                        return await restUtils.Get($"/api/paymentdata/frbtamt/{edate.ToString("yyyy-MM-dd")}/{ctrcd}");
                    }
                    else
                        return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
                }
                else
                    return ReturnResponce.AccessDeniedResponce();
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }



        /// <summary>
        ///	#Үйл ажиллагааны нэмэлт дүн /direct, special, commision дундаа ашиглана/
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-12-01
        /// </remarks>
        /// <param name="sdate">Дуусах огноо</param> 
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("ifrbtamt/{sdate}/{edate}/{ctrcd}")]
        public async Task<ResponseClient> GetIfrbtAmtDetail(DateTime sdate, DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                if (usercomtype == 2)
                {
                    HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                    if (restUtils.StoreServerConnected)
                    {
                        return await restUtils.Get($"/api/paymentdata/ifrbtamt/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{ctrcd}");
                    }
                    else
                        return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
                }
                else
                    return ReturnResponce.AccessDeniedResponce();
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        /// <summary>
        ///	#Нормаль сток
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-12-06
        /// </remarks>
        /// <param name="edate">Дуусах огноо цикль</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize]
        [Route("norstkamt/{edate}/{ctrcd}")]
        public async Task<ResponseClient> GetNormalStockDetail(DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                if (usercomtype == 2)
                {
                    HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                    if (restUtils.StoreServerConnected)
                    {
                        return await restUtils.Get($"/api/paymentdata/norstkamt/{edate.ToString("yyyy-MM-dd")}/{ctrcd}");
                    }
                    else
                        return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
                }
                else
                    return ReturnResponce.AccessDeniedResponce();
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        ///	#Нормаль сток
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-12-06
        /// </remarks>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <param name="sdate">Эхлэх огноо цикль</param>
        /// <param name="edate">Дуусах огноо цикль</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("promotionamt/{ctrcd}/{sdate}/{edate}")]
        public async Task<ResponseClient> GetPromotionDetail(string ctrcd, DateTime sdate, DateTime edate)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                if (usercomtype == 2)
                {
                    HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                    if (restUtils.StoreServerConnected)
                    {
                        return await restUtils.Get($"/api/paymentdata/promotionamt/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{ctrcd}");
                    }
                    else
                        return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
                }
                else
                    return ReturnResponce.AccessDeniedResponce();
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }





        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("crdfeeamt/{ctrcd}/{sdate}/{edate}")]
        public async Task<ResponseClient> GetCardFeeDetail(string ctrcd, DateTime sdate, DateTime edate)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));

                if (usercomtype == 2)
                {
                    HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                    if (restUtils.StoreServerConnected)
                    {
                        return await restUtils.Get($"/api/paymentdata/crdfeeamt/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{ctrcd}");
                    }
                    else
                        return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
                }
                else
                    return ReturnResponce.AccessDeniedResponce();
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("getlicensedetail/{regno}/{licdate}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetLicenseDetail(string regno, DateTime licdate)
        {
            var organization = Logics.ManagementLogic.GetOrganization(_dbContext, regno);
            if (organization == null)
                return ReturnResponce.NotFoundResponce();

            try
            {
                var values = Logics.LicenseLogic.GetCompanyUserLicenses(_dbContext, _log, organization.ID, licdate);
                return ReturnResponce.ListReturnResponce(values);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }

            //DateTime sdate = new DateTime(licdate.Year, licdate.Month, 1);
            //DateTime edate = sdate.AddMonths(1).AddSeconds(-1);
            //string month = sdate.ToString("MM");
            //int year = sdate.Year;
            //var licenselist = _dbContext.SYSTEM_BIZ_LIC_DETAIL.Where(x => x.YEAR == year && x.MONTH == month && x.BIZID == organization.ID && x.STOREID == 2);
            //var query = from lic in licenselist.ToList()
            //            join users in _dbContext.SYSTEM_USERS
            //            on lic.USERID equals users.ID
            //            join roles in _dbContext.SYSTEM_ROLES
            //            on lic.ROLEID equals roles.ID
            //            select new
            //            {
            //                users.ID,
            //                users.LASTNAME,
            //                users.FIRSTNAME,
            //                roles.ROLENAME,
            //                lic.LOGCOUNT,
            //                lic.SCORE,
            //                lic.DAYCOUNT,
            //                lic.LICPRICE
            //            };

            //return ReturnResponce.ListReturnResponce(query);
        }

        #endregion

    }

}




