using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class PaymentHeader
    {
        public string strymd { get; set; }

        public string stpymd { get; set; }

        public string ctrcd { get; set; }

        public string ctrnm { get; set; }

        public decimal? amt { get; set; }

        public decimal? frbtamt { get; set; }

        public decimal? ifrbtamt { get; set; }

        public decimal? evnamt { get; set; }

        public decimal? normalstk { get; set; }

        public decimal? payamt { get; set; }

        public string PBGB { get; set; }

        public string edi { get; set; }

        public string banknm { get; set; }

        public string accno { get; set; }

        public decimal? penamt { get; set; }

        public decimal? crdfee { get; set; }

        public decimal? invamt { get; set; }

        public string paycycle { get; set; }
    }
}
