using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.RequestModels
{
    public class ReportRequest
    {
        public string Controller { get; set; }
        public string Route { get; set; }
        public int Index { get; set; }

        public int StoreId { get; set; }
        public int CompanyId { get; set; }
        public string RegNo { get; set; }
        public string ContractNo { get; set; }
        public string BranchCode { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
