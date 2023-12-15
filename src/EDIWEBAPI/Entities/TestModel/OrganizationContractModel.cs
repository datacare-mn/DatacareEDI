using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.TestModel
{
    public class OrganizationContractModel
    {
        public int ID { get; set; }
        public int ORGANIZATIONID { get; set; }
        public string REGNO { get; set; }
        public int CONTRACTID { get; set; }
        public string CONTRACTNO { get; set; }
        public string DESCRIPTION { get; set; }
    }
}
