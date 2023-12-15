using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.TestModel
{
    public class OrganizationDetailModel
    {
        public int ID { get; set; }
        public int TESTID { get; set; }
        public int DETAILID { get; set; }
        public string CONTROLLER { get; set; }
        public string ROUTE { get; set; }
        public string DESCRIPTION { get; set; }
        public int TYPE { get; set; }
        public int SUCCESS { get; set; }
        public int RESPONSE { get; set; }
    }
}
