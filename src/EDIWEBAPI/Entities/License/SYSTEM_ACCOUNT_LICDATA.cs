using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    public class SYSTEM_ACCOUNT_LICDATA
    {
        public string REGNO { get; set; }

        public string  COMPANYNAME { get; set; }

        public string  LICENSEDATE { get; set; }

        public decimal? AMOUNT { get; set; }

        public string PAYCYCLE { get; set; }

        public string PAYCTRCD { get; set; }

        public string PAYSTORECODE { get; set; }
    }
}
