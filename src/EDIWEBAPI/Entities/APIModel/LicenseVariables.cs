
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class LicenseVariables
    {
        public string REGNO { get; set; }

        public string yyyymm { get; set; }

        public decimal ctrcnt { get; set; }

        public decimal? invamt { get; set; }

        public int? skucnt { get; set; }
        public string payctrcd { get; set; }
        public string paycycle { get; set; }
        public string payjumcd { get; set; }
    }
}
