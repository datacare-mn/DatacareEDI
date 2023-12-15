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

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/reportstoredata")]
    public class ReportStoreController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<ReportStoreController> _log;

        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public ReportStoreController(OracleDbContext context, ILogger<ReportStoreController> log)
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
        /// <param name="regno">Регистер №</param>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="beginDate">Эхлэх огноо</param>
        /// <param name="endDate">Дуусах огноо</param>
        /// <param name="type">1 = ДЭЛГЭРЭНГҮЙ, 2 = БАРААГААР, 3 = САЛБАРААР</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("eventreport/{regno}/{contractno}/{beginDate}/{endDate}/{type}")]
        public async Task<ResponseClient> GetEventData(string regno, string contractno, DateTime beginDate, DateTime endDate, int type)
        {
            try
            {
                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = UsefulHelpers.STORE_ID,
                    CompanyId = Logics.ManagementLogic.GetOrganization(_dbContext, regno).ID,
                    ContractNo = contractno,
                    BeginDate = beginDate,
                    EndDate = endDate,
                    Index = 7,
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
        [Authorize(Policy = "StoreApiUser")]
        [Route("nrmlreportdata/{comid}/{regno}/{sdate}/{reportindex}")]
        public async Task<ResponseClient> GetReportData(int comid, string regno, DateTime sdate, int reportindex)
        {
            try
            {
                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = comid,
                    CompanyId = Logics.ManagementLogic.GetOrganization(_dbContext, regno).ID,
                    ContractNo = null,
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
        [Authorize(Policy = "StoreApiUser")]
        [Route("itemreportdata/{comid}/{branchcode}/{contractno}/{skucd}/{year}")]
        public async Task<ResponseClient> GetItemReportData(int comid, string branchcode, string contractno, string skucd, string year)
        {
            var restUtils = new HttpRestUtils(comid, _dbContext);
            if (!restUtils.StoreServerConnected)
                return ReturnResponce.FailedMessageResponce("StoreAPI not connected!");

            var response = restUtils.Get($"/api/reportdata/getitemreportdata/{branchcode}/{skucd}/{contractno}/{year}").Result;
            if (!response.Success)
                return response;

            var jsonData = Convert.ToString(response.Value);
            var header = JsonConvert.DeserializeObject<List<object>>(jsonData);

            return ReturnResponce.ListReturnResponce(header);

        }



        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("getskulist/{regno}")]
        public async Task<ResponseClient> GetSkuListData(string regno)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var restUtils = new HttpRestUtils(comid, _dbContext);
            if (!restUtils.StoreServerConnected)
                return ReturnResponce.FailedMessageResponce("StoreAPI not connected!");

            var response = restUtils.Get($"/api/reportdata/getskulist/{regno}").Result;
            if (!response.Success)
                return response;

            var jsonData = Convert.ToString(response.Value);
            var header = JsonConvert.DeserializeObject<List<object>>(jsonData);

            return ReturnResponce.ListReturnResponce(header);

        }
    }
}
