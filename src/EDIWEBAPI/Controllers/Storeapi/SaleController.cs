using EDIWEBAPI.Attributes;
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
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/sale")]
    public class SaleController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<SaleController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public SaleController(OracleDbContext context, ILogger<SaleController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        /// <summary>
        ///	Борлуулалтын мэдээллийг харуулж буй хэсэг
        /// </summary>
        /// <param name="comid">Дэлгүүр №</param>
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="beginDate">Эхлэх огноо</param>
        /// <param name="endDate">Дуусах огноо</param>
        /// <param name="type">1 = ДЭЛГЭРЭНГҮЙ, 2 = БАРААГААР, 3 = САЛБАРААР</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize]
        [Route("salesku/{comid}/{branchcode}/{contractno}/{beginDate}/{endDate}/{type}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetSaleSku(int comid, string branchcode, string contractno, DateTime beginDate, DateTime endDate, int type)
        {
            try
            {
                var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                Logics.ReportLogic.CheckReportQty(_dbContext, _log, "Sale", "GetSaleSku", userid);

                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = UsefulHelpers.STORE_ID,
                    CompanyId = comid,
                    BranchCode = branchcode,
                    ContractNo = contractno,
                    BeginDate = beginDate,
                    EndDate = endDate,
                    Controller = "saledata",
                    Route = "data",
                    Index = 3
                };
                var result = Logics.ReportLogic.GetSaleData<SaleTotal>(_dbContext, _log, request).Result;
                if (result.Success && result.RowCount > 0 && type != 1)
                {
                    return Logics.ReportLogic.GetSale((List<SaleTotal>)result.Value, type).Result;
                }
                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex, false);
            }
        }

        /// <summary>
        ///	Бизнесийн хэрэглэгчид цагийн борлуулалтын мэдээллийг харуулж буй хэсэг
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-02-22
        /// </remarks>
        /// <param name="comid">Дэлгүүр №</param>
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="beginDate">Эхлэх огноо</param>
        /// <param name="endDate">Дуусах огноо</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize]
        [Route("saletime/{comid}/{branchcode}/{contractno}/{beginDate}/{endDate}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetSaleTime(int comid, string branchcode, string contractno, DateTime beginDate, DateTime endDate)
        {
            try
            {
                var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                Logics.ReportLogic.CheckReportQty(_dbContext, _log, "Sale", "GetSaleTime", userid);

                branchcode = UsefulHelpers.ReplaceNull(branchcode);
                var restUtils = new HttpRestUtils(comid, _dbContext);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

                return Logics.ReportLogic.GetSaleTime(restUtils, branchcode, contractno, beginDate, endDate).Result;
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex, false);
            }
        }
    }
}
