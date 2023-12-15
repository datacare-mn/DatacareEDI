using EDIWEBAPI.Context;
using EDIWEBAPI.Controllers.Storeapi;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
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
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/dashboardbusinesss")]
    public class DashboardBusinessController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<DashboardBusinessController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public DashboardBusinessController(OracleDbContext context, ILogger<DashboardBusinessController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        private List<string> GetContracts(int comId, int storeId, string contractNo)
        {
            var values = new List<string>();
            if (string.IsNullOrEmpty(contractNo) || UsefulHelpers.IsNull(contractNo))
                values = Logics.ContractLogic.GetContracts(_dbContext, _log, storeId, comId, true)
                    .Select(c => c.CONTRACTNO).ToList();
            else
                values.Add(contractNo);

            return values;
        }


        [HttpGet]
        [Route("productplan/{departmentId}")]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient GetProductPlan(int departmentId)
        {
            var comId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            try
            {
                return Logics.MasterLogic.GetProductPlan(_dbContext, _log, ORGTYPE.Бизнес, comId, 0, departmentId);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("productperformance/{duration}/{departmentId}")]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient GetProductPerformation(int duration, int departmentId)
        {
            var comId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            try
            {
                var beginDate = DateTime.Now.AddDays(-duration);
                return Logics.MasterLogic.GetProductPerformance(_dbContext, _log, ORGTYPE.Бизнес, comId, 0, departmentId, beginDate);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getproperties/{contractno}/{beginDate}/{endDate}")]
        public ResponseClient GetProperties(string contractno, DateTime beginDate, DateTime endDate)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
            try
            {
                //var result = new List<Entities.Dashboard.DASH_BIZ_ACTION>();

                var response = Logics.PaymentLogic.GetStorePayment(_dbContext, _log, UsefulHelpers.STORE_ID, regno, contractno, beginDate, endDate, false, false);
                response.RowCount = 0;
                if (response.Success && response.Value != null)
                {
                    var values = (List<PaymentDto>)response.Value;
                    if (values.Any() && Logics.PaymentLogic.CheckCycle(_dbContext, _log, UsefulHelpers.STORE_ID, values[0].paycycle, endDate))
                        response.RowCount = values.Count(a => a.ATTACHDATE == null);
                }

                var orderResponse = Logics.OrderLogic.GetOrderHeader(_dbContext, UsefulHelpers.STORE_ID, comid, "%", contractno, endDate, endDate, "%");
                var returns = Logics.OrderLogic.GetReturnHeader(_dbContext, UsefulHelpers.STORE_ID, comid, "%", contractno, beginDate, endDate, "%");
                var buyings = Logics.BILogic.GetBuyingHeader(_dbContext, UsefulHelpers.STORE_ID, regno, contractno, beginDate, endDate, "1");

                var result = new { PAYMENT = response.RowCount, ORDER = orderResponse.RowCount, RETURN = returns.RowCount, BUYING = buyings.RowCount };
                return ReturnResponce.ListReturnResponce(result);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getsaletotalbydate/{contractno}/{beginDate}/{endDate}")]
        public ResponseClient GetSaleTotalByDate(string contractno, DateTime beginDate, DateTime endDate)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
            try
            {
                var result = new List<SaleBranch>();
                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = UsefulHelpers.STORE_ID,
                    CompanyId = comid,
                    RegNo = regno,
                    ContractNo = contractno,
                    BeginDate = beginDate,
                    EndDate = endDate,

                    Controller = "saledata",
                    Route = "storedata",
                    Index = 4
                };

                var response = Logics.ReportLogic.GetSaleData<SaleBranch>(_dbContext, _log, request).Result;
                if (response.Success && response.Value != null)
                    result = (List<SaleBranch>)response.Value;

                return ReturnResponce.ListReturnResponce(result);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getcustomerbydate/{contractno}/{beginDate}/{endDate}")]
        public ResponseClient GetCustomerByDate(string contractno, DateTime beginDate, DateTime endDate)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                var result = new List<SaleCustomer>();
                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = UsefulHelpers.STORE_ID,
                    CompanyId = comid,
                    ContractNo = contractno,
                    Index = 8,
                    BeginDate = beginDate,
                    EndDate = endDate
                };
                var response = Logics.ReportLogic.GetBetweenReportData<SaleCustomer>(_dbContext, request).Result;
                if (response.Success && response.Value != null)
                    result = (List<SaleCustomer>)response.Value;

                return ReturnResponce.ListReturnResponce(result);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getsalebranch/{contractno}/{beginDate}/{endDate}")]
        public ResponseClient GetSaleBranch(string contractno, DateTime beginDate, DateTime endDate)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                var contracts = GetContracts(comid, UsefulHelpers.STORE_ID, contractno);
                var result = new List<SaleBranch>();
                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = UsefulHelpers.STORE_ID,
                    CompanyId = comid,
                    BranchCode = "null",
                    //ContractNo = contractno,
                    BeginDate = beginDate,
                    EndDate = endDate,
                    Controller = "saledata",
                    Route = "data",
                    Index = 1
                };

                foreach (var contract in contracts)
                {
                    request.ContractNo = contract;
                    var response = Logics.ReportLogic.GetSaleData<SaleBranch>(_dbContext, _log, request).Result;
                    if (!response.Success || response.Value == null) continue;

                    var values = (List<SaleBranch>)response.Value;
                    values.ForEach(v => { v.ctrcd = contract; result.Add(v); });
                }

                if (contracts.Count > 1)
                {
                    result = (from r in result
                              group r by new { r.branch } into g
                              select new SaleBranch()
                              {
                                  branch = g.Key.branch,
                                  qty_dd = g.Sum(a => a.qty_dd),
                                  qty_mm = g.Sum(a => a.qty_mm),
                                  supply_dd = g.Sum(a => a.supply_dd),
                                  supply_mm = g.Sum(a => a.supply_mm)
                              }).ToList();
                }

                return ReturnResponce.ListReturnResponce(result);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("gettopproduct/{contractno}/{beginDate}/{endDate}/{top}/{amount}")]
        public ResponseClient GetTopProduct(string contractno, DateTime beginDate, DateTime endDate, int top, bool amount)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                var contracts = GetContracts(comid, UsefulHelpers.STORE_ID, contractno);
                var result = new List<SaleTotal>();
                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = UsefulHelpers.STORE_ID,
                    CompanyId = comid,
                    BranchCode = "null",
                    //ContractNo = contractno,
                    BeginDate = beginDate,
                    EndDate = endDate,
                    Controller = "saledata",
                    Route = "data",
                    Index = 3
                };

                foreach (var contract in contracts)
                {
                    request.ContractNo = contract;
                    var response = Logics.ReportLogic.GetSaleData<SaleTotal>(_dbContext, _log, request).Result;
                    if (!response.Success || response.Value == null) continue;

                    var values = (List<SaleTotal>)response.Value;
                    values.ForEach(v => { v.ctrcd = contract; result.Add(v); });
                }


                if (result.Any())
                {
                    var grouped = (from l in result
                                   group l by new { l.barcode, l.prodname } into g
                                   select new
                                   {
                                       barcode = g.Key.barcode,
                                       prodname = g.Key.prodname,
                                       AMOUNT = g.Sum(a => amount ? a.supply_dd : a.qty_dd)
                                   }).OrderByDescending(a => a.AMOUNT).Take(top);

                    if (grouped.Any())
                        result = grouped.Select(a => new SaleTotal() { barcode = a.barcode, prodname = a.prodname, qty_dd = a.AMOUNT }).ToList();

                }
                return ReturnResponce.ListReturnResponce(result);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("gettopstock/{contractno}/{endDate}/{top}/{increasing}")]
        public ResponseClient GetTopStock(string contractno, DateTime endDate, int top, bool increasing)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = UsefulHelpers.STORE_ID,
                    CompanyId = comid,
                    ContractNo = contractno,
                    BeginDate = endDate,
                    Index = 9
                };
                var result = new List<StockModel>();
                var response = Logics.ReportLogic.GetReportData<StockModel>(_dbContext, request).Result;
                if (response.Success && response.Value != null)
                {
                    //var json = JsonConvert.SerializeObject(response.Value);
                    //result = JsonConvert.DeserializeObject<List<StockModel>>(json);
                    result = (List<StockModel>)response.Value;
                }

                if (result.Any())
                {
                    var grouped = (from l in result
                                   group l by new { l.barcode, l.prodname } into g
                                   select new
                                   {
                                       g.Key.barcode,
                                       g.Key.prodname,
                                       AMOUNT = g.Sum(a => a.STKQTY)
                                   }).OrderByDescending(a => a.AMOUNT).Take(top);

                    if (grouped.Any())
                        result = grouped.Select(a => new StockModel() { barcode = a.barcode, prodname = a.prodname, STKQTY = a.AMOUNT }).ToList();
                }
                return ReturnResponce.ListReturnResponce(result);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getorders/{contractno}/{startDate}/{endDate}")]
        public ResponseClient GetOrders(string contractno, DateTime startDate, DateTime endDate)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                var response = Logics.OrderLogic.GetOrderHeader(_dbContext, UsefulHelpers.STORE_ID, comid, "%", contractno, startDate, endDate, "%");
                if (!response.Success || response.Value == null)
                    return response;

                var values = (List<OrderDto>)response.Value;

                //var days = (from v in values
                //            group v by v.ordilja into g
                //            select new { ILJA = g.Key }).ToList();

                var groupValues = (from v in values
                                   group v by new { v.storecd, v.ordilja } into g
                                   orderby g.Key.ordilja
                                   select new 
                                   {
                                       branch = g.Key.storecd,
                                       date = g.Key.ordilja,
                                       amount = g.Sum(a => a.ord_supplyamt),
                                       quantity = g.Count()
                                   }).ToList();

                //var result = (from d in days
                //              join v in groupValues on d.ILJA equals v.ordilja into lj
                //              from l in lj.DefaultIfEmpty()
                //              select new
                //              {
                //                  STORECD = l.storecd,
                //                  ORDILJA = d.ILJA,
                //                  ORD_SUPPLYAMT = l.ord_supplyamt
                //              }).ToList();

                return ReturnResponce.ListReturnResponce(groupValues);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getbuyings/{contractno}/{startDate}/{endDate}")]
        public ResponseClient GetBuyings(string contractno, DateTime startDate, DateTime endDate)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
            try
            {
                var response = Logics.BILogic.GetBuyingHeader(_dbContext, UsefulHelpers.STORE_ID, regno, contractno, startDate, endDate, "null");
                if (!response.Success || response.Value == null)
                    return response;

                var values = (List<BuyingHeader>)response.Value;

                var result = (from v in values
                              group v by new { v.storename, v.buyymd } into g
                              orderby g.Key.buyymd
                              select new
                              {
                                  branch = g.Key.storename,
                                  date = g.Key.buyymd,
                                  amount = g.Sum(a => a.buyamt),
                                  quantity = g.Sum(a => a.buyqty)
                              }).ToList();

                return ReturnResponce.ListReturnResponce(result);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getpayments/{contractno}/{startDate}/{endDate}/{top}")]
        public ResponseClient GetPayments(string contractno, DateTime startDate, DateTime endDate, int top)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var regNo = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
            try
            {
                var contracts = (from c in _dbContext.MST_CONTRACT.ToList()
                                 where c.BUSINESSID == comid && (contractno == "null" || c.CONTRACTNO == contractno)
                                 select new { c.CONTRACTNO }).ToList();

                var requests = from r in _dbContext.REQ_PAYMENT.ToList()
                               join c in contracts on r.CONTRACTNO equals c.CONTRACTNO
                               where r.APPROVEDDATE != null
                               select new { r.CONTRACTNO, r.STPYMD, r.APPROVEDDATE, r.ATTACHDATE };

                var values = (from r in requests
                              where r.ATTACHDATE != null
                              orderby r.APPROVEDDATE descending
                              select new Entities.Dashboard.DASH_BIZ_ACTION()
                              {
                                  STATUS = "Амжилттай",
                                  ACTION = $"{r.CONTRACTNO}-{r.STPYMD} гэрээний нэхэмжлэл илгээх",
                                  ACTIONDATE = r.APPROVEDDATE
                              }).Take(top).ToList();

                var unsuccessful = (from r in requests
                                    where r.ATTACHDATE == null
                                    orderby r.APPROVEDDATE descending
                                    select new Entities.Dashboard.DASH_BIZ_ACTION()
                                    {
                                        STATUS = "Амжилтгүй",
                                        ACTION = $"{r.CONTRACTNO}-{r.STPYMD} гэрээний нэхэмжлэл илгээх",
                                        ACTIONDATE = r.APPROVEDDATE
                                    }).Take(top);


                if (unsuccessful.Any())
                {
                    values.AddRange(unsuccessful.ToList());
                    values = values.OrderByDescending(a => a.ACTIONDATE).Take(top).ToList();
                }

                return ReturnResponce.ListReturnResponce(values);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        #region Get
        [HttpGet]
        [Route("lastrequest")]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient GetLastRequest()
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var currentdata = _dbContext.DASH_BIZ_LAST_REQUEST(comid).ToList();
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
        [Route("paymentcount")]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient AllPayment()
        {
            try
            {
                int usercomtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                string regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
                DateTime sdate = DateTime.Now.AddDays(-30);
                DateTime edate = DateTime.Now;

                Logics.ContractLogic.Modify(_dbContext, _log, UsefulHelpers.STORE_ID, regno);

                if (usercomtype == 1)
                {
                    HttpRestUtils restUtils = new HttpRestUtils(2, _dbContext);
                    if (restUtils.StoreServerConnected)
                    {
                        List<PaymentHeader> PaymentHeader = new List<PaymentHeader>();
                        string jsonData = Convert.ToString(restUtils.Get($"/api/paymentdata/paymentheader/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}").Result.Value);
                        PaymentHeader = JsonConvert.DeserializeObject<List<PaymentHeader>>(jsonData);

                        if (PaymentHeader.Count > 0)
                        {
                            var query = (from header in PaymentHeader.ToList()
                                         join request in (from pymt in _dbContext.REQ_PAYMENT.ToList()
                                                          join users in _dbContext.SYSTEM_USERS on pymt.APPROVEDUSER equals users?.ID into su
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

                                         join contractslist in _dbContext.MST_CONTRACT.ToList() on header.ctrcd equals contractslist.CONTRACTNO into con
                                         from contr in con.DefaultIfEmpty()
                                         join company in _dbContext.SYSTEM_ORGANIZATION.ToList() on contr.BUSINESSID equals company.ID into companycontract
                                         from comcontract in companycontract.DefaultIfEmpty()
                                         select new
                                         {

                                             amt = header.amt,
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
                                             header.paycycle,
                                             rt?.APPROVEDDATE,
                                             APPROVEDUSER = rt?.FIRSTNAME,
                                             rt?.ATTACHFILE,
                                             rt?.CONTRACTNO,
                                             rt?.DESCRIPTION,
                                             rt?.STORESEEN,
                                             rt?.ID,
                                             rt?.ATTACHDATE,
                                             comid = comcontract.ID,
                                             regno = comcontract.REGNO
                                         });
                            int calculatepayment = query.Where(x => x.ATTACHFILE == null).Count();
                            int attachedpayment = query.Where(x => x.ATTACHFILE != null).Count();
                            var dataconrats = query.Where(x => x.ATTACHFILE == null);
                            PaymentController pcontroller = new PaymentController(_dbContext, null);
                            List<string> checkedcontracts = new List<string>();
                            foreach (var data in dataconrats)
                            {
                                var rc = pcontroller.CheckAttachment(2, data.paycycle);
                                if (rc.Result.Success)
                                {
                                    checkedcontracts.Add(data.ctrcd);
                                }
                            }
                            object[] xvalue = new object[2];
                            xvalue[0] = new { calculatepayment = calculatepayment, attachedpayment = attachedpayment };
                            xvalue[1] = new { contractlist = checkedcontracts };

                            return ReturnResponce.ListReturnResponce(xvalue);


                        }
                        else
                            return ReturnResponce.NotFoundResponce();
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
        [Route("paymentreportcount")]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient AllPaymentReport()
        {
            try
            {
                string regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
                DateTime sdate = DateTime.Now.AddDays(-30);
                DateTime edate = DateTime.Now;
                int approvedpayment = _dbContext.REQ_PAYMENT_REPORT.Where(x => x.APPROVEDATE >= sdate && x.REGNO == regno).Count();
                int attachedpayment = _dbContext.REQ_PAYMENT_REPORT.Where(x => x.ATTACHDATE >= sdate && x.REGNO == regno).Count();
                object[] xvalue = new object[1];
                xvalue[0] = new { approvedpayment = approvedpayment, attachedpayment = attachedpayment };
                return ReturnResponce.ListReturnResponce(xvalue);
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);

            }
        }


        [HttpGet]
        [Route("saledata")]
        [Authorize(Policy = "BizApiUser")]
        public async Task<ResponseClient> GetSaleData()
        {
            try
            {
                string branchcode = "%";
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                DateTime sdate = DateTime.Now.AddDays(-3);
                
                var restUtils = new HttpRestUtils(2, _dbContext);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
                
                var contractlist = Logics.ContractLogic.GetContracts(_dbContext, _log, UsefulHelpers.STORE_ID, comid);
                var SaleSkuData = new List<SaleSku>();
                if (contractlist == null)
                    return ReturnResponce.NotFoundResponce();
                
                var retData = new List<object>();
                for (DateTime date = sdate; date < DateTime.Today; date = date.AddDays(1))
                {
                    foreach (MST_CONTRACT contract in contractlist)
                    {
                        //date = new DateTime(2017, 11, 9);
                        string jsonData = Convert.ToString(restUtils.Get($"/api/saledata/salesku/{branchcode}/{contract.CONTRACTNO}/{date.ToString("yyyyMMdd")}").Result.Value);
                        SaleSkuData.AddRange(JsonConvert.DeserializeObject<List<SaleSku>>(jsonData));
                    }
                    string daynumber = date.ToString("dd");

                    decimal retvalus = 0;

                    foreach (SaleSku sb in SaleSkuData)
                    {
                        retvalus += Convert.ToDecimal(sb.supply_dd);
                    }
                    retData.Add(new { value = retvalus, daynumber = daynumber });
                    SaleSkuData = new List<SaleSku>();
                }

                return ReturnResponce.ListReturnResponce(retData);
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }

        [HttpGet]
        [Authorize]
        [Route("saletime")]
        public async Task<ResponseClient> GetSaleTime()
        {
            try
            {
                string branchcode = "%";
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                DateTime sdate = DateTime.Now.AddDays(-3);
                var restUtils = new HttpRestUtils(2, _dbContext);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
                
                var contractlist = Logics.ContractLogic.GetContracts(_dbContext, _log, UsefulHelpers.STORE_ID, comid);
                var SaletimeData = new List<SaleTime>();
                if (contractlist == null)
                    return ReturnResponce.NotFoundResponce();

                var retData = new List<object>();
                for (DateTime date = sdate; date < DateTime.Today; date = date.AddDays(1))
                {
                    foreach (MST_CONTRACT contract in contractlist)
                    {
                        string jsonData = Convert.ToString(restUtils.Get($"/api/saledata/saletime/{branchcode}/{contract.CONTRACTNO}/{date.ToString("yyyyMMdd")}").Result.Value);
                        SaletimeData.AddRange(JsonConvert.DeserializeObject<List<SaleTime>>(jsonData));
                    }
                    string daynumber = date.ToString("dd");
                    decimal retvalus = 0;
                    foreach (SaleTime sb in SaletimeData)
                    {
                        if (sb.seq == "4" && sb.tp == "P")
                        {
                            retvalus += Convert.ToDecimal(sb.total);
                        }
                    }
                    retData.Add(new { value = retvalus, daynumber = daynumber });
                    SaletimeData = new List<SaleTime>();

                }
                return ReturnResponce.ListReturnResponce(retData);
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
        [Route("orderstat")]
        public async Task<ResponseClient> GetOrderHeader()
        {
            // CompanyLogUtils.SaveCompanyLog(HttpContext);
            string contractlist = "";
            try
            {
                string skucd = "%";
                string branchcode = "%";
                string contractno = "%";
                int businessid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                contractlist = DbUtils.GetBusinessContractList(HttpContext, _dbContext);

                DateTime sdate = DateTime.Now;
                DateTime edate = DateTime.Now;

                HttpRestUtils restUtils = new HttpRestUtils(2, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<OrderHeader> OrderHeader = new List<OrderHeader>();
                    string jsonData = "";
                    if (contractlist.Length > 0)
                    {
                        jsonData = Convert.ToString(restUtils.Post($"/api/orderdata/orderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{skucd}", contractlist).Result.Value);
                    }
                    else
                    {
                        jsonData = Convert.ToString(restUtils.Post($"/api/orderdata/orderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{skucd}", "").Result.Value);
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
                                         orddate = header.ordilja,
                                         branch = header.jumcd,
                                         header.ctrcd,
                                         header.ctrnm,
                                         header.ord_supplyamt,
                                         prodcnt = header.skucnt
                                     });
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
        [Authorize(Policy = "BizApiUser")]
        [Route("orderstatmonth")]
        public async Task<ResponseClient> GetMonthCount()
        {
            // CompanyLogUtils.SaveCompanyLog(HttpContext);
            string contractlist = "";
            try
            {
                string skucd = "%";
                string branchcode = "%";
                string contractno = "%";
                int businessid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                contractlist = DbUtils.GetBusinessContractList(HttpContext, _dbContext);

                DateTime sdate = DateTime.Now.AddDays(-30);
                DateTime edate = DateTime.Now;

                HttpRestUtils restUtils = new HttpRestUtils(2, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<OrderHeader> OrderHeader = new List<OrderHeader>();
                    string jsonData = "";
                    if (contractlist.Length > 0)
                    {
                        jsonData = Convert.ToString(restUtils.Post($"/api/orderdata/orderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{skucd}", contractlist).Result.Value);
                    }
                    else
                    {
                        jsonData = Convert.ToString(restUtils.Post($"/api/orderdata/orderheader/{branchcode}/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{skucd}", "").Result.Value);
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
                                         orddate = header.ordilja,
                                         header.ord_supplyamt,
                                         prodcnt = header.skucnt
                                     });
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
        [Authorize(Policy = "BizApiUser")]
        [Route("buyingheaderstat")]

        public async Task<ResponseClient> GetBuyingHeader()
        {
            try
            {
                string regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
                DateTime sdate = DateTime.Now.AddDays(-30);
                DateTime edate = DateTime.Now;
                string contractlist = "";
                string contractno = "%";

                HttpRestUtils restUtils = new HttpRestUtils(2, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<BuyingHeader> BuyingHeaders = new List<BuyingHeader>();
                    string jsonData = "";
                    jsonData = Convert.ToString(restUtils.Get($"/api/buying/buyingheader/%/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}").Result.Value);
                    BuyingHeaders = JsonConvert.DeserializeObject<List<BuyingHeader>>(jsonData);
                    if (BuyingHeaders.Count > 0)
                    {
                        var query = BuyingHeaders.Where(x => x.ordtp == "B");
                        var buydata = query.Select(x => new { x.ordamt, x.ordqty, x.buyymd });
                        var buydata2 = query.Select(x => new { x.buyamt, x.buyqty, x.buyymd });

                        var ord = buydata.GroupBy(x => x.buyymd)
                            .Select(x => new { ordamt = x.Sum(b => b.ordamt), ordqty = x.Sum(b => b.ordqty), orddate = x.Key }).ToList();
                        var buys = buydata2.GroupBy(x => x.buyymd)
                            .Select(x => new { buyamt = x.Sum(b => b.buyamt), buyqty = x.Sum(b => b.buyqty), buydate = x.Key }).ToList();

                        object[] xvalue = new object[2];
                        xvalue[0] = new { ord };
                        xvalue[1] = new { buys };
                        return ReturnResponce.ListReturnResponce(xvalue);
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



        #endregion
    }
}
