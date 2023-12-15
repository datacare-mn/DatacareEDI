using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class MultiMailRequest
    {
        public string Note { get; set; }
        public List<OrganizationPayment> Organizations { get; set; }
    }
}
