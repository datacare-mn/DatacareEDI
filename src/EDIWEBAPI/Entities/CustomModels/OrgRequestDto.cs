using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class OrgRequestDto
    {
        public int ID { get; set; }
        public string REGNO { get; set; }
        public string ORGNAME { get; set; }
        public string CEONAME { get; set; }
        public string EMAIL { get; set; }
        public string MOBILE { get; set; }
        public int DEPARTMENTID { get; set; }
        public string ADDRESS { get; set; }
        public string NOTE { get; set; }
    }
}
