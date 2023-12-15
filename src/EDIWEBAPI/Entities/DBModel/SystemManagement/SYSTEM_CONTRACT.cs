using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_CONTRACT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]

        public int ID { get; set; }
        public int STOREID { get; set; }
        public DateTime BEGINDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public int USERQTY { get; set; }
        public int ENABLED { get; set; }
        public decimal? PRICE { get; set; }
        public string NOTE { get; set; }
    }
}
