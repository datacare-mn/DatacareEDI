using EDIWEBAPI.Attributes;
using EDIWEBAPI.Context;
using EDIWEBAPI.Controllers.SendData;
using EDIWEBAPI.Controllers.SysManagement;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.Payment;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/paymentdata")]
    public class PaymentController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<PaymentController> _log;
        
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public PaymentController(OracleDbContext context, ILogger<PaymentController> log)
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
        /// <param name="comid">Дэлгүүр ID</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="contractno">Гэрээний дугаар</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Амжилттай</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("{comid}/{sdate}/{edate}/{contractno}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> Get(int comid, DateTime sdate, DateTime edate, string contractno)
        {
            var regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
            var companyid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                return Logics.PaymentLogic.GetPayment(_dbContext, _log, comid, companyid, regno, contractno, sdate, edate);
            }
            catch (Exception ex)
            {
                var methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        ///	#Төлбөр тооцооны үндсэн хэсэг
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-14
        /// </remarks>
        /// <param name="comid">Дэлгүүр ID</param>
        /// <param name="year">Жил</param>
        /// <param name="month">Сар</param>
        /// <param name="contractno">Гэрээний дугаар</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getdata/{comid}/{year}/{month}/{contractno}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetData(int comid, int year, int month, string contractno)
        {
            var companyId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var regNo = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
            try
            {
                var sdate = new DateTime(year, month, 1);
                var edate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                return Logics.PaymentLogic.GetPayment(_dbContext, _log, comid, companyId, regNo, contractno, sdate, edate);
            }
            catch (Exception ex)
            {
                var methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        ///	ӨРТӨӨГИЙН ТӨЛБӨР
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Үүсгэсэн огноо : 2017-11-14
        /// </remarks>
        /// <param name="comid">Дэлгүүр ID</param>
        /// <param name="licdate">Огноо</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Амжилттай</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Route("getlicensedetail/{comid}/{licdate}")]
        [Authorize(Policy ="BizApiUser")]
        public ResponseClient GetLicenseDetail(int comid, DateTime licdate)
        {
            var businessid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                var values = Logics.LicenseLogic.GetCompanyUserLicenses(_dbContext, _log, businessid, licdate);
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
            //var licenselist =  _dbContext.SYSTEM_BIZ_LIC_DETAIL.Where(x => x.YEAR == year && x.MONTH == month && x.BIZID == businessid && x.STOREID == comid);
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

  



        /// <summary>
        ///	#Бараа материалын үнэ
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-14
        /// </remarks>
        /// <param name="comid">Дэлгүүр ID</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="regno">Нэвтэрсэн байгууллагын регистер</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <param name="pbgb">payment төрөл</param>

        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("amtdetail/{comid}/{sdate}/{edate}/{ctrcd}/{pbgb}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetAmdDetail(int comid, DateTime sdate, DateTime edate, string ctrcd, int pbgb)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 1)
                {
                    string regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));

                    HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                    if (restUtils.StoreServerConnected)
                    {
                         return await restUtils.Get($"/api/paymentdata/paymenamt{pbgb}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}/{ctrcd}");
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
        /// Торгуулийн дэлгэрэнгүй /Direct special commision хамтдаа ашиглана.../
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-12-01
        /// </remarks>
        /// <param name="comid">Дэлгүүр</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("penamtdetail/{comid}/{sdate}/{edate}/{ctrcd}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetPenAmtDetail(int comid, DateTime sdate, DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 1)
                {
                    string regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));

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
        /// <param name="comid">Дэлгүүр</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("frbtamt/{comid}/{edate}/{ctrcd}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetFrbtAmtDetail(int comid,  DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 1)
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
        /// <param name="comid">Дэлгүүр</param>
        /// <param name="sdate">Эхлэх огноо</param> 
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("ifrbtamt/{comid}/{sdate}/{edate}/{ctrcd}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetIfrbtAmtDetail(int comid, DateTime sdate, DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 1)
                {
                    string regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
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
        /// <param name="comid">Дэлгүүр ID</param>
        /// <param name="edate">Дуусах огноо цикль</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("norstkamt/{comid}/{edate}/{ctrcd}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetNormalStockDetail(int comid, DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 1)
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
        /// <param name="comid">Дэлгүүр ID</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <param name="sdate">Эхлэх огноо цикль</param>
        /// <param name="edate">Дуусах огноо цикль</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("promotionamt/{comid}/{ctrcd}/{sdate}/{edate}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetPromotionDetail(int comid, string ctrcd, DateTime sdate, DateTime edate)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 1)
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
        [Authorize(Policy = "BizApiUser")]
        [Route("crdfeeamt/{comid}/{ctrcd}/{sdate}/{edate}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetCardFeeDetail(int comid, string ctrcd, DateTime sdate, DateTime edate)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 1)
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



        [HttpPut]
        [Authorize(Policy = "BizApiUser")]
        [Route("updatepayment/{comid}/{edate}/{ctrcd}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> UpdatePaymentStoreAPI(int comid, DateTime edate, string ctrcd, string status)
        {
            try
            {
                var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (orgType != 1)
                    return ReturnResponce.AccessDeniedResponce();

                return Logics.PaymentLogic.UpdatePayment(_dbContext, comid, edate, ctrcd, status);
            }
            catch (Exception ex)
            {
                // MethodBase methodBase = MethodBase.GetCurrentMethod();
                // _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }




        #endregion

        #region PaymentApprove

        /// <summary>
        ///	#НЭХЭМЖЛЭХ БАТАЛГААЖУУЛАХ
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-30
        /// </remarks>
        /// <param name="param">List REQ_PAYMENT </param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("approvepayment")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Post([FromBody]List<REQ_PAYMENT> param)
        {
            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            var businessId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var datas = new List<REQ_PAYMENT>();
            try
            {
                // НЭХЭМЖЛЭХ ATTACH ХИЙХЭД APPROVEPAYMENT ЭХЛЭЭД ДУУДАГДААД ХЭРВЭЭ АМЖИЛТТАЙ БОЛВОЛ АРААС НЬ 
                // ATTACHPAYMENTWITHFILE API - Г ШУУД ДУУДДАГ
                foreach (REQ_PAYMENT vdata in param)
                {
                    var existing = Logics.PaymentLogic.GetPayment(_dbContext, vdata.CONTRACTNO, vdata.STPYMD);
                    if (!ModelState.IsValid || existing != null) continue;
                    
                    var curdate = DateTime.ParseExact(vdata.STPYMD, "yyyyMMdd", CultureInfo.InvariantCulture);
                    var result = Logics.PaymentLogic.UpdatePayment(_dbContext, UsefulHelpers.STORE_ID, curdate, vdata.CONTRACTNO, "Y");
                    //  if (!result.Success)
                    //   return ReturnResponce.FailedMessageResponce("Таны үйлдэл амжилтгүй боллоо! Тухайн дэлгүүрийн няглантай холбогдоно уу!");

                    var newPayment = new REQ_PAYMENT()
                    {
                        ID = Convert.ToInt32(Logics.BaseLogic.GetNewId(_dbContext, "REQ_PAYMENT")),
                        STPYMD = vdata.STPYMD,
                        CONTRACTNO = vdata.CONTRACTNO,
                        BUSINESSID = businessId,
                        ATTACHDATE = null,
                        ATTACHFILE = null,
                        DESCRIPTION = null,
                        APPROVEDUSER = userid,
                        APPROVEDDATE = DateTime.Now,
                        STORESEEN = 0,
                        STOREPRINT = 0
                    };

                    datas.Add(newPayment);
                    Logics.BaseLogic.Insert(_dbContext, newPayment, false);
                }

                if (datas.Any())
                    _dbContext.SaveChanges();

                return ReturnResponce.ListReturnResponce(datas.Distinct());
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        /// <summary>
        ///	#Төлбөр тооцоог цуцлах
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-12-01
        /// </remarks>
        /// <param name="contractno">Гэрээний</param>
        /// <param name="stpymd">Дуусах огноо</param>
        /// <param name="reason">Шалтгаан</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpDelete]
        [Route("unapprove")]
        [Authorize(Policy = "StoreApiUser")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Delete([FromBody] PaymentDeleteModel param)
        {
            var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            try
            {
                var current = Logics.PaymentLogic.GetPayment(_dbContext, param.ContractNo, param.Date);
                if (current == null)
                    return ReturnResponce.NotFoundResponce();

                var replacedReason = UsefulHelpers.IsNull(param.Reason) ? string.Empty : param.Reason;   
                current.ATTACHFILE = null;
                current.ATTACHDATE = null;

                Logics.BaseLogic.Update(_dbContext, current, false);

                var disabled = _dbContext.REQ_PAYMENT_DISABLED.FirstOrDefault(a => a.PAYMENTID == current.ID);
                if (disabled == null)
                {
                    Logics.BaseLogic.Insert(_dbContext, 
                        new REQ_PAYMENT_DISABLED()
                        {
                            ID = Convert.ToDecimal(Logics.BaseLogic.GetNewId(_dbContext, "REQ_PAYMENT_DISABLED")),
                            PAYMENTID = current.ID,
                            DESCRIPTION = replacedReason,
                            QUANTITY = 1,
                            DISABLEDBY = userId,
                            DISABLEDDATE = DateTime.Now
                        }, false);
                }
                else
                {
                    disabled.QUANTITY = disabled.QUANTITY + 1;
                    disabled.DISABLEDBY = userId;
                    disabled.DISABLEDDATE = DateTime.Now;

                    Logics.BaseLogic.Update(_dbContext, disabled, false);
                }

                _dbContext.SaveChanges();
                
                var currentuser = Logics.ManagementLogic.GetUser(_dbContext, Convert.ToInt32(current.APPROVEDUSER));
                var currentcompany = Logics.ManagementLogic.GetOrganization(_dbContext, UsefulHelpers.STORE_ID);

                if (currentuser != null && !string.IsNullOrEmpty(currentuser.USERMAIL))
                {
                    Emailer.Send(_dbContext, _log, currentuser.USERMAIL, replacedReason, Enums.SystemEnums.MessageType.Payment,
                        null, null, null, current.CONTRACTNO, currentcompany.COMPANYNAME, current.STPYMD,
                        currentuser.USERMAIL, UsefulHelpers.STORE_ID);
                }

                return ReturnResponce.SuccessMessageResponce("Амжилттай!");
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpGet]
        [Authorize]
        [Route("checkattach/{storeid}/{cycleindex}")]
        public async Task<ResponseClient> CheckAttachment(int storeid, string cycleindex)
        {
            return Logics.PaymentLogic.CheckCycle(_dbContext, _log, storeid, cycleindex, DateTime.Today) ?
                ReturnResponce.SuccessMessageResponce("Батлах боломжтой") :
                ReturnResponce.FailedMessageResponce("Батлах боломжгүй!");
        }




        /// <summary>
        ///	#Нэхэмжлэл батлах 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-12-01
        /// </remarks>
        /// <param name="param">REQ_PAYMENT</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpPut]
        [Authorize]
        [Route("attachpayment")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> PostAttach(IFormFile uploadedFile, string json)
        {
            var logMethod = "PAYMENTCONTROLLER.PostAttach";
            try
            {
                if (!ModelState.IsValid)
                    return ReturnResponce.ModelIsNotValudResponce();

                var param = JsonConvert.DeserializeObject<List<REQ_PAYMENT>>(json);
                var companyName = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyName));

                foreach (REQ_PAYMENT vdata in param)
                {
                    var currentdata = _dbContext.REQ_PAYMENT.FirstOrDefault(x => x.STPYMD == vdata.STPYMD && x.CONTRACTNO == vdata.CONTRACTNO);
                    if (currentdata == null) continue;

                    //var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                    //var currentcontract = _dbContext.MST_CONTRACT.FirstOrDefault(x => x.CONTRACTNO == currentdata.CONTRACTNO);

                    currentdata.ATTACHFILE = vdata.ATTACHFILE;
                    currentdata.DESCRIPTION = vdata.DESCRIPTION;
                    currentdata.ATTACHDATE = DateTime.Now;
                    Logics.BaseLogic.Update(_dbContext, currentdata);
                }
                var contratids = param.Select(x => x.ID);

                string iamgeurl = ConfigData.GetCongifData(ConfigData.ConfigKey.FileServerURL) + param[0].ATTACHFILE;
                string attachmenticon = ConfigData.GetCongifData(ConfigData.ConfigKey.FileServerURL) + "uploads//resource/attachment.png";

                var cal = new CalendarMail()
                {
                    MailAddress = ConfigData.GetCongifData(ConfigData.ConfigKey.StorePaymentMail),
                    MessageData = $"Нэхэмжлэл",
                    Subject = "Нэхэмжлэл " + param[0].CONTRACTNO,
                    Location = "www.urto.mn"
                };
                // cal.Description = SystemResource.MailResource.PaymentTemplate.Replace("#attachurl", iamgeurl).Replace("#CompanyName", $"Байгууллага: <b>{companyName}</b>").Replace("#Contractno", $"Гэрээ №: <b>{param[0].CONTRACTNO}").Replace("#Date", $"Цикль: <b>{param[0].STPYMD}</b>").Replace("#fileurl", cal.FileUrl);
                if (UsefulHelpers.HasImageExtension(iamgeurl))
                {
                    cal.FileUrl = iamgeurl;
                    cal.Description = SystemResource.MailResource.PaymentTemplate.Replace("#attachurl", iamgeurl).Replace("#fileurl", iamgeurl).Replace("#CompanyName", $"Байгууллага: <b>{companyName}</b>").Replace("#Contractno", $"Гэрээ №: <b>{param[0].CONTRACTNO}").Replace("#Date", $"Цикль: <b>{param[0].STPYMD}</b>");
                }
                else
                {
                    cal.FileUrl = attachmenticon;
                    cal.Description = SystemResource.MailResource.PaymentTemplate.Replace("#attachurl", attachmenticon).Replace("#fileurl", iamgeurl).Replace("#CompanyName", $"Байгууллага: <b>{companyName}</b>").Replace("#Contractno", $"Гэрээ №: <b>{param[0].CONTRACTNO}").Replace("#Date", $"Цикль: <b>{param[0].STPYMD}</b>");
                }

                using (var mailController = new MailSendController(_dbContext, null))
                {
                    var response = mailController.SendMail(cal, HttpContext);
                }

                DateTime curdate = DateTime.ParseExact(param[0].STPYMD, "yyyyMMdd", CultureInfo.InvariantCulture);
                //_dbContext.SaveChanges();
                var currentpayments = _dbContext.REQ_PAYMENT.Where(x => contratids.Contains(x.ID));
                return ReturnResponce.ListReturnResponce(currentpayments);
            }
            catch (Exception ex)
            {
                _log.LogError(1, $" {logMethod} ERROR {ex}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


   

        [HttpPut]
        [Authorize]
        [Route("attachpaymentwithfile")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> PostAttachFile(IFormFile uploadedFile, string json)
        {
            var logMethod = "PAYMENTCONTROLLER.PostAttachFile";
            try
            {
                if (!ModelState.IsValid)
                    return ReturnResponce.ModelIsNotValudResponce();

                var param = JsonConvert.DeserializeObject<List<REQ_PAYMENT>>(json);
                var rss = Attacher.File(_log, uploadedFile, "attachedfiles");
                if (!rss.Success)
                    return rss;

                var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                var companyName = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyName));
                
                var data = param.FirstOrDefault().STPYMD;
                var data2 = param.LastOrDefault().STPYMD;

                foreach (REQ_PAYMENT vdata in param)
                {
                    vdata.ATTACHFILE = Convert.ToString(rss.Value);
                    var currentdata = Logics.PaymentLogic.GetPayment(_dbContext, vdata.CONTRACTNO, vdata.STPYMD);
                    if (currentdata == null) continue;

                    currentdata.ATTACHFILE = vdata.ATTACHFILE;
                    currentdata.ATTACHDATE = DateTime.Now;
                    currentdata.DESCRIPTION = $"({data} - {data2}) " + vdata.DESCRIPTION;

                    Logics.BaseLogic.Update(_dbContext, currentdata, false);
                }
                _dbContext.SaveChanges();

                var cal = new CalendarMail()
                {
                    MailAddress = ConfigData.GetCongifData(ConfigData.ConfigKey.StorePaymentMail),
                    MessageData = $"Нэхэмжлэл",
                    Subject = param[0].DESCRIPTION,
                    Location = "www.urto.mn"
                };


                var iamgeurl = ConfigData.GetCongifData(ConfigData.ConfigKey.FileServerURL) + param[0].ATTACHFILE;
                var attachmenticon = ConfigData.GetCongifData(ConfigData.ConfigKey.FileServerURL) + "uploads//resource/attachment.png";

                var mailBody = SystemResource.MailResource.PaymentTemplate
                        .Replace("#fileurl", iamgeurl).Replace("#CompanyName", $"Байгууллага: <b>{companyName}</b>")
                        .Replace("#Contractno", $"Гэрээ №: <b>{param[0].CONTRACTNO}")
                        .Replace("#Date", $"Цикль: <b>{param[0].STPYMD}</b>");

                if (UsefulHelpers.HasImageExtension(iamgeurl))
                {
                    cal.FileUrl = iamgeurl;
                    cal.Description = mailBody.Replace("#attachurl", iamgeurl);
                }
                else
                {
                    cal.FileUrl = attachmenticon;
                    cal.Description = mailBody.Replace("#attachurl", attachmenticon);
                }

                using (var mailController = new MailSendController(_dbContext, null))
                {
                    var response = mailController.SendMail(cal, HttpContext);
                }

                //var contratids = param.Select(x => x.ID);
                //var currentpayments = _dbContext.REQ_PAYMENT.Where(x => contratids.Contains(x.ID));

                var results = (from p in _dbContext.REQ_PAYMENT.ToList()
                               join a in param on p.ID equals a.ID
                               select p).ToList();

                return ReturnResponce.ListReturnResponce(results);
            }
            catch (Exception ex)
            {
                _log.LogError(1, $" {logMethod} ERROR {ex}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }



        #endregion
        


        [HttpGet]
        [AllowAnonymous]
        [Route("testcyclecheck{today}/{cycleindex}")]
        public ResponseClient GetTestCycle(DateTime today, string cycleindex)
        {
            try
            {
                return ReturnResponce.SuccessMessageResponce(Convert.ToString(Logics.PaymentLogic.CheckCycle(_dbContext, _log, 2, cycleindex, today)));
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

    }


    public class LicenseAmount
    {
        public string ContractCode { get; set; }

        public decimal LicenseAMT { get; set; }

        public string  strymd { get; set; }

        public string stpymd { get; set; }
    }

    
}
