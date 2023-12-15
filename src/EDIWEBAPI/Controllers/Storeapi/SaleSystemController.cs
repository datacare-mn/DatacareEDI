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
    public class SaleSystemController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<SaleSystemController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public SaleSystemController(OracleDbContext context, ILogger<SaleSystemController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        /// <summary>
        ///	Системийн хэрэглэгчид салбарын борлуулалтын мэдээллийг харуулж буй хэсэг
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-02-22
        /// </remarks>
        /// <param name="comid">Дэлгүүр №</param>
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="saledate">Огноо</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("salebranch/{comid}/{branchcode}/{contractno}/{saledate}")]

        public async Task<ResponseClient> GetSaleBranch(int comid, string branchcode, string contractno, DateTime saledate)
        {
            try
            {
                if (branchcode == "null")
                {
                    branchcode = "%";
                }
                HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<SaleBranch> SaleBranchData = new List<SaleBranch>();
                    string jsonData = Convert.ToString(restUtils.Get($"/api/saledata/salebranch/{branchcode}/{contractno}/{saledate.ToString("yyyyMMdd")}").Result.Value);
                    SaleBranchData = JsonConvert.DeserializeObject<List<SaleBranch>>(jsonData);
                    return ReturnResponce.ListReturnResponce(SaleBranchData);
                }
                else
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }


        /// <summary>
        ///	Системийн хэрэглэгчид Барааны борлуулалтын мэдээллийг харуулж буй хэсэг
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-02-22
        /// </remarks>
        /// <param name="comid">Дэлгүүр №</param>
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="saledate">Огноо</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("salesku/{comid}/{branchcode}/{contractno}/{saledate}")]

        public async Task<ResponseClient> GetSaleSku(int comid, string branchcode, string contractno, DateTime saledate)
        {
            try
            {
                if (branchcode == "null")
                {
                    branchcode = "%";
                }
                HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<SaleSku> SaleSkuData = new List<SaleSku>();
                    string jsonData = Convert.ToString(restUtils.Get($"/api/saledata/salesku/{branchcode}/{contractno}/{saledate.ToString("yyyyMMdd")}").Result.Value);
                    SaleSkuData = JsonConvert.DeserializeObject<List<SaleSku>>(jsonData);
                    //  return ReturnResponce.ListReturnResponce(OrderHeader);
                    return ReturnResponce.ListReturnResponce(SaleSkuData);
                }
                else
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

            }

            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }


        /// <summary>
        ///	Системийн хэрэглэгчид нийт борлуулалтын мэдээллийг харуулж буй хэсэг
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-02-22
        /// </remarks>
        /// <param name="comid">Дэлгүүр №</param>
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="saledate">Огноо</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        

        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("saletotal/{comid}/{branchcode}/{contractno}/{saledate}")]

        public async Task<ResponseClient> GetSaleTot(int comid, string branchcode, string contractno, DateTime saledate)
        {
            try
            {
                if (branchcode == "null")
                {
                    branchcode = "%";
                }
                HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<SaleTotal> SaleTotalData = new List<SaleTotal>();
                    string jsonData = Convert.ToString(restUtils.Get($"/api/saledata/saletot/{branchcode}/{contractno}/{saledate.ToString("yyyyMMdd")}").Result.Value);
                    SaleTotalData = JsonConvert.DeserializeObject<List<SaleTotal>>(jsonData);
                    //  return ReturnResponce.ListReturnResponce(OrderHeader);
                    return ReturnResponce.ListReturnResponce(SaleTotalData);
                }
                else
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

            }

            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }


        /// <summary>
        ///	Системийн хэрэглэгчид цагийн борлуулалтын мэдээллийг харуулж буй хэсэг
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-02-22
        /// </remarks>
        /// <param name="comid">Дэлгүүр №</param>
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="saledate">Огноо</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("saletime/{comid}/{branchcode}/{contractno}/{saledate}")]

        public async Task<ResponseClient> GetSaleTime(int comid, string branchcode, string contractno, DateTime saledate)
        {
            try
            {
                if (branchcode == "null")
                {
                    branchcode = "%";
                }
                HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<SaleTime> SaletimeData = new List<SaleTime>();
                    string jsonData = Convert.ToString(restUtils.Get($"/api/saledata/saletime/{branchcode}/{contractno}/{saledate.ToString("yyyyMMdd")}").Result.Value);
                    SaletimeData = JsonConvert.DeserializeObject<List<SaleTime>>(jsonData);
                    //  return ReturnResponce.ListReturnResponce(OrderHeader);
                    return ReturnResponce.ListReturnResponce(SaletimeData);
                }
                else
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }







    }
}
