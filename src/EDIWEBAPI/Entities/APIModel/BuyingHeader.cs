using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class BuyingHeader
    {
        public string storename { get; set; }

        public string storecd { get; set; }

        public string buyymd { get; set; }

        public string buyno { get; set; }

        public string ordtp { get; set; }

        public string ctrcd { get; set; }

        public string ctrnm { get; set; }

        public string regno { get; set; }

        public int? ordqty { get; set; }

        public decimal? ordamt { get; set; }

        public decimal? buyqty { get; set; }

        public decimal? buyamt { get; set; }
    }
}
