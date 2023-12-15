using EDIWEBAPI.Context;
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
using EDIWEBAPI.Entities.APIModel;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/storeorderdata")]
    public class OrderStoreController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<OrderStoreController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public OrderStoreController(OracleDbContext context, ILogger<OrderStoreController> log)
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
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="skucd">Бар код</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("orderheader/{branchcode}/{contractno}/{sdate}/{skucd}")]

        public async Task<ResponseClient> GetOrderHeader(string branchcode, string contractno, DateTime sdate, string skucd)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                return Logics.OrderLogic.GetOrderHeader(_dbContext, comid, 0, branchcode, contractno, sdate, sdate, skucd);
            }
            catch (Exception ex)
            {
                var methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


       [HttpGet]
       [Authorize]
       [Route("posttest")]
        public ResponseClient Post()
        {
            string data = DbUtils.GetStoreContractList(HttpContext, _dbContext);
            try
            {
                var restUtils = new HttpRestUtils(UsefulHelpers.STORE_ID, _dbContext);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
                
                string jsondatastring = JsonConvert.SerializeObject(data);
                var response = restUtils.Post($"/api/paymentdata/postdata", data).Result;
                if (!response.Success)
                    return response;

                string jsonData = Convert.ToString(response.Value);
                var orderDetail = JsonConvert.DeserializeObject<List<OrderDetail>>(jsonData);
                return orderDetail.Count > 0 ?
                    ReturnResponce.ListReturnResponce(orderDetail) :
                    ReturnResponce.NotFoundResponce();
            }
            catch (Exception ex)
            {
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
        [Authorize(Policy = "StoreApiUser")]
        [Route("orderdetail/{branchcode}/{orderdate}/{orderno}")]

        public async Task<ResponseClient> GetOrderDetail(string branchcode, string orderdate, string orderno)
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


        #region Return
        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("returnorderheader/{branchcode}/{contractno}/{sdate}/{edate}/{skucd}")]

        public async Task<ResponseClient> GetReturnOrderHeader(string branchcode, string contractno, DateTime sdate, DateTime edate, string skucd = "")
        {
            try
            {
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                string contractlist = "";
                if (branchcode == "null")
                {
                    branchcode = "%";
                }
                if (contractno == "null")
                {
                    contractno = "%";
                    contractlist = DbUtils.GetStoreContractList(HttpContext, _dbContext);
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
                                         storecd = header.jumcd,
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


        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("returnorderdetail/{branchcode}/{orderdate}/{orderno}")]

        public async Task<ResponseClient> GetReturnOrderDetail(string branchcode, string orderdate, string orderno)
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
