using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.Payment
{
    public class REQ_PAYMENT_REPORT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int? ID { get; set; }

        public string REGNO { get; set; }

        public int? COMID { get; set; }

        public decimal? AMOUNT { get; set; }

        public DateTime? APPROVEDATE { get; set; }

        public int? APPROVEDUSER { get; set; }

        public DateTime? ATTACHDATE { get; set; }

        public int? ATTACHUSER { get; set; }
        public string ATTACHFILE { get; set; }

        public string DESCRIPTION { get; set; }

        public DateTime? STARTDATE { get; set; }

        public DateTime? ENDDATE { get; set; }

        public int? STOREPRINT { get; set; }
    }
}
