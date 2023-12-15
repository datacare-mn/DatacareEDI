using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class CompanyLicenseDto
    {
        public int ID { get; set; }
        public string REGNO { get; set; }
        public string NAME { get; set; }
        public DateTime? REGISTRYDATE { get; set; }
        public int USERQTY { get; set; }
        public decimal AMOUNT { get; set; }
        public List<UserLicenseDto> DETAILS { get; set; }
    }
}
