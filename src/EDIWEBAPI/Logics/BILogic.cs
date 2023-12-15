using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Context;
using EDIWEBAPI.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EDIWEBAPI.Entities.APIModel;

namespace EDIWEBAPI.Logics
{
    public class BILogic : BaseLogic
    {
        public static ResponseClient GetPaymentReport(OracleDbContext _dbContext, ILogger _log, int storeid, string regno, DateTime sdate, DateTime edate)
        {
            // PAYMENTREPORTCONTROLLER.GetPaymentReport
            var rstapp = new SystemRestAppUtils(_dbContext, _log, "ACCOUNT", storeid);
            if (!rstapp.ServerConnected)
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("");

            var response = rstapp.Get(_log, $"/api/Accounting/getcustbaldetailed/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}").Result;
            response.RowCount = response.Value != null ? response.Value.ToString().Split('{').Count() - 1 : 0;
            return response;
        }

        public static ResponseClient GetBuyingHeader(OracleDbContext _dbContext, int storeid, string regno, string contractno,
            DateTime sdate, DateTime edate, string buyingstatus)
        {
            contractno = UsefulHelpers.ReplaceNull(contractno);

            var restUtils = new HttpRestUtils(storeid, _dbContext);
            if (!restUtils.StoreServerConnected)
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

            var response = restUtils.Get($"/api/buying/buyingheader/%/{contractno}/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}").Result;
            if (!response.Success)
                return response;

            var jsonData = Convert.ToString(response.Value);
            var headers = JsonConvert.DeserializeObject<List<BuyingHeader>>(jsonData);
            if (headers == null || headers.Count == 0)
                return ReturnResponce.NotFoundResponce();

            // ЗАСАЛТ
            if (buyingstatus == "2")
            {
                //var query = headers.Where(x => x.ordtp == "B");
                var query = headers.Where(x => !x.ordqty.HasValue || x.ordqty.Value == 0);
                return ReturnResponce.ListReturnResponce(query.ToList());
            }
            // ХУДАЛДАН АВАЛТ
            if (buyingstatus == "1")
            {
                var query = headers.Where(x => x.ordqty.HasValue && x.ordqty.Value > 0);
                //var query = headers.Where(x => x.ordtp == "B");
                return ReturnResponce.ListReturnResponce(query.ToList());
            }
            // БУЦААЛТ
            if (buyingstatus == "0")
            {
                var query = headers.Where(x => x.ordqty.HasValue && x.ordqty.Value < 0);
                //var query = headers.Where(x => x.ordtp == "R");
                return ReturnResponce.ListReturnResponce(query.ToList());
            }
            
            return ReturnResponce.ListReturnResponce(headers.ToList());
            
        }
    }
}
