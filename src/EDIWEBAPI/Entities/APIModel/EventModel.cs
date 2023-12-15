using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class EventModel
    {
        public string branch { get; set; }
        public string brand { get; set; }
        public string barcode { get; set; }
        public string prodname { get; set; }
        public decimal? supply { get; set; }
        public decimal? saleqty { get; set; }
        public decimal? saleamt { get; set; }
        public decimal? custqty { get; set; }
        public decimal? deduction { get; set; }
        public decimal? deductionpersale { get { return saleqty.HasValue && saleqty.Value != 0 ? deduction / saleqty : 0; } }
        public decimal? deductionpercustomer { get { return custqty.HasValue && custqty.Value != 0 ? deduction / custqty : 0; } }
    }
}
