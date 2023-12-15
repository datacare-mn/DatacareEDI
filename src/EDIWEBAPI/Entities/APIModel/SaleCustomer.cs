using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class SaleCustomer
    {
        public string date { get; set; }
        public string ctrcd { get; set; }
        public string barcode { get; set; }
        public decimal? custcnt { get; set; }
        public decimal? custgy { get; set; }
        public decimal? custavgsale { get; set; }
        public decimal? custsalegy { get; set; }
        public decimal? mbrcnt { get; set; }
        public decimal? mbravgsale { get; set; }
        public decimal? mbrsalegy { get; set; }
        public decimal? samecatcd { get; set; }
        public decimal? fcast { get; set; }
    }
}
