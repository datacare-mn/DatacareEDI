using EDIWEBAPI.Entities.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.ResultModels
{
    public class ReturnVendorList
    {
        public List<VendorCompany> RetVendorList { get; set; }

        public int TotalCount { get; set; }
    }
}
