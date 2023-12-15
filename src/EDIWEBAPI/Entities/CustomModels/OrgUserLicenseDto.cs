using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class OrgUserLicenseDto
    {
        public int BUSINESSID { get; set; }
        public int USERID { get; set; }
        public string USERMAIL { get; set; }
        public string USERNAME { get; set; }
        public int TYPE { get; set; }
        public int COUNT { get; set; }
        public decimal? AMOUNT { get; set; }
    }
}
