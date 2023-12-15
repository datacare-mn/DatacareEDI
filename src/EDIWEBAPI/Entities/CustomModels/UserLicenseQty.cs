using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class UserLicenseQty
    {
        public int USERID { get; set; }
        public int LICENSEID { get; set; }
        public int COUNT { get; set; }
    }
}
