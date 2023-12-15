using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class RequestProductDto
    {
        public int ID { get; set; }
        public int STOREID { get; set; }
        public int? CONTRACTID { get; set; }
        public string CONTRACTNO { get; set; }
        public string CONTRACTDESC { get; set; }
        public int DEPARTMENTID { get; set; }
        public int REQUESTID { get; set; }
        public string NOTE { get; set; }
    }
}
