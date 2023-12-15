using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.Dashboard
{
    public class DASH_USER_LICINFO
    {
        public string COMPANYNAME { get; set; }

        public string USERMAIL { get; set; }

        public string ROLENAME { get; set; }

        public int? ISAGREEMENT { get; set; }
        
        public string OLDROLENAME { get; set; }
    }
}
