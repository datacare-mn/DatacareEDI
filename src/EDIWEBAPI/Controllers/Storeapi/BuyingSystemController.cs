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
    public class BuyingSystemController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<BuyingSystemController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public BuyingSystemController(OracleDbContext context, ILogger<BuyingSystemController> log)
        {
            _dbContext = context;
            _log = log;
        }

        #endregion


        #region BuyingHeader
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
        /// <param name="regno">Байгууллагын регистр</param>
        /// <param name="comid">Дэлгүүр ID</param>
        /// <param name="buyingstatus">Buying status 1 = buying, 0 = Буцаалт, null = Бүгд </param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>



        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("buyingheader/{contractno}/{sdate}/{edate}/{regno}/{comid}/{buyingstatus}")]


        public async Task<ResponseClient> GetBuyingHeader(string contractno, DateTime sdate, DateTime edate, string regno, int comid, string buyingstatus)
        {
            try
            {
               
                string contractlist = "";
                if (contractno == "null")
                {
                    contractno = "%";
                }
                HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<BuyingHeader> BuyingHeaders = new List<BuyingHeader>();
                    string jsonData = "";
                    jsonData = Convert.ToString(restUtils.Get($"/api/buying/buyingheader/%/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}").Result.Value);
                    BuyingHeaders = JsonConvert.DeserializeObject<List<BuyingHeader>>(jsonData);
                    if (BuyingHeaders.Count > 0)
                    {
                        if (buyingstatus == "1")
                        {
                            var query = BuyingHeaders.Where(x => x.ordtp == "B");
                            return ReturnResponce.ListReturnResponce(query);
                        }
                        if (buyingstatus == "0")
                        {
                            var query = BuyingHeaders.Where(x => x.ordtp == "R");
                            return ReturnResponce.ListReturnResponce(query);
                        }
                        else
                        {
                            return ReturnResponce.ListReturnResponce(BuyingHeaders);
                        }

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


        #endregion
    }
}
