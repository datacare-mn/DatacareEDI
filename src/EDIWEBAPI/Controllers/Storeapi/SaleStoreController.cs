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
    [Route("api/salestore")]
    public class SaleStoreController : Controller
    {

        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<SaleStoreController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public SaleStoreController(OracleDbContext context, ILogger<SaleStoreController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        /// <summary>
        ///	Дэлгүүрийн хэрэглэгчид нийт борлуулалтын мэдээллийг харуулж буй хэсэг
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Үүсгэсэн огноо : 2018-02-22
        /// </remarks>
        /// <param name="branchcode">Салбарын код</param>
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
        [Route("salesku/{branchcode}/{regno}/{contractno}/{beginDate}/{endDate}/{type}")]
        public async Task<ResponseClient> GetSaleSku(string branchcode, string regno, string contractno, DateTime beginDate, DateTime endDate, int type)
        {
            try
            {
                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId)),
                    RegNo = regno,
                    BranchCode = branchcode,
                    ContractNo = contractno,
                    BeginDate = beginDate,
                    EndDate = endDate,

                    Controller = "saledata",
                    Route = "storedata",
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
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        ///	Дэлгүүрийн хэрэглэгчид цагийн борлуулалтын мэдээллийг харуулж буй хэсэг
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Үүсгэсэн огноо : 2018-02-22
        /// </remarks>
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="beginDate">Эхлэх огноо</param>
        /// <param name="endDate">Дуусах огноо</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("saletime/{branchcode}/{contractno}/{beginDate}/{endDate}")]
        public async Task<ResponseClient> GetSaleTime(string branchcode, string contractno, DateTime beginDate, DateTime endDate)
        {
            try
            {
                branchcode = UsefulHelpers.ReplaceNull(branchcode);
                var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                var restUtils = new HttpRestUtils(comid, _dbContext);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

                return Logics.ReportLogic.GetSaleTime(restUtils, branchcode, contractno, beginDate, endDate).Result;
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

    }
}
