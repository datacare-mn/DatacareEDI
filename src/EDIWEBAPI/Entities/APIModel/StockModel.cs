using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class StockModel
    {
        public string CTRCD { get; set; }
        public string branch { get; set; }
        public string barcode { get; set; }
        public string prodname { get; set; }
        public string UNIT { get; set; }
        public string prodst { get; set; }
        public decimal? SUPPLY { get; set; }
        public decimal? STKQTY { get; set; }
        public decimal? STKAMT { get; set; }
    }
}
