using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class Company
    {
        public string CityPayer { get; set; }
        public string Found { get; set; }
        public string LastReceiptDate { get; set; }
        public string Name { get; set; }
        public string ReceiptFound { get; set; }
        public string VatPayer { get; set; }
        public string VatPayerRegisteredDate { get; set; }
    }
}
