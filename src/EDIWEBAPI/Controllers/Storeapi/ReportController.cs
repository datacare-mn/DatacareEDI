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
using EDIWEBAPI.Attributes;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/reportdata")]
    public class ReportController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<ReportController> _log;

        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public ReportController(OracleDbContext context, ILogger<ReportController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        /// <summary>
        ///	УРАМШУУЛЛЫН ТАЙЛАН
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Үүсгэсэн огноо : 2018-02-22
        /// </remarks>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="beginDate">Эхлэх огноо</param>
        /// <param name="endDate">Дуусах огноо</param>
        /// <param name="type">1 = ДЭЛГЭРЭНГҮЙ, 2 = БАРААГААР, 3 = САЛБАРААР</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("eventreport/{contractno}/{beginDate}/{endDate}/{type}")]
        public async Task<ResponseClient> GetEventData(string contractno, DateTime beginDate, DateTime endDate, int type)
        {
            try
            {
                var reportIndex = 7;
                var businessid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                // LOGFILTER АШИГЛАХГҮЙ БАЙГАА УЧИР НЬ REPORTINDEX - ЭЭРЭЭ СҮҮЛДЭЭ ЯЛГАРАХ ШААРДЛАГАТАЙ БАЙГАА
                Logics.ManagementLogic.RequestLog(_dbContext, _log, HttpContext, "ReportData", reportIndex.ToString(),
                    $"[type : {type}, contractno : {contractno}, sdate : {beginDate}]");

                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = UsefulHelpers.STORE_ID,
                    CompanyId = businessid,
                    ContractNo = contractno,
                    BeginDate = beginDate,
                    EndDate = endDate,
                    Index = reportIndex,
                    Controller = "",
                    Route = ""
                };
                return Logics.ReportLogic.GetEventReport(_dbContext, type, request).Result;
                //return Logics.ReportLogic.GetBetweenReportData<object>(_dbContext, request).Result;
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("nrmlreportdata/{comid}/{contractno}/{sdate}/{reportindex}")]
        public async Task<ResponseClient> GetReportData(int comid, string contractno, DateTime sdate, int reportindex)
        {
            try
            {
                var businessid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                // LOGFILTER АШИГЛАХГҮЙ БАЙГАА УЧИР НЬ REPORTINDEX - ЭЭРЭЭ СҮҮЛДЭЭ ЯЛГАРАХ ШААРДЛАГАТАЙ БАЙГАА
                Logics.ManagementLogic.RequestLog(_dbContext, _log, HttpContext, "ReportData", reportindex.ToString(),
                    $"[comid : {comid}, contractno : {contractno}, sdate : {sdate}]");

                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = comid,
                    CompanyId = businessid,
                    ContractNo = contractno,
                    BeginDate = sdate,
                    Index = reportindex,
                    Controller = "",
                    Route = ""
                };
                return Logics.ReportLogic.GetReportData<object>(_dbContext, request).Result;
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
       }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("itemreportdata/{comid}/{branchcode}/{contractno}/{skucd}/{year}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetItemReportData(int comid, string branchcode, string contractno, string skucd, string year)
        {
            try
            {
                //Logics.ManagementLogic.RequestLog(_dbContext, _log, HttpContext, "ReportData", "ItemReportData",
                //    $"[comid : {comid}, branchcode : {branchcode}, contractno : {contractno}, skucd: {skucd}, year : {year}]");

                var restUtils = new HttpRestUtils(comid, _dbContext);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedMessageResponce("StoreAPI not connected!");

                var storeResponse = restUtils.Get($"/api/reportdata/getitemreportdata/{branchcode}/{skucd}/{contractno}/{year}").Result;
                if (!storeResponse.Success)
                    return storeResponse;

                var jsonData = Convert.ToString(storeResponse.Value);
                var header = JsonConvert.DeserializeObject<List<object>>(jsonData);

                return ReturnResponce.ListReturnResponce(header);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getskulist/{comid}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetSkuListData(int comid)
        {
            try
            {
                //Logics.ManagementLogic.RequestLog(_dbContext, _log, HttpContext, "ReportData", "Getskulist",
                //    $"[comid : {comid}]");

                var regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
                var restUtils = new HttpRestUtils(comid, _dbContext);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedMessageResponce("StoreAPI not connected!");

                var storeResponse = restUtils.Get($"/api/reportdata/getskulist/{regno}").Result;
                if (!storeResponse.Success)
                    return storeResponse;
                
                var jsonData = Convert.ToString(restUtils.Get($"/api/reportdata/getskulist/{regno}").Result.Value);
                var header = JsonConvert.DeserializeObject<List<object>>(jsonData);

                return ReturnResponce.ListReturnResponce(header);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

    }
}
