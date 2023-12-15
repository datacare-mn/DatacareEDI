using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/paymentsystemdata")]
    public class PaymentSystemController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<PaymentSystemController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public PaymentSystemController(OracleDbContext context, ILogger<PaymentSystemController> log)
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
        /// <param name="regno">RegNo</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="contractno">Гэрээ</param>
        /// <param name="approved">Батлагдсан эсэх</param>
        /// <param name="attached">Нэхэмжлэгдсэн эсэх</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>





        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("{comid}/{regno}/{sdate}/{edate}/{contractno}/{approved}/{attached}")]
        public async Task<ResponseClient> Get(int comid, string regno, DateTime sdate, DateTime edate, string contractno, bool approved, bool attached)
        {
            try
            {
                int storeid = comid;
                if (contractno == "null")
                {
                    contractno = "%";
                }
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                HttpRestUtils restUtils = new HttpRestUtils(storeid, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<PaymentHeader> PaymentHeader = new List<PaymentHeader>();
                    string jsonData = Convert.ToString(restUtils.Get($"/api/paymentdata/paymentheader/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}").Result.Value);
                    PaymentHeader = JsonConvert.DeserializeObject<List<PaymentHeader>>(jsonData);

                    if (PaymentHeader.Count > 0)
                    {
                        if (attached && approved)
                        {
                            var query = (from header in PaymentHeader.ToList()
                                         join request in (from pymt in _dbContext.REQ_PAYMENT.ToList()
                                                          join users in _dbContext.SYSTEM_USERS.ToList()
                                                          on pymt.APPROVEDUSER equals users?.ID into su
                                                          from appuser in su.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              pymt.APPROVEDDATE,
                                                              pymt.APPROVEDUSER,
                                                              pymt.ATTACHFILE,
                                                              pymt.CONTRACTNO,
                                                              pymt.DESCRIPTION,
                                                              pymt.STORESEEN,
                                                              pymt.ID,
                                                              pymt.ATTACHDATE,
                                                              appuser?.FIRSTNAME,
                                                              pymt.STPYMD
                                                          }
                                                         )
                                         on new { stopdate = header.stpymd, contractno = header.ctrcd }
                                         equals new { stopdate = request.STPYMD, contractno = request.CONTRACTNO } into t
                                         from rt in t.DefaultIfEmpty()
                                         select new
                                         {
                                             header.amt,
                                             header.ctrcd,
                                             header.ctrnm,
                                             header.edi,
                                             header.evnamt,
                                             header.frbtamt,
                                             header.ifrbtamt,
                                             header.normalstk,
                                             header.payamt,
                                             header.PBGB,
                                             header.stpymd,
                                             header.strymd,
                                             header.banknm,
                                             header.accno,
                                             header.penamt,
                                             header.crdfee,
                                             header.invamt,
                                             rt?.APPROVEDDATE,
                                             APPROVEDUSER = rt?.FIRSTNAME,
                                             rt?.ATTACHFILE,
                                             rt?.CONTRACTNO,
                                             rt?.DESCRIPTION,
                                             rt?.STORESEEN,
                                             rt?.ID,
                                             rt?.ATTACHDATE
                                         }).Where(x => (x.ctrcd == contractno || contractno == "%") && x.ATTACHDATE != null).OrderBy(x => x.ctrcd).ThenBy(x => x.stpymd);
                            return ReturnResponce.ListReturnResponce(query);
                        }
                        if (approved && !attached)
                        {
                            var query = (from header in PaymentHeader.ToList()
                                         join request in (from pymt in _dbContext.REQ_PAYMENT.ToList()
                                                          join users in _dbContext.SYSTEM_USERS.ToList()
                                                          on pymt.APPROVEDUSER equals users?.ID into su
                                                          from appuser in su.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              pymt.APPROVEDDATE,
                                                              pymt.APPROVEDUSER,
                                                              pymt.ATTACHFILE,
                                                              pymt.CONTRACTNO,
                                                              pymt.DESCRIPTION,
                                                              pymt.STORESEEN,
                                                              pymt.ID,
                                                              pymt.ATTACHDATE,
                                                              appuser?.FIRSTNAME,
                                                              pymt.STPYMD
                                                          }
                                                         )
                                         on new { stopdate = header.stpymd, contractno = header.ctrcd }
                                         equals new { stopdate = request.STPYMD, contractno = request.CONTRACTNO } into t
                                         from rt in t.DefaultIfEmpty()
                                         select new
                                         {
                                             header.amt,
                                             header.ctrcd,
                                             header.ctrnm,
                                             header.edi,
                                             header.evnamt,
                                             header.frbtamt,
                                             header.ifrbtamt,
                                             header.normalstk,
                                             header.payamt,
                                             header.PBGB,
                                             header.stpymd,
                                             header.strymd,
                                             header.banknm,
                                             header.accno,
                                             header.penamt,
                                             header.crdfee,
                                             header.invamt,
                                             rt?.APPROVEDDATE,
                                             APPROVEDUSER = rt?.FIRSTNAME,
                                             rt?.ATTACHFILE,
                                             rt?.CONTRACTNO,
                                             rt?.DESCRIPTION,
                                             rt?.STORESEEN,
                                             rt?.ID,
                                             rt?.ATTACHDATE
                                         }).Where(x => (x.ctrcd == contractno || contractno == "%") && (x.CONTRACTNO != null && x.APPROVEDDATE != null)).OrderBy(x => x.ctrcd).ThenBy(x => x.stpymd);
                            return ReturnResponce.ListReturnResponce(query);
                        }
                        else
                        {
                            var query = (from header in PaymentHeader.ToList()
                                         join request in (from pymt in _dbContext.REQ_PAYMENT.ToList()
                                                          join users in _dbContext.SYSTEM_USERS.ToList()
                                                          on pymt.APPROVEDUSER equals users?.ID into su
                                                          from appuser in su.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              pymt.APPROVEDDATE,
                                                              pymt.APPROVEDUSER,
                                                              pymt.ATTACHFILE,
                                                              pymt.CONTRACTNO,
                                                              pymt.DESCRIPTION,
                                                              pymt.STORESEEN,
                                                              pymt.ID,
                                                              pymt.ATTACHDATE,
                                                              appuser?.FIRSTNAME,
                                                              pymt.STPYMD
                                                          }
                                                         )
                                         on new { stopdate = header.stpymd, contractno = header.ctrcd }
                                         equals new { stopdate = request.STPYMD, contractno = request.CONTRACTNO } into t
                                         from rt in t.DefaultIfEmpty()
                                         select new
                                         {
                                             header.amt,
                                             header.ctrcd,
                                             header.ctrnm,
                                             header.edi,
                                             header.evnamt,
                                             header.frbtamt,
                                             header.ifrbtamt,
                                             header.normalstk,
                                             header.payamt,
                                             header.PBGB,
                                             header.stpymd,
                                             header.strymd,
                                             header.banknm,
                                             header.accno,
                                             header.penamt,
                                             header.crdfee,
                                             header.invamt,
                                             rt?.APPROVEDDATE,
                                             APPROVEDUSER = rt?.FIRSTNAME,
                                             rt?.ATTACHFILE,
                                             rt?.CONTRACTNO,
                                             rt?.DESCRIPTION,
                                             rt?.STORESEEN,
                                             rt?.ID,
                                             rt?.ATTACHDATE
                                         }).Where(x => x.ctrcd == contractno || contractno == "%").OrderBy(x => x.ctrcd).ThenBy(x => x.stpymd);
                            return ReturnResponce.ListReturnResponce(query);
                        }




                    }
                    else
                        return ReturnResponce.NotFoundResponce();
                }
                else
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);

            }
            //try
            //{
            //    int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
            //    if (usercomtype == 3)
            //    {
            //        HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
            //        if (restUtils.StoreServerConnected)
            //        {
            //            List<PaymentHeader> PaymentHeader = new List<PaymentHeader>();
            //            string jsonData = Convert.ToString(restUtils.Get($"/api/paymentdata/paymentheader/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}").Result.Value);
            //            PaymentHeader = JsonConvert.DeserializeObject<List<PaymentHeader>>(jsonData);

            //            if (PaymentHeader.Count > 0)
            //            {
            //                var query = (from header in PaymentHeader.ToList()
            //                             join request in (from pymt in _dbContext.REQ_PAYMENT.ToList()
            //                                              join users in _dbContext.SYSTEM_USERS.ToList()
            //                                              on pymt.APPROVEDUSER equals users?.ID into su
            //                                              from appuser in su.DefaultIfEmpty()
            //                                              select new
            //                                              {
            //                                                  pymt.APPROVEDDATE,
            //                                                  pymt.APPROVEDUSER,
            //                                                  pymt.ATTACHFILE,
            //                                                  pymt.CONTRACTNO,
            //                                                  pymt.DESCRIPTION,
            //                                                  pymt.STORESEEN,
            //                                                  pymt.ID,
            //                                                  pymt.ATTACHDATE,
            //                                                  appuser?.FIRSTNAME,
            //                                                  pymt.STPYMD
            //                                              }
            //                                                 )
            //                             on new { stopdate = header.stpymd, contractno = header.ctrcd }
            //                             equals new { stopdate = request.STPYMD, contractno = request.CONTRACTNO } into t
            //                             from rt in t.DefaultIfEmpty()
            //                             select new
            //                             {
            //                                 header.amt,
            //                                 header.ctrcd,
            //                                 header.ctrnm,
            //                                 header.edi,
            //                                 header.evnamt,
            //                                 header.frbtamt,
            //                                 header.ifrbtamt,
            //                                 header.normalstk,
            //                                 header.payamt,
            //                                 header.PBGB,
            //                                 header.stpymd,
            //                                 header.strymd,
            //                                 header.banknm,
            //                                 header.accno,
            //                                 header.penamt,
            //                                 header.crdfee,
            //                                 rt?.APPROVEDDATE,
            //                                 APPROVEDUSER = rt?.FIRSTNAME,
            //                                 rt?.ATTACHFILE,
            //                                 rt?.CONTRACTNO,
            //                                 rt?.DESCRIPTION,
            //                                 rt?.STORESEEN,
            //                                 rt?.ID,
            //                                 rt?.ATTACHDATE
            //                             }).OrderBy(x => x.ctrcd).ThenBy(x => x.stpymd);
            //                return ReturnResponce.ListReturnResponce(query);
            //            }
            //            else
            //                return ReturnResponce.NotFoundResponce();
            //        }
            //        else
            //            return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            //    }
            //    else
            //        return ReturnResponce.AccessDeniedResponce();
            //}
            //catch (Exception ex)
            //{
            //    MethodBase methodBase = MethodBase.GetCurrentMethod();
            //    _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
            //    return ReturnResponce.GetExceptionResponce(ex);

            //}
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
        /// <param name="regno">regNo</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <param name="pbgb">payment төрөл</param>

        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("amtdetail/{comid}/{regno}/{sdate}/{edate}/{ctrcd}/{pbgb}")]
        public async Task<ResponseClient> GetAmdDetail(int comid, string regno, DateTime sdate, DateTime edate, string ctrcd, int pbgb)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 3)
                {
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
        [Authorize(Policy = "EdiApiUser")]
        [Route("penamtdetail/{comid}/{regno}/{sdate}/{edate}/{ctrcd}")]
        public async Task<ResponseClient> GetPenAmtDetail(int comid, DateTime sdate, DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 3)
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
        [Authorize(Policy = "EdiApiUser")]
        [Route("frbtamt/{comid}/{edate}/{ctrcd}")]
        public async Task<ResponseClient> GetFrbtAmtDetail(int comid, DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 3)
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
        /// <param name="sdate">Дуусах огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="ctrcd">Гэрээ №</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("ifrbtamt/{comid}/{sdate}/{edate}/{ctrcd}")]
        public async Task<ResponseClient> GetIfrbtAmtDetail(int comid, DateTime sdate, DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 3)
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
        [Authorize(Policy = "EdiApiUser")]
        [Route("norstkamt/{comid}/{edate}/{ctrcd}")]
        public async Task<ResponseClient> GetNormalStockDetail(int comid, DateTime edate, string ctrcd)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 3)
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
        [Authorize(Policy = "EdiApiUser")]
        [Route("promotionamt/{comid}/{ctrcd}/{sdate}/{edate}")]
        public async Task<ResponseClient> GetPromotionDetail(int comid, string ctrcd, DateTime sdate, DateTime edate)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 3)
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
        [Authorize(Policy = "EdiApiUser")]
        [Route("crdfeeamt/{comid}/{ctrcd}/{sdate}/{edate}")]
        public async Task<ResponseClient> GetCardFeeDetail(int comid, string ctrcd, DateTime sdate, DateTime edate)
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                if (usercomtype == 3)
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

        #endregion
    }
}
