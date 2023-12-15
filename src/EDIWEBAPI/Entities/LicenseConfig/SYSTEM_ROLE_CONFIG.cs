using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.LicenseConfig
{
    public class SYSTEM_ROLE_CONFIG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public int ROLEID { get; set; }

        public int MINSCORE { get; set; }

        public int MAXSCORE { get; set; }

        public string RANGENAME { get; set; }

        public decimal? PRICE { get; set; }
        [ForeignKey("ROLEID")]
        public virtual SYSTEM_ROLES SYSTEM_ROLES { get; set; }
    }
}
