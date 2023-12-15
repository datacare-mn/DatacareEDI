using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.DBModel.Order;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using EDIWEBAPI.Entities.APIModel;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using EDIWEBAPI.Entities.RequestModels;

namespace EDIWEBAPI.Logics
{
    public class OrderLogic : BaseLogic
    {
        public static Entities.Interfaces.ISeen GetOrder(OracleDbContext _context, ILogger _log, SYSTEM_SHORTURL url)
        {
            try
            {
                Entities.Interfaces.ISeen found = null;
                if (url.TYPE == "ORDER")
                {
                    found = _context.REQ_ORDER.FirstOrDefault(x => (url.RECORDID.HasValue ? x.ORDERID == url.RECORDID : x.FILEURL == url.LONGURL));
                }
                else if (url.TYPE == "RETURN")
                {
                    found = _context.REQ_RETURN_ORDER.FirstOrDefault(x => (url.RECORDID.HasValue ? x.RETORDERID == url.RECORDID : x.FILEURL == url.LONGURL));
                }
                return found;
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, $"{UsefulHelpers.GetMethodName(System.Reflection.MethodBase.GetCurrentMethod())} => {UsefulHelpers.GetExceptionMessage(ex)}");
                return null;
            }
        }

        public static SYSTEM_SHORTURL GetShortUrl(OracleDbContext _context, ILogger _log, string shorturl)
        {
            SYSTEM_SHORTURL url = null;
            try
            {
                var urls = shorturl.Replace("%2F", "/");
                url = _context.SYSTEM_SHORTURL.FirstOrDefault(x => x.SHORTURL == urls);
                if (url != null)
                {
                    url.LASTREQDATE = DateTime.Now;
                    url.VISITCOUNT = (url.VISITCOUNT + 1);
                    Update(_context, url);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, $"{UsefulHelpers.GetMethodName(System.Reflection.MethodBase.GetCurrentMethod())} => {UsefulHelpers.GetExceptionMessage(ex)}");
            }
            return url;
        }

        public static ResponseClient GetOrderDetail(OracleDbContext _context, ILogger _logger, string branchCode, string orderDate, string orderNo)
        {
            var request = new List<OrderDetailRequest>();
            request.Add(new OrderDetailRequest() { BranchCode = branchCode, OrderDate = orderDate, OrderNo = orderNo });
            return GetOrderDetail(_context, _logger, request);
        }

