using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.Dashboard
{
    public class DASH_BIZ_LAST_REQUEST
    {
        public string country { get; set; }

        public string city { get; set; }

        public string isp { get; set; }

        public string ipaddress { get; set; }

        public string lat { get; set; }

        public string lon { get; set; }

        public string username { get; set; }

        public DateTime? requestdate { get; set; }
    }
}
