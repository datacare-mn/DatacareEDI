using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class OrderHeader
    {
        public string ordilja { get; set; } //order date

        public string ordno { get; set; } // orderno

        [JsonProperty(PropertyName = "storecd")]
        public string jumcd { get; set; } //storename

        public string ctrcd { get; set; } //contractcode

        public string ctrnm { get; set; } //contractname

        public string eta { get; set; } //xurgex ognoo

        public decimal? ord_supply { get; set; } //vatgui dun

        public decimal? ord_supplyvat { get; set; } //vat

        public decimal? ord_supplyamt { get; set; } //vatgui dun

        public int? skucnt { get; set; } //sku count
    }
}
