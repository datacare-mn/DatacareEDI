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
    [Route("api/buying")]
    public class BuyingController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<BuyingController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public BuyingController(OracleDbContext context, ILogger<BuyingController> log)
        {
            _dbContext = context;
            _log = log;
        }

        #endregion

        /// <summary>
        ///	#Татан авалтын мэдээлэл
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-03-07
        /// </remarks>
        /// <param name="contractno">Гэрээ №</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="comid">Дэлгүүр ID</param>
        /// <param name="buyingstatus">Buying status 1 = buying, 0 = Буцаалт, null = Бүгд </param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("buyingheader/{contractno}/{sdate}/{edate}/{comid}/{buyingstatus}")]
        public async Task<ResponseClient> GetBuyingHeader(string contractno, DateTime sdate, DateTime edate, int comid, string buyingstatus)
        {
            try
            {
                var regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyReg));
                return Logics.BILogic.GetBuyingHeader(_dbContext, comid, regno, contractno, sdate, edate, buyingstatus);
            }
            catch (Exception ex)
            {
                var methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }

        [HttpGet]
        [Authorize]
        [Route("buyingdetail/{storecode}/{v_buyymd}/{v_buyno}/{comid}")]

        public async Task<ResponseClient> GetBuyingDetail(string storecode, string v_buyymd, string v_buyno, int comid)
        {
            try
            {
                HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<BuyingDetail> BuyingDetails = new List<BuyingDetail>();
                    string jsonData = "";
                    jsonData = Convert.ToString(restUtils.Get($"/api/buying/buyingdetail/{storecode}/{v_buyymd}/{v_buyno}").Result.Value);
                    BuyingDetails = JsonConvert.DeserializeObject<List<BuyingDetail>>(jsonData);
                    if (BuyingDetails.Count > 0)
                    {
                    return ReturnResponce.ListReturnResponce(BuyingDetails);
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


    }
}
