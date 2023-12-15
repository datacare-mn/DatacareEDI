using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class ReturnOrderDetail
    {
        public string jumcd { get; set; }
        public string ilja { get; set; }

        public string skucd { get; set; } //sku code

        public string skunm { get; set; } //skuname

        public string unit { get; set; } //xemjix negj

        public decimal? boxqty { get; set; } //xairtsag dotorx too

        public decimal? ordbox { get; set; } //zaxialsan xairtsagnii too

        public decimal? order_qty { get; set; } //zaxialsan too

        public decimal? ord_supply { get; set; } //zaxialsan vatgui une

        public decimal? ord_supplyvat { get; set; } //vat

        public decimal? ord_supplyamt { get; set; }

        public string reason { get; set; }

        public string contxt { get; set; }
    }
}
