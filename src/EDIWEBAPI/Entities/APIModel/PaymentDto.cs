using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class PaymentDto
    {
        public int TYPE { get; set; }
        public string REASON { get; set; }
        public decimal? amt { get; set; }
        public string ctrcd{ get; set; }
        public string ctrnm { get; set; }
        public string edi { get; set; }
        public decimal? evnamt { get; set; }
        public decimal? frbtamt { get; set; }
        public decimal? ifrbtamt { get; set; }
        public decimal? normalstk { get; set; }
        public decimal? payamt { get; set; }
        public string PBGB { get; set; }
        public string stpymd { get; set; }
        public string strymd { get; set; }
        public string banknm { get; set; }
        public string accno { get; set; }
        public decimal? penamt { get; set; }
        public decimal? crdfee { get; set; }
        public decimal? invamt { get; set; }
        public string paycycle { get; set; }
        public DateTime? APPROVEDDATE { get; set; }
        public string APPROVEDUSER { get; set; }
        public string ATTACHFILE { get; set; }
        public DateTime? ATTACHDATE { get; set; }
        public string CONTRACTNO { get; set; }
        public string DESCRIPTION { get; set; }
        public int? STORESEEN { get; set; }
        public int? ID { get; set; }
        public int? STOREPRINT { get; set; }
        public int? comid { get; set; }
        public string regno { get; set; }
        public decimal? LICENSEAMOUNT { get; set; }
        public string YEARANDMONTH { get; set; }

        public void SetLicenseAmount(decimal value)
        {
            this.LICENSEAMOUNT = value;
            this.payamt = this.payamt - value;
        }
    }
}
