using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class OrderDto
    {
        public string ordilja { get; set; }
        public string ordno { get; set; }
        public string storecd { get; set; }
        public string ctrcd { get; set; }
        public string ctrnm { get; set; }
        public string eta { get; set; }
        public decimal? ord_supply { get; set; }
        public decimal? ord_supplyvat { get; set; }
        public decimal? ord_supplyamt { get; set; }
        public int? skucnt { get; set; }
        public int? ISSEEN { get; set; }
        public decimal? ORDERID { get; set; }
        public string seenuser { get; set; }
        public string APPROVEDUSER { get; set; }
        public DateTime? SEENDATE { get; set; }
        public DateTime? APPROVEDDATE { get; set; }
    }
}
