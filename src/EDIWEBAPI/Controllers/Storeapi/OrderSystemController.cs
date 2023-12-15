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
    public class OrderSystemController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<OrderSystemController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public OrderSystemController(OracleDbContext context, ILogger<OrderSystemController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion


        /// <summary>
        ///	#Захиалгын хураангуй /Дэлгүүр/
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-01-16
        /// </remarks>
        /// <param name="comid">Дэлгүүрийн ID</param>
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="skucd">Бар код</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("orderheader/{comid}/{branchcode}/{contractno}/{sdate}/{skucd}/{businessid}")]


        public async Task<ResponseClient> GetOrderHeader(int comid, string branchcode, string contractno, DateTime sdate, string skucd, int businessid)
        {
            try
            {
                string contractlist = "";
                if (skucd == "null")
                {
                    skucd = "%";
                }
                if (branchcode == "null")
                {
                    branchcode = "%";
                }
                if (contractno == "null")
                {
                    contractno = "%";
                    contractlist = DbUtils.GetSystemContractList(HttpContext, _dbContext, comid, businessid);
                }
                HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<OrderHeader> OrderHeader = new List<OrderHeader>();
                    string jsonData = "";
                    if (contractlist.Length > 0)
                    {
                        jsonData = Convert.ToString(restUtils.Post($"/api/orderdata/orderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{sdate.ToString("yyyy-MM-dd")}/{skucd}", contractlist).Result.Value);
                    }
                    else
                    {
                        jsonData = Convert.ToString(restUtils.Post($"/api/orderdata/orderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{sdate.ToString("yyyy-MM-dd")}/{skucd}", "").Result.Value);
                    }
                    OrderHeader = JsonConvert.DeserializeObject<List<OrderHeader>>(jsonData);

                    if (OrderHeader.Count > 0)
                    {
                        var query = (from header in OrderHeader.ToList()
                                     join request in (from ordr in _dbContext.REQ_ORDER.ToList()
                                                      join users in _dbContext.SYSTEM_USERS.ToList()
                                                      on ordr.APPROVEDUSER equals users?.ID into au
                                                      join seeuser in _dbContext.SYSTEM_USERS.ToList()
                                                      on ordr.SEENUSER equals seeuser?.ID into su
                                                      from appuser in au.DefaultIfEmpty()
                                                      from seenuser in su.DefaultIfEmpty()
                                                      select new
                                                      {
                                                          ordr.APPROVEDDATE,
                                                          ordr.APPROVEDUSER,
                                                          ordr.CONTRACTNO,
                                                          ordr.ISSEEN,
                                                          ordr.SEENDATE,
                                                          ordr.SEENUSER,
                                                          appuser?.FIRSTNAME,
                                                          ordr.ORDERDATE,
                                                          ordr.ORDERNO,
                                                          seenuser = seenuser?.FIRSTNAME
                                                      }
                                                      )
                                     on new { stopdate = header.ordilja, orderno = header.ordno, contractno = header.ctrcd }
                                     equals new { stopdate = request.ORDERDATE, orderno = request.ORDERNO, contractno = request.CONTRACTNO } into t
                                     from rt in t.DefaultIfEmpty()
                                     select new
                                     {
                                         header.ordilja,
                                         header.ordno,
                                        storecd = header.jumcd,
                                         header.ctrcd,
                                         header.ctrnm,
                                         header.eta,
                                         header.ord_supply,
                                         header.ord_supplyvat,
                                         header.ord_supplyamt,
                                         header.skucnt,
                                         rt?.ISSEEN,
                                         rt?.SEENDATE,
                                         SEENUSER = rt?.seenuser,
                                         rt?.APPROVEDDATE,
                                         APPROVEDUSER = rt?.FIRSTNAME
                                     }).Where(x => x.ctrcd == contractno || contractno == "%").OrderBy(x => x.ctrcd).ThenBy(x => x.ordilja);
                        return ReturnResponce.ListReturnResponce(query.Distinct());
                    }
                    return ReturnResponce.NotFoundResponce();
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }


        /// <summary>
        ///	#Захиалгын дэлгэрэнгүй /Дэлгүүр/ 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-01-16
        /// </remarks>
        /// <param name="comid">Дэлгүүрийн ID</param>
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="orderdate">Захиалгын огноо</param>
        /// <param name="orderno">Захиалгын дугаар</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("orderdetail/{comid}/{branchcode}/{orderdate}/{orderno}")]

        public async Task<ResponseClient> GetOrderDetail(int comid, string branchcode, string orderdate, string orderno)
        {
            try
            {
                return Logics.OrderLogic.GetOrderDetail(_dbContext, _log, branchcode, orderdate, orderno);
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        /// <summary>
        ///	#Бизнес талаас Буцаалтын үндсэн хэсэн
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="comid">Дэлгүүр №</param>
        /// <param name="branchcode">Салбар №</param>
        /// <param name="contractno">Гэрээ №</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="skucd">Баркод</param>
        /// <param name="businessid">Байгууллага ID</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        #region Return
        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("returnorderheader/{comid}/{branchcode}/{contractno}/{sdate}/{edate}/{skucd}/{businessid}")]

        public async Task<ResponseClient> GetReturnOrderHeader(int comid, string branchcode, string contractno, DateTime sdate, DateTime edate, string skucd, int businessid)
        {
            try
            {
                string contractlist = "";
                if (branchcode == "null")
                {
                    branchcode = "%";
                }
                if (contractno == "null")
                {
                    contractno = "%";
                    contractlist = DbUtils.GetSystemContractList(HttpContext, _dbContext, comid, businessid);
                }
                if (skucd == "null")
                {
                    skucd = "%";
                }
                HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<ReturnOrderHeader> OrderHeader = new List<ReturnOrderHeader>();
                    string jsonData = "";
                    if (contractlist.Length > 0)
                    {
                        // jsonData = Convert.ToString(restUtils.Post($"/api/orderdata/orderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{skucd}", contractlist).Result.Value);
                        jsonData = Convert.ToString(restUtils.Post($"/api/orderdata/returnorderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{skucd}", contractlist).Result.Value);
                    }
                    else
                    {
                        //jsonData = Convert.ToString(restUtils.Post($"/api/orderdata/orderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{skucd}", "").Result.Value);
                        jsonData = Convert.ToString(restUtils.Post($"/api/orderdata/returnorderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{skucd}", "").Result.Value);
                    }
                    OrderHeader = JsonConvert.DeserializeObject<List<ReturnOrderHeader>>(jsonData);
                    if (OrderHeader.Count > 0)
                    {
                        var query = (from header in OrderHeader.ToList()
                                     join request in (from ordr in _dbContext.REQ_RETURN_ORDER.ToList()
                                                      join users in _dbContext.SYSTEM_USERS.ToList()
                                                      on ordr.APPROVEDUSER equals users?.ID into au
                                                      join seeuser in _dbContext.SYSTEM_USERS.ToList()
                                                      on ordr.SEENUSER equals seeuser?.ID into su
                                                      from appuser in au.DefaultIfEmpty()
                                                      from seenuser in su.DefaultIfEmpty()
                                                      select new
                                                      {
                                                          ordr.APPROVEDDATE,
                                                          ordr.APPROVEDUSER,
                                                          ordr.CONTRACTNO,
                                                          ordr.ISSEEN,
                                                          ordr.SEENDATE,
                                                          ordr.SEENUSER,
                                                          appuser?.FIRSTNAME,
                                                          ordr.ORDERDATE,
                                                          ordr.ORDERNO,
                                                          seenuser = seenuser?.FIRSTNAME
                                                      }
                                                      )
                                     on new { stopdate = header.ilja, orderno = header.retno, contractno = header.ctrcd }
                                     equals new { stopdate = request.ORDERDATE, orderno = request.ORDERNO, contractno = request.CONTRACTNO } into t
                                     from rt in t.DefaultIfEmpty()
                                     select new
                                     {
                                         header.ilja,
                                         header.retno,
                                         storecd =  header.jumcd,
                                         header.ctrcd,
                                         header.ctrnm,
                                         header.qty,
                                         header.supply,
                                         header.supplyvat,
                                         header.supplyamt,
                                         header.notifinfo,
                                         header.sndymd,
                                         rt?.ISSEEN,
                                         rt?.SEENDATE,
                                         SEENUSER = rt?.seenuser,
                                         rt?.APPROVEDDATE,
                                         APPROVEDUSER = rt?.FIRSTNAME
                                     }).Where(x => x.ctrcd == contractno || contractno == "%").OrderBy(x => x.ctrcd).ThenBy(x => x.retno);
                        return ReturnResponce.ListReturnResponce(query.Distinct());
                    }
                    return ReturnResponce.NotFoundResponce();

                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }



        /// <summary>
        ///	#Систем талаас буцаалтын задаргаа
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="comid">Дэлгүүр№</param>
        /// <param name="branchcode">Салбар №</param>
        /// <param name="orderdate">Захиалгын огноо</param>
        /// <param name="orderno">Захиалгын №</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("returnorderdetail/{comid}/{branchcode}/{orderdate}/{orderno}")]

        public async Task<ResponseClient> GetReturnOrderDetail(int comid, string branchcode, string orderdate, string orderno)
        {
            try
            {
                return Logics.OrderLogic.GetReturnDetail(_dbContext, _log, branchcode, orderdate, orderno).Result;
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
