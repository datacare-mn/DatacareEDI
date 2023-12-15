using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.LicenseConfig
{
    public class SYSTEM_LIC_SALE_CONFIG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public decimal? MINVALUE { get; set; }

        public  decimal? MAXVALUE { get; set; }

        public int? SCORE { get; set; }

        public string RANGENAME { get; set; }
    }
}
