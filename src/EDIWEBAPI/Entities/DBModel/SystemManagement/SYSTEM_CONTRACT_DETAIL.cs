using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_CONTRACT_DETAIL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]

        public int ID { get; set; }
        public int LICENSEID { get; set; }
        public int YEAR { get; set; }
        public string MONTH { get; set; }
        public decimal? PRICE { get; set; }
        public int ACTUALQTY { get; set; }
        public int CHARGEQTY { get; set; }
    }
}
