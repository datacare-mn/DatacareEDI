using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class OrganizationLicenseDto
    {
        public int ID { get; set; }
        public string REGNO { get; set; }
        public string NAME { get; set; }
        public int USERQTY { get; set; }
        public decimal? BASEFEE { get; set; }
        public int REPORTQTY { get; set; }
        public decimal? REPORTFEE { get; set; }
        public decimal? TOTALFEE { get; set; }
        public decimal? TOTALAMOUNT { get; set; }
        public bool CALCULATED { get; set; }
        public string UPDATEDBY { get; set; }
        public DateTime? UPDATEDDATE { get; set; }
        public List<UserReportDto> DETAILS { get; set; }
    }
}
