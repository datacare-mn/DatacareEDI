using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.RequestModels
{
    public class OrganizationUserRequest
    {
        public int ORGID { get; set; }
        public string REGNO { get; set; }
        public string COMPANYNAME { get; set; }
        public string ADDRESS { get; set; }
        public string CEONAME { get; set; }
        public string WEBSITE { get; set; }

        public string USEREMAIL { get; set; }
        public string USERNAME { get; set; }
        public string USERMOBILE { get; set; }
    }
}
