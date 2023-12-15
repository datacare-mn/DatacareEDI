using EDIWEBAPI.Context;
using EDIWEBAPI.Controllers.SysManagement;
using EDIWEBAPI.Entities.DBModel.Payment;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/paymentreport")]
    public class PaymentReportController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<PaymentReportController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public PaymentReportController(OracleDbContext context, ILogger<PaymentReportController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion



        /// <summary>
        ///	#Санхүүгийн актын жагсаалт
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-02-21
        /// </remarks>
        /// <param name="storeid">Дэлгүүрийн id</param>
        /// <param name="regno">Байлгуулагын регистр</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize]
        [Route("paymentreport/{storeid}/{regno}/{sdate}/{edate}")]

        public ResponseClient GetPaymentReport(string regno, DateTime sdate, DateTime edate, int storeid)
        {
            try
            {
                if (regno == "null")
                    regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
                
                return Logics.BILogic.GetPaymentReport(_dbContext, _log, storeid, regno, sdate, edate);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpPost]
        [Authorize]
        [Route("approvepaymentreport")]
        public ResponseClient ApprovePaymentReport([FromBody]REQ_PAYMENT_REPORT param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            try
            {
                var regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
                var olderdata = _dbContext.REQ_PAYMENT_REPORT.Where(x => x.COMID == param.COMID && x.REGNO == regno && x.ATTACHFILE == null && 
                    x.STARTDATE == param.STARTDATE && x.ENDDATE == param.ENDDATE);
                if (olderdata != null)
                {
                    //  _dbContext.Entry(olderdata).State = System.Data.Entity.EntityState.Deleted;
                    // _dbContext.REQ_PAYMENT_REPORT.RemoveRange(olderdata);
                    _dbContext.REQ_PAYMENT_REPORT.RemoveRange(olderdata);
                    _dbContext.SaveChanges();
                }

                //param.ID = Convert.ToInt32(_dbContext.GetTableID("REQ_PAYMENT_REPORT"));
                param.ID = Convert.ToInt32(Logics.BaseLogic.GetNewId(_dbContext, "REQ_PAYMENT_REPORT"));
                Logics.BaseLogic.Insert(_dbContext, new REQ_PAYMENT_REPORT()
                {
                    ID = param.ID,
                    APPROVEDATE = DateTime.Now,
                    APPROVEDUSER = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId)),
                    AMOUNT = param.AMOUNT,
                    REGNO = regno,
                    STARTDATE = param.STARTDATE,
                    ENDDATE = param.ENDDATE,
                    COMID = param.COMID
                });

                //_dbContext.REQ_PAYMENT_REPORT.Add(reportdata);
                //_dbContext.SaveChanges();

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sdate"></param>
        /// <param name="edate"></param>
        /// <param name="regno"></param>
        /// <param name="comid"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("reportheaderdata/{sdate}/{edate}/{regno}/{comid}")]
        [Authorize]
        public ResponseClient GetReportHeader(DateTime sdate, DateTime edate, string regno, int comid)
        {
            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
            try
            {
                edate = edate.AddDays(1).AddSeconds(-1);
                if (orgType == (int)Enums.SystemEnums.ORGTYPE.Бизнес)
                    regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));

                return Logics.PaymentLogic.GetPaymentReport(_dbContext, _log, comid, regno, sdate, edate, 
                    orgType != (int)Enums.SystemEnums.ORGTYPE.Бизнес);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPut]
        [Authorize(Policy = "BizApiUser")]
        [Route("attachpaymentreportwithfile")]
        public async Task<ResponseClient> PostAttachFile(IFormFile uploadedFile, string json)
        {
            if (string.IsNullOrEmpty(json))
                return ReturnResponce.NotFoundResponce();

            try
            {
                var param = JsonConvert.DeserializeObject<REQ_PAYMENT_REPORT>(json);
                var rss = Attacher.File(_log, uploadedFile, "attachedfiles");
                if (!rss.Success)
                    return ReturnResponce.FailedMessageResponce("Үйлдэл хийхэд алдаа гарлаа дахин оролдоно уу!");

                var currentdata = _dbContext.REQ_PAYMENT_REPORT.FirstOrDefault(x => x.REGNO == param.REGNO && x.COMID == param.COMID && x.ATTACHFILE == null);
                if (currentdata == null)
                    return ReturnResponce.NotFoundResponce();

                currentdata.ATTACHFILE = Convert.ToString(rss.Value);
                currentdata.ATTACHUSER = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
                currentdata.DESCRIPTION = param.DESCRIPTION;
                currentdata.ATTACHDATE = DateTime.Now;

                Logics.BaseLogic.Update(_dbContext, currentdata);
                //_dbContext.Entry(currentdata).State = System.Data.Entity.EntityState.Modified;
                //_dbContext.SaveChanges();

                return ReturnResponce.ListReturnResponce(currentdata);
            }
            catch (Exception ex)
            {
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
        [Route("printstatuschange/{id}")]
        public async Task<ResponseClient> PrintStatusChange(int id)
        {
            if (id > 0)
            {
                var currentpayment = _dbContext.REQ_PAYMENT_REPORT.FirstOrDefault(x => x.ID == id);
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



        [HttpDelete]
        [Route("unapprovereport/{id}")]
        [Authorize("BizApiUser")]
        public ResponseClient Delete(int id)
        {
            var current = _dbContext.REQ_PAYMENT_REPORT.FirstOrDefault(x => x.ID == id && x.ATTACHFILE == null);
            if (current != null)
            {
                _dbContext.Entry(current).State = System.Data.Entity.EntityState.Deleted;
                _dbContext.SaveChanges();
                return ReturnResponce.SuccessMessageResponce("Амжилттай!");
            }
            else
                return ReturnResponce.NotFoundResponce();
        }


        [HttpDelete]
        [Route("unapprovereportstore/{id}")]
        [Authorize("StoreApiUser")]
        public ResponseClient UnApproveReport(int id)
        {
            var current = _dbContext.REQ_PAYMENT_REPORT.FirstOrDefault(x => x.ID == id && x.ATTACHFILE != null);
            if (current != null)
            {
                current.ATTACHFILE = null;
                current.ATTACHDATE = null;
                current.DESCRIPTION = null;
                _dbContext.Entry(current).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges();
                return ReturnResponce.ListReturnResponce(current);
            }
            else
                return ReturnResponce.NotFoundResponce();
        }
    }
}
    