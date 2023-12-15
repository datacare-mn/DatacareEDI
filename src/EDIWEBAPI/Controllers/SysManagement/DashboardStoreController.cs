using EDIWEBAPI.Context;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/dashboardstore")]
    public class DashboardStoreController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<DashboardStoreController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public DashboardStoreController(OracleDbContext context, ILogger<DashboardStoreController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        [HttpGet]
        [Route("productplan/{departmentId}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetProductPlan(int departmentId)
        {
            var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            try
            {
                return Logics.MasterLogic.GetProductPlan(_dbContext, _log, ORGTYPE.Дэлгүүр, 0, userId, departmentId);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpGet]
        [Route("productperformance/{duration}/{departmentId}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetProductPerformation(int duration, int departmentId)
        {
            var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            try
            {
                var beginDate = DateTime.Now.AddDays(-duration);
                return Logics.MasterLogic.GetProductPerformance(_dbContext, _log, ORGTYPE.Дэлгүүр, 0, userId, departmentId, beginDate);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("getProperties/{beginDate}/{endDate}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetProperties(DateTime beginDate, DateTime endDate)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                var fullEndDate = UsefulHelpers.ConvertDate(endDate);
                var failedPayments = _dbContext.REQ_PAYMENT.Where(a => beginDate <= a.APPROVEDDATE && a.APPROVEDDATE <= fullEndDate && a.ATTACHDATE == null).Count();

                var canceled = _dbContext.SYSTEM_MAIL_LOG.Where(a => a.TYPE == Enums.SystemEnums.MessageType.Payment
                                && beginDate <= a.REQUESTDATE && a.REQUESTDATE <= fullEndDate).Count();

                var monthly = Logics.PaymentLogic.GetPaymentReport(_dbContext, _log, comid, "null", beginDate, fullEndDate, true);
                var daily = Logics.PaymentLogic.GetPaymentReport(_dbContext, _log, comid, "null", endDate, fullEndDate, true);

                return ReturnResponce.ListReturnResponce(new
                {
                    FAILED = failedPayments,
                    CANCELED = canceled,
                    MONTH = monthly.RowCount,
                    DAILY = daily.RowCount
                });
                //return ReturnResponce.SuccessMessageResponce("");
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("getPayments/{beginDate}/{endDate}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetPayments(DateTime beginDate, DateTime endDate)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                endDate = UsefulHelpers.ConvertDate(endDate);
                var response = Logics.PaymentLogic.GetStorePayment(_dbContext, _log, comid, "null", "null", beginDate, endDate, true, true);
                if (!response.Success || response.Value == null)
                    return ReturnResponce.NotFoundResponce();

                var values = (List<EDIWEBAPI.Entities.APIModel.PaymentDto>)response.Value;
                var results = (from v in values
                               group v by v.paycycle into g
                               select new
                               {
                                   PAYCYCLE = g.Key,
                                   COUNT = g.Count()
                               }).ToList();
                
                return ReturnResponce.ListReturnResponce(results);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("getProducts/{beginDate}/{endDate}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetProducts(DateTime beginDate, DateTime endDate)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                endDate = UsefulHelpers.ConvertDate(endDate);
                var products = (from p in _dbContext.REQ_PRODUCT
                                join r in _dbContext.MST_PRODUCT_REQUEST on p.REQUESTID equals r.ID
                                where p.ENABLED == 1 && beginDate <= p.REQUESTDATE && p.REQUESTDATE <= endDate
                                group p by new
                                {
                                    TODAY = endDate.Year == p.REQUESTDATE.Year && endDate.Month == p.REQUESTDATE.Month && endDate.Day == p.REQUESTDATE.Day ? 1 : 0,
                                    r.NAME
                                } into g
                                select new
                                {
                                    TODAY = g.Key.TODAY,
                                    NAME = g.Key.NAME,
                                    COUNT = g.Count()
                                }).ToList();

                var sums = (from p in products
                            group p by new { p.NAME } into g
                            select new
                            {
                                g.Key.NAME,
                                TODAY = g.Sum(a => a.TODAY == 1 ? a.COUNT : 0),
                                WEEK = g.Sum(a => a.COUNT)
                            }).ToList();

                return ReturnResponce.ListReturnResponce(sums);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("getOrders/{currentDate}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetOrders(DateTime currentDate)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                var dateInString = currentDate.ToString("yyyyMMdd");
                var orders = _dbContext.REQ_ORDER.Where(a => a.ORDERDATE == dateInString);

                return ReturnResponce.ListReturnResponce(new
                {
                    COUNT = orders.Count(),
                    SEEN = orders.Count(a => a.ISSEEN == 1),
                    APPROVED = orders.Count(a => a.APPROVEDDATE.HasValue)
                });
                //var mails = ((from m in _dbContext.REQ_ORDER
                //              where m.ORDERDATE == dateInString
                //              group m by m.ORDERDATE into g
                //              select new
                //              {
                //                  TYPE = "ORDER",
                //                  COUNT = g.Count(),
                //                  SEEN = g.Sum(a => a.ISSEEN == 1 ? 1 : 0),
                //                  APPROVED = g.Sum( a=> a.APPROVEDUSER.HasValue ? 1 : 0)
                //              })
                //              .Union
                //              (from m in _dbContext.REQ_RETURN_ORDER
                //               where m.ORDERDATE == dateInString
                //               group m by m.ORDERDATE into g
                //               select new
                //               {
                //                   TYPE = "RETURN",
                //                   COUNT = g.Count(),
                //                   SEEN = g.Sum(a => a.ISSEEN == 1 ? 1 : 0),
                //                   APPROVED = g.Sum(a => a.APPROVEDUSER.HasValue ? 1 : 0)
                //               })).ToList();

                //return ReturnResponce.ListReturnResponce(mails);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("getMails/{beginDate}/{endDate}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetMails(DateTime beginDate, DateTime endDate)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                endDate = UsefulHelpers.ConvertDate(endDate);

                var mails = ((from m in _dbContext.SYSTEM_MAIL_LOG
                              where m.STOREID == comid && (m.TYPE == Enums.SystemEnums.MessageType.Order || m.TYPE == Enums.SystemEnums.MessageType.Return)
                                 && beginDate <= m.REQUESTDATE && m.REQUESTDATE <= endDate
                              group m by new
                              {
                                  TODAY = endDate.Year == m.REQUESTDATE.Value.Year && endDate.Month == m.REQUESTDATE.Value.Month && endDate.Day == m.REQUESTDATE.Value.Day ? 1 : 0,
                                  m.ISSEND,
                                  m.TYPE
                              } into g
                              select new
                              {
                                  TYPE = "MAIL",
                                  TODAY = g.Key.TODAY,
                                  ISSEND = g.Key.ISSEND,
                                  SUBJECT = g.Key.TYPE == Enums.SystemEnums.MessageType.Order ? "ORDER" : "RETURN",
                                  COUNT = g.Count()
                              })
                              .Union
                              (from m in _dbContext.SYSTEM_MESSAGE_ARCHIVE
                               where m.STOREID == comid && (m.TYPE == Enums.SystemEnums.MessageType.Order || m.TYPE == Enums.SystemEnums.MessageType.Return)
                                  && beginDate <= m.REQUESTDATE && m.REQUESTDATE <= endDate
                               group m by new
                               {
                                   TODAY = endDate.Year == m.REQUESTDATE.Value.Year && endDate.Month == m.REQUESTDATE.Value.Month && endDate.Day == m.REQUESTDATE.Value.Day ? 1 : 0,
                                   m.ISSENT,
                                   m.TYPE
                               } into g
                               select new
                               {
                                   TYPE = "MESSAGE",
                                   TODAY = g.Key.TODAY,
                                   ISSEND = g.Key.ISSENT,
                                   SUBJECT = g.Key.TYPE == Enums.SystemEnums.MessageType.Order ? "ORDER" : "RETURN",
                                   COUNT = g.Count()
                               })).ToList();

                //var todays = (from m in mails
                //              group m by new { m.TYPE, m.SUBJECT } into g
                //              select new
                //              {
                //                  TYPE = g.Key.TYPE,
                //                  SUBJECT = g.Key.SUBJECT == Enums.SystemEnums.MessageType.Order ? "ORDER" : "RETURN",
                //                  SUCCESSFUL_TODAY = g.Sum(a => a.ISSEND == 1 ? a.TODAY * a.COUNT : 0),
                //                  SUCCESSFUL_TOTAL = g.Sum(a => a.ISSEND == 1 ? a.COUNT : 0),
                //                  UNSUCCESSFUL_TODAY = g.Sum(a => a.ISSEND != 1 ? a.TODAY * a.COUNT : 0),
                //                  UNSUCCESSFUL_TOTAL = g.Sum(a => a.ISSEND != 1 ? a.COUNT : 0)
                //              }).ToList();

                return ReturnResponce.ListReturnResponce(mails);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        #region Get
        [HttpGet]
        [Route("countstat")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetCountStat()
        {
            var todayattachedpaymentcount = _dbContext.REQ_PAYMENT.Where(x=> 
                    x.ATTACHDATE.Value.Year ==  DateTime.Today.Year &&
                    x.ATTACHDATE.Value.Month == DateTime.Today.Month &&
                    x.ATTACHDATE.Value.Day == DateTime.Today.Day).Count();

            var todaystoreprintpaymentcount = _dbContext.REQ_PAYMENT.Where(x =>
                x.ATTACHDATE.Value.Year == DateTime.Today.Year &&
                x.ATTACHDATE.Value.Month == DateTime.Today.Month &&
                x.ATTACHDATE.Value.Day == DateTime.Today.Day && x.STOREPRINT == 1).Count();

            var todayattachedreportcount= _dbContext.REQ_PAYMENT_REPORT.Where(x =>
                    x.ATTACHDATE.Value.Year == DateTime.Today.Year &&
                    x.ATTACHDATE.Value.Month == DateTime.Today.Month &&
                    x.ATTACHDATE.Value.Day == DateTime.Today.Day).Count();

          var todaysendordercount =  _dbContext.REQ_ORDER.Where(x =>
                    x.CREATEDDATE.Value.Year == DateTime.Today.Year &&
                    x.CREATEDDATE.Value.Month == DateTime.Today.Month &&
                    x.CREATEDDATE.Value.Day == DateTime.Today.Day).Count();

            var todayseenordercount = _dbContext.REQ_ORDER.Where(x =>
                     x.CREATEDDATE.Value.Year == DateTime.Today.Year &&
                     x.CREATEDDATE.Value.Month == DateTime.Today.Month &&
                     x.CREATEDDATE.Value.Day == DateTime.Today.Day && x.ISSEEN ==1).Count();

            object[] xvalue = new object[1];
            xvalue[0] = new {
                todayattachedpaymentcount = todayattachedpaymentcount,
                todaystoreprintpaymentcount = todaystoreprintpaymentcount,
                todayattachedreportcount = todayattachedreportcount,
                todaysendordercount = todaysendordercount,
                todayseenordercount = todayseenordercount
            };
            return ReturnResponce.ListReturnResponce(xvalue);
        }


        [HttpGet]
        [Route("countstatmonth")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetCountStatMonth()
        {

          return ReturnResponce.ListReturnResponce( _dbContext.DASH_STORE_MONTH_DATA());

        }


        [HttpGet]
        [Route("lastrequest")]
        [Authorize(Policy = "StoreApiUser")]
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
        #endregion




    }
}
