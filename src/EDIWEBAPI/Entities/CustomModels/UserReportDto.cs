using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class UserReportDto
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public string LICENSENAME { get; set; }
        public int REPORTQTY { get; set; }
        public decimal? PRICE { get; set; }
    }
}
