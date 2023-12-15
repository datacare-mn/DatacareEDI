using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class UserLicenseDto
    {
        public int LICENSEID { get; set; }
        public string NAME { get; set; }
        public string NOTE { get; set; }
        public string IMAGEURL { get; set; }
        public int TYPE { get; set; }
        public bool SAVED { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? ANNUALPRICE { get; set; }
        public decimal? ACTUALPRICE { get; set; }
        public int QTY { get; set; }
        public int DEFAULTQTY { get; set; }
        public decimal? AMOUNT { get; set; }
        public int ENABLED { get; set; }
        public int ENABLEDANNUAL { get; set; }
        public int VALUE { get; set; }
        public int VALUEANNUAL { get; set; }
        public int USAGEQTY { get; set; }
        public decimal? PARENTID { get; set; }
        //public DateTime? DISABLEDDATE { get; set; }
        //public DateTime? ENABLEDDATE { get; set; }
    }
}
