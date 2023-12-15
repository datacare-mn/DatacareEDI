using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.ResultModels
{
    public class VendorList
    {
        public SYSTEM_ORGANIZATION organization { get; set; }

        public List<MST_CONTRACT> contracts { get; set; }
    }
}
