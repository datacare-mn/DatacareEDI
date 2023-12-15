using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.LicenseConfig
{
    public class LicenseReportData
    {
        public SYSTEM_REPORT_LICENSE_HEADER Header { get; set; }
        
        public List<SYSTEM_REPORT_LICENSE_DETAIL> Detail { get; set; }
    }
}
