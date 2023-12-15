using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.RequestModels
{
    public class OrderDetailRequest
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string OrderDate { get; set; }
        public string OrderNo { get; set; }
    }
}
