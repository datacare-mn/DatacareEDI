
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.LicenseConfig
{
    public class SYSTEM_REPORT_LICENSE_HEADER
    {
        public string COMPANYNAME { get; set; }

        public string REGNO { get; set; }

        public int? YEAR { get; set; }

        public string MONTH { get; set; }

        public int? SCORE { get; set; }

        public int? SKUCNT { get; set; }

        public int? CONTRACTCNT { get; set; }

        public int? USERCNT { get; set; }

        public string SALEAMT { get; set; }

        public decimal? LICENSEAMOUNT { get; set; }

        public string STORENAME { get; set; }

        public int? STOREID { get; set; }

        public int? BUSINESSID { get; set; }
    }
}
