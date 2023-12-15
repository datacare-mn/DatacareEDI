using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.LicenseConfig
{
    public class SYSTEM_REPORT_LICENSE_DETAIL
    {
        public int? YEAR { get; set; }

        public string MONTH { get; set; }

        public string FULLNAME { get; set; }

        public int? LOGCOUNT { get; set; }

        public decimal LICPRICE { get; set; }

        public int? SCORE { get; set; }

        public int? STOREID { get; set; }

        public int? BIZID { get; set; }

        public int? USERID { get; set; }

        public int? DAYCOUNT { get; set; }

        public string ROLENAME { get; set; }

    }
}
