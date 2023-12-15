using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class BuyingDetail
    {
        public string skucd { get; set; }

        public string skunm { get; set; }

        public string unit { get; set; }

        public decimal? boxqty { get; set; }

        public decimal? buybox { get; set; }

        public decimal? buyqty { get; set; }

        public decimal? buy_supply { get; set; }

        public decimal? buy_supplyvat { get; set; }

        public decimal? buy_supplyamt { get; set; }
    }
}
