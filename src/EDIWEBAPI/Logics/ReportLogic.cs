using EDIWEBAPI.Context;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using EDIWEBAPI.Entities.APIModel;

namespace EDIWEBAPI.Logics
{
    public class ReportLogic
    {
        public static void CheckReportQty(OracleDbContext _context, ILogger _logger, string controller, string route, decimal userId)
        {
            var license = _context.SYSTEM_LICENSE_PRICE.FirstOrDefault(a => a.ROUTE == route && a.CONTROLLER == controller);
            if (license == null) return;

            var qty = _context.SYSTEM_REQUEST_ACTION_LOG.Where(a => a.USERID == userId && a.REQUESTDATE.HasValue
                    && a.ROUTE == route && a.CONTROLLER == controller && a.SUCCESS == (byte)1
                    && System.Data.Entity.DbFunctions.TruncateTime(a.REQUESTDATE.Value) == DateTime.Today).Count();

            if (qty > license.DAILYLIMIT)
                throw new Exception("Таны өдөрт хандах тоо хэтэрсэн тул уг тайлангийн эрх хязгаарлагдлаа. Та дараагийн өдрөөс хандах боломжтой.");
        }

        public static async Task<ResponseClient> GetReportData<T>(OracleDbContext _context, Entities.RequestModels.ReportRequest request)
        {
            try
            {
                //var procedure = ReportIndexByProcedureName(false, request.Index);

                var contractlist = string.Empty;
                if (UsefulHelpers.IsNull(request.ContractNo))
                {
                    contractlist = DbUtils.GetBusinessContractList(request.CompanyId, _context);
                }
                else
                {
                    var contraclst = _context.MST_CONTRACT.Where(x => x.CONTRACTNO == request.ContractNo).Select(x => new { data = x.CONTRACTNO });
                    if (contraclst != null)
                    {
                        var model = contraclst.ToList();
                        contractlist = JsonConvert.SerializeObject(model);
                    }
                }

                var restUtils = new HttpRestUtils(request.StoreId, _context);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedMessageResponce("StoreAPI not connected!");

                var url = $"/api/reportdata/monthlydata/{request.Index}/%/{request.BeginDate.ToString("yyyy-MM-dd")}";
                var storeResponse = restUtils.Post(url, contractlist).Result;
                if (!storeResponse.Success)
                    return storeResponse;

                var jsonData = Convert.ToString(storeResponse.Value);
                var header = JsonConvert.DeserializeObject<List<T>>(jsonData);

                return ReturnResponce.ListReturnResponce(header);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static async Task<ResponseClient> GetEventReport(OracleDbContext _context, int type, Entities.RequestModels.ReportRequest request)
        {
            try
            {
                var response = GetBetweenReportData<EventModel>(_context, request).Result;
                if (!response.Success || type == 1)
                    return response;

                var values = (List<EventModel>)response.Value;
                var newValues = new List<EventModel>();
                if (type == 2)
                    newValues = (from v in values
                                 group v by new { v.barcode, v.brand, v.prodname } into g
                                 let supply = values.Where(a => a.barcode == g.Key.barcode).OrderBy(a => $"{a.branch}").FirstOrDefault()
                                 orderby g.Key.barcode
                                 select new EventModel()
                                 {
                                     barcode = g.Key.barcode,
                                     brand = g.Key.brand,
                                     prodname = g.Key.prodname,
                                     custqty = g.Sum(a => a.custqty),
                                     deduction = g.Sum(a => a.deduction),
                                     saleamt = g.Sum(a => a.saleamt),
                                     saleqty = g.Sum(a => a.saleqty),
                                     supply = supply == null ? 0 : supply.supply
                                 }).ToList();
                else if (type == 3)
                    newValues = (from v in values
                                 group v by new { v.branch } into g
                                 orderby g.Key.branch
                                 select new EventModel()
                                 {
                                     branch = g.Key.branch,
                                     custqty = g.Sum(a => a.custqty),
                                     deduction = g.Sum(a => a.deduction),
                                     saleamt = g.Sum(a => a.saleamt),
                                     saleqty = g.Sum(a => a.saleqty)
                                 }).ToList();

                return ReturnResponce.ListReturnResponce(newValues);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static async Task<ResponseClient> GetBetweenReportData<T>(OracleDbContext _context, Entities.RequestModels.ReportRequest request)
        {
            try
            {
                //var procedure = ReportIndexByProcedureName(true, request.Index);
                var contractlist = string.Empty;
                if (UsefulHelpers.IsNull(request.ContractNo))
                {
                    contractlist = DbUtils.GetBusinessContractList(request.CompanyId, _context);
                }
                else
                {
                    var contraclst = _context.MST_CONTRACT.Where(x => x.CONTRACTNO == request.ContractNo).Select(x => new { data = x.CONTRACTNO });
                    if (contraclst != null)
                    {
                        var model = contraclst.ToList();
                        contractlist = JsonConvert.SerializeObject(model);
                    }
                }

                var restUtils = new HttpRestUtils(request.StoreId, _context);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedMessageResponce("StoreAPI not connected!");

                var url = $"/api/reportdata/rangedata/{request.Index}/%/{request.BeginDate.ToString("yyyy-MM-dd")}/{request.EndDate.ToString("yyyy-MM-dd")}";
                var storeResponse = restUtils.Post(url, contractlist).Result;
                if (!storeResponse.Success)
                    return storeResponse;

                var jsonData = Convert.ToString(storeResponse.Value);
                var header = JsonConvert.DeserializeObject<List<T>>(jsonData);

                return ReturnResponce.ListReturnResponce(header);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static async Task<ResponseClient> GetSaleData<T>(OracleDbContext _context, ILogger _logger, Entities.RequestModels.ReportRequest request)
        {
            try
            {
                var branchCode = UsefulHelpers.ReplaceNull(request.BranchCode);

                var restUtils = new HttpRestUtils(request.StoreId, _context);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

                var url = $"/api/{request.Controller}/{request.Route}/{request.Index}/{branchCode}/{request.RegNo}/{request.ContractNo}/{request.BeginDate.ToString("yyyy-MM-dd")}/{request.EndDate.ToString("yyyy-MM-dd")}";
                var response = restUtils.Get(url).Result;
                if (!response.Success)
                    return response;

                var jsonData = Convert.ToString(response.Value);
                var data = JsonConvert.DeserializeObject<List<T>>(jsonData);
                return ReturnResponce.ListReturnResponce(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static async Task<ResponseClient> GetSale(List<SaleTotal> values, int type)
        {
            var newValues = new List<SaleTotal>();
            if (type == 2)
                newValues = (from v in values
                             group v by new { v.barcode, v.prodname, v.ctrcd } into g
                             let suppy = values.Where(a => a.barcode == g.Key.barcode).OrderBy(a => $"{a.branch}{a.saledate}").FirstOrDefault()
                             select new SaleTotal()
                             {
                                 ctrcd = g.Key.ctrcd,
                                 barcode = g.Key.barcode,
                                 prodname = g.Key.prodname,
                                 supply = suppy == null ? 0 : suppy.supply,
                                 qty_dd = g.Sum(a => a.qty_dd),
                                 supply_dd = g.Sum(a => a.supply_dd)
                             }).ToList();
            else if (type == 3)
                newValues = (from v in values
                             group v by new { v.branch, v.ctrcd } into g
                             select new SaleTotal()
                             {
                                 branch = g.Key.branch,
                                 ctrcd = g.Key.ctrcd,
                                 qty_dd = g.Sum(a => a.qty_dd),
                                 supply_dd = g.Sum(a => a.supply_dd)
                             }).ToList();

            return ReturnResponce.ListReturnResponce(newValues);
        }

        public static async Task<ResponseClient> GetSaleTime(HttpRestUtils util, string branchcode, string contractno, DateTime beginDate, DateTime endDate)
        {
            if (beginDate > endDate)
                return ReturnResponce.FailedMessageResponce("Эхлэх огноо дуусах огнооноос их байх боломжгүй.");

            var newValues = new List<SaleTime>();
            var days = endDate.Subtract(beginDate).TotalDays + 1;
            for (int index = 0; index < days; index ++)
            {
                var currentDate = beginDate.AddDays(index);
                var url = $"/api/saledata/time/{branchcode}/{contractno}/{currentDate.ToString("yyyy-MM-dd")}";
                var response = util.Get(url).Result;
                if (!response.Success || response.Value == null) continue;

                var jsonData = Convert.ToString(response.Value);
                var data = JsonConvert.DeserializeObject<List<SaleTime>>(jsonData);
                if (!data.Any()) continue;

                foreach (var current in data)
                {
                    var found = newValues.FirstOrDefault(a => a.barcode == current.barcode && a.seq == current.seq);
                    if (found == null)
                        newValues.Add(current);
                    else
                        found.Append(current);
                }
            }
            return ReturnResponce.ListReturnResponce(newValues);
        }
    }
}