        public static ResponseClient GetOrderDetail(OracleDbContext _context, ILogger _logger, List<OrderDetailRequest> headers, string groupType = "BRANCH")
        {
            try
            {
                var restUtils = new HttpRestUtils(UsefulHelpers.STORE_ID, _context);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

                var details = new List<OrderDetail>();
                foreach (var header in headers)
                {
                    var response = restUtils.Get($"/api/orderdata/orderdetail/{header.BranchCode}/{header.OrderDate}/{header.OrderNo}").Result;
                    if (!response.Success) continue;

                    var jsonData = Convert.ToString(response.Value);
                    var currentDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(jsonData);
                    if (currentDetails == null || currentDetails.Count == 0) continue;

                    currentDetails.ForEach(a =>
                    {
                        a.jumcd = header.BranchName;
                        a.ilja = header.OrderDate;
                        details.Add(a);
                    });
                }

                // ХЭРВЭЭ ОЛОН ЗАХИАЛГЫН МЭДЭЭЛЛИЙГ АВАХ БОЛ
                // САЛБАР, БАРААГААР GROUP ЛЭЖ БУЦААНА
                if (headers.Count > 1 && details.Any())
                {
                    var groupedDetails = new List<OrderDetail>();
                    if (groupType == "BRANCH")
                        groupedDetails = (from d in details
                                          group d by new { d.jumcd, d.skucd, d.skunm, d.unit, d.boxqty } into g
                                          let costObject = details.Where(a => a.jumcd == g.Key.jumcd && a.skucd == g.Key.skucd).OrderBy(a => a.ilja).FirstOrDefault()
                                          select new OrderDetail()
                                          {
                                              jumcd = g.Key.jumcd,
                                              skucd = g.Key.skucd,
                                              skunm = g.Key.skunm,
                                              unit = g.Key.unit,
                                              boxqty = g.Key.boxqty,
                                              ordbox = g.Sum(a => a.ordbox),
                                              ordqty = g.Sum(a => a.ordqty),
                                              ord_supply = costObject == null ? 0 : costObject.ord_supply,
                                              ord_supplyvat = costObject == null ? 0 : costObject.ord_supplyvat,
                                              ord_supplyamt = g.Sum(a => a.ord_supplyamt)
                                          }).ToList();

                    else if (groupType == "DATE")
                        groupedDetails = (from d in details
                                          group d by new { d.ilja, d.skucd, d.skunm, d.unit, d.boxqty } into g
                                          let costObject = details.Where(a => a.ilja == g.Key.ilja && a.skucd == g.Key.skucd).OrderBy(a => a.jumcd).FirstOrDefault()
                                          select new OrderDetail()
                                          {
                                              ilja = g.Key.ilja,
                                              skucd = g.Key.skucd,
                                              skunm = g.Key.skunm,
                                              unit = g.Key.unit,
                                              boxqty = g.Key.boxqty,
                                              ordbox = g.Sum(a => a.ordbox),
                                              ordqty = g.Sum(a => a.ordqty),
                                              ord_supply = costObject == null ? 0 : costObject.ord_supply,
                                              ord_supplyvat = costObject == null ? 0 : costObject.ord_supplyvat,
                                              ord_supplyamt = g.Sum(a => a.ord_supplyamt)
                                          }).ToList();

                    return ReturnResponce.ListReturnResponce(groupedDetails.OrderBy(a => a.skucd));
                }

                return details.Any() ?
                    ReturnResponce.ListReturnResponce(details) :
                    ReturnResponce.NotFoundResponce();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ORDERLOGIC.GETORDERDETAIL : {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static ResponseClient GetOrderHeader(OracleDbContext _dbContext,
            int storeid, int comid, string branchcode, string contractno, DateTime sdate, DateTime edate,
            string skucd, int orderid = 0)
        {
            skucd = UsefulHelpers.ReplaceNull(skucd);
            branchcode = UsefulHelpers.ReplaceNull(branchcode);
            var contractlist = string.Empty;

            if (UsefulHelpers.IsNull(contractno))
            {
                contractno = "%";
                contractlist = comid == 0 ? DbUtils.GetStoreContractList(storeid, _dbContext) : DbUtils.GetBusinessContractList(comid, _dbContext);
            }

            var restUtils = new HttpRestUtils(storeid, _dbContext);
            if (!restUtils.StoreServerConnected)
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

            //if (orderid != 0)
            //{
            //    var found = _dbContext.REQ_ORDER.FirstOrDefault(r => r.ORDERID == orderid);
            //    contractno = found != null ? found.CONTRACTNO : "0";
            //}

            var url = $"/api/orderdata/orderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{skucd}";
            var jsonData = Convert.ToString(restUtils.Post(url, contractlist).Result.Value);

            var headers = JsonConvert.DeserializeObject<List<OrderHeader>>(jsonData);
            if (headers == null || headers.Count == 0)
                return ReturnResponce.NotFoundResponce();

            var orders = (from ordr in _dbContext.REQ_ORDER.ToList()

                          join users in _dbContext.SYSTEM_USERS.ToList()
                          on ordr.APPROVEDUSER equals users?.ID into au
                          from appuser in au.DefaultIfEmpty()

                          join seeuser in _dbContext.SYSTEM_USERS.ToList()
                          on ordr.SEENUSER equals seeuser?.ID into su
                          from seenuser in su.DefaultIfEmpty()

                          where (contractno == "%" || ordr.CONTRACTNO == contractno)

                          select new
                          {
                              ordr.ORDERID,
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
                          });

            var query = (from header in headers
                         join request in orders
                         on new { stopdate = header.ordilja, orderno = header.ordno, contractno = header.ctrcd }
                         equals new { stopdate = request.ORDERDATE, orderno = request.ORDERNO, contractno = request.CONTRACTNO } into t
                         from rt in t.DefaultIfEmpty()
                         select new OrderDto()
                         {
                             ordilja = header.ordilja,
                             ordno = header.ordno,
                             storecd = header.jumcd,
                             ctrcd = header.ctrcd,
                             ctrnm = header.ctrnm,
                             eta = header.eta,
                             ord_supply = header.ord_supply,
                             ord_supplyvat = header.ord_supplyvat,
                             ord_supplyamt = header.ord_supplyamt,
                             skucnt = header.skucnt,
                             ISSEEN = rt?.ISSEEN,
                             SEENDATE = rt?.SEENDATE,
                             seenuser = rt?.seenuser,
                             APPROVEDDATE = rt?.APPROVEDDATE,
                             APPROVEDUSER = rt?.FIRSTNAME,
                             ORDERID = rt?.ORDERID
                         })
                         .Where(h => (orderid == 0) ? (contractno == "%" || h.ctrcd == contractno) : (h.ORDERID == orderid))
                         .OrderBy(x => x.ctrcd).ThenBy(x => x.ordilja);

            return ReturnResponce.ListReturnResponce(query.Distinct().ToList());
        }

        public static async Task<ResponseClient> GetReturnDetail(OracleDbContext _context, ILogger _logger, string branchCode, string orderDate, string orderNo)
        {
            var request = new List<OrderDetailRequest>();
            request.Add(new OrderDetailRequest() { BranchCode = branchCode, OrderDate = orderDate, OrderNo = orderNo });
            return GetOrderDetail(_context, _logger, request);
        }

        public static async Task<ResponseClient> GetReturnDetail(OracleDbContext _context, ILogger _logger, List<OrderDetailRequest> headers, string groupType = "BRANCH")
        {
            try
            {
                var restUtils = new HttpRestUtils(UsefulHelpers.STORE_ID, _context);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

                var details = new List<ReturnOrderDetail>();
                foreach (var header in headers)
                {
                    var response = restUtils.Get($"/api/orderdata/returnorderdetail/{header.BranchCode}/{header.OrderDate}/{header.OrderNo}").Result;
                    if (!response.Success) continue;

                    var jsonData = Convert.ToString(response.Value);
                    var currentDetails = JsonConvert.DeserializeObject<List<ReturnOrderDetail>>(jsonData);
                    if (currentDetails == null || currentDetails.Count == 0) continue;
                    
                    currentDetails.ForEach(a =>
                    {
                        a.jumcd = header.BranchName;
                        a.ilja = header.OrderDate;
                        details.Add(a);
                    });
                }

                // ХЭРВЭЭ ОЛОН ЗАХИАЛГЫН МЭДЭЭЛЛИЙГ АВАХ БОЛ
                // САЛБАР, БАРААГААР GROUP ЛЭЖ БУЦААНА
                if (headers.Count > 1 && details.Any())
                {
                    var groupedDetails = new List<ReturnOrderDetail>();
                    if (groupType == "BRANCH")
                        groupedDetails = (from d in details
                                          group d by new { d.jumcd, d.skucd, d.skunm, d.unit, d.boxqty } into g
                                          let costObject = details.Where(a => a.jumcd == g.Key.jumcd && a.skucd == g.Key.skucd).OrderBy(a => a.ilja).FirstOrDefault()
                                          select new ReturnOrderDetail()
                                          {
                                              jumcd = g.Key.jumcd,
                                              skucd = g.Key.skucd,
                                              skunm = g.Key.skunm,
                                              unit = g.Key.unit,
                                              boxqty = g.Key.boxqty,
                                              ordbox = g.Sum(a => a.ordbox),
                                              order_qty = g.Sum(a => a.order_qty),
                                              ord_supply = costObject == null ? 0 : costObject.ord_supply,
                                              ord_supplyvat = costObject == null ? 0 : costObject.ord_supplyvat,
                                              ord_supplyamt = g.Sum(a => a.ord_supplyamt)
                                          }).ToList();

                    else if (groupType == "DATE")
                        groupedDetails = (from d in details
                                          group d by new { d.ilja, d.skucd, d.skunm, d.unit, d.boxqty } into g
                                          let costObject = details.Where(a => a.ilja == g.Key.ilja && a.skucd == g.Key.skucd).OrderBy(a => a.jumcd).FirstOrDefault()
                                          select new ReturnOrderDetail()
                                          {
                                              ilja = g.Key.ilja,
                                              skucd = g.Key.skucd,
                                              skunm = g.Key.skunm,
                                              unit = g.Key.unit,
                                              boxqty = g.Key.boxqty,
                                              ordbox = g.Sum(a => a.ordbox),
                                              order_qty = g.Sum(a => a.order_qty),
                                              ord_supply = costObject == null ? 0 : costObject.ord_supply,
                                              ord_supplyvat = costObject == null ? 0 : costObject.ord_supplyvat,
                                              ord_supplyamt = g.Sum(a => a.ord_supplyamt)
                                          }).ToList();

                    return ReturnResponce.ListReturnResponce(groupedDetails);
                }

                return details.Any() ?
                    ReturnResponce.ListReturnResponce(details) :
                    ReturnResponce.NotFoundResponce();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ORDERLOGIC.GETRETURNDETAIL : {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static ResponseClient GetReturnHeader(OracleDbContext _dbContext,
            int storeid, int comid, string branchcode, string contractno, DateTime sdate, DateTime edate,
            string skucd, int returnid = 0)
        {
            skucd = UsefulHelpers.ReplaceNull(skucd);
            branchcode = UsefulHelpers.ReplaceNull(branchcode);
            var contractlist = string.Empty;

            if (UsefulHelpers.IsNull(contractno))
            {
                contractno = "%";
                contractlist = comid == 0 ? DbUtils.GetStoreContractList(storeid, _dbContext) : DbUtils.GetBusinessContractList(comid, _dbContext);
            }

            var restUtils = new HttpRestUtils(storeid, _dbContext);
            if (!restUtils.StoreServerConnected)
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

            //if (returnid != 0)
            //{
            //    var found = _dbContext.REQ_RETURN_ORDER.FirstOrDefault(r => r.RETORDERID == returnid);
            //    contractno = found != null ? found.CONTRACTNO : "0";
            //}

            var url = $"/api/orderdata/returnorderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{skucd}";
            var response = restUtils.Post(url, contractlist).Result;
            if (!response.Success)
                return response;

            var jsonData = Convert.ToString(response.Value);
            
            var headers = JsonConvert.DeserializeObject<List<ReturnOrderHeader>>(jsonData);
            if (headers == null || headers.Count == 0)
                return ReturnResponce.NotFoundResponce();

            var orders = (from ordr in _dbContext.REQ_RETURN_ORDER.ToList()

                          join users in _dbContext.SYSTEM_USERS.ToList()
                          on ordr.APPROVEDUSER equals users?.ID into au
                          from appuser in au.DefaultIfEmpty()

                          join seeuser in _dbContext.SYSTEM_USERS.ToList()
                          on ordr.SEENUSER equals seeuser?.ID into su
                          from seenuser in su.DefaultIfEmpty()

                          where (contractno == "%" || ordr.CONTRACTNO == contractno)

                          select new
                          {
                              ordr.RETORDERID,
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
                          });

            var query = (from header in headers.ToList()
                         join request in orders
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
                             APPROVEDUSER = rt?.FIRSTNAME,
                             rt?.RETORDERID
                         })
                         .Where(h => (returnid == 0) ? (contractno == "%" || h.ctrcd == contractno) : (h.RETORDERID == returnid))
                         .OrderBy(x => x.ctrcd).ThenBy(x => x.retno);

            return ReturnResponce.ListReturnResponce(query.Distinct().ToList());
        }

        public static REQ_ORDER Get(OracleDbContext _context, string orderDate, string orderNo, string contractNo)
        {
            return _context.REQ_ORDER.FirstOrDefault(x => x.ORDERNO == orderNo && x.ORDERDATE == orderDate && x.CONTRACTNO == contractNo);
        }

        public static REQ_RETURN_ORDER GetReturn(OracleDbContext _context, string orderDate, string orderNo, string contractNo, string retIndex)
        {
            return _context.REQ_RETURN_ORDER.FirstOrDefault(x => x.ORDERNO == orderNo && x.ORDERDATE == orderDate && x.CONTRACTNO == contractNo && x.RETURNINDEX == retIndex);
        }

        public static void RemoveOld(OracleDbContext _context)
        {
            var createddate = DateTime.Today.AddMonths(-1);
            var oldorders = _context.REQ_ORDER.Where(x => x.CREATEDDATE <= createddate);
            if (oldorders.Any())
            {
                _context.REQ_ORDER.RemoveRange(oldorders);
                _context.SaveChanges();
            }
        }
    }
}
