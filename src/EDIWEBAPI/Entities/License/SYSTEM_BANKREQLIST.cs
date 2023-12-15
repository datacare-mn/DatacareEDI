using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    public class SYSTEM_BANKREQLIST
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal ID { get; set; }

        public string ACCOUNTNO { get; set; }

        public string ACCOUNTNAME { get; set; }

        public DateTime? TRANSACDATE { get; set; }

        public decimal? DEBIT { get; set; }

        public decimal? CREDIT { get; set; }

        public string DESCRIPTION { get; set; }

        public string DESCVALUE { get; set; }
    }
}
