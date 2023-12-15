using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.Order
{
    public class REQ_ORDER : Interfaces.ISeen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal? ORDERID { get; set; }

        public string ORDERDATE { get; set; }

        public string ORDERNO { get; set; }

        public string STORENO { get; set; }

        public string FILEURL { get; set; }

        public string CONTRACTNO { get; set; }

        public DateTime? CREATEDDATE { get; set; }

        public int? ISSEEN { get; set; }

        public int? SEENUSER { get; set; }

        public DateTime? SEENDATE { get; set; }

        public int? APPROVEDUSER { get; set; }

        public DateTime? APPROVEDDATE { get; set; }
    }
}
