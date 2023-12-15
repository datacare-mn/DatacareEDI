using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class PaymentHeaderReturn
    {
        public int? ID { get; set; }
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

        public string STPYMD { get; set; }

        public string CONTRACTNO { get; set; }

        public string ATTACHFILE { get; set; }

        public string DESCRIPTION { get; set; }

        public int? APPROVEDUSER { get; set; }

        public DateTime? APPROVEDDATE { get; set; }

        public int? STORESEEN { get; set; }
    }
}
