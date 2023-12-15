using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    public class QPayData
    {
        public string qrcode { get; set; }

        public string qrimage { get; set; }

        public string licenseKey { get; set; }

        public decimal? price { get; set; }
    }
}
