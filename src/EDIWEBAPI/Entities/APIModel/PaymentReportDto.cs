using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class PaymentReportDto
    {
        public int? ID { get; set; }
        public string REGNO { get; set; }
        public string COMPANYNAME { get; set; }
        public DateTime? ATTACHDATE { get; set; }
        public string ATTACHFILE { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal? AMOUNT { get; set; }
        public DateTime? APPROVEDATE { get; set; }
        public DateTime? STARTDATE { get; set; }
        public DateTime? ENDDATE { get; set; }
        public int? STOREPRINT { get; set; }
        public string attacheduser { get; set; }
        public string approveduser { get; set; }
    }
}
