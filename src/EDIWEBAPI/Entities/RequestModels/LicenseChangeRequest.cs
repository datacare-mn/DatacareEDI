using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.RequestModels
{
    public class LicenseChangeRequest
    {
        public decimal BUSINESSID { get; set; }
        public DateTime LICENSEDATE { get; set; }
        public decimal NEWVALUE { get; set; }
        public string NOTE { get; set; }
    }
}
