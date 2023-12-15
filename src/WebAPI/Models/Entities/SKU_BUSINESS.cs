using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("SKU_BUSINESS")]
    public class SKU_BUSINESS  : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int BUSINESSSKUID { get; set; }
        [Required]
        public int COMID { get; set; }
        [Required]
        public int SKUID { get; set; }

        [Required]
        public int ISOWNER { get; set; }
        [Required]
        public int ISACTIVE { get; set; }

        public int? BALANCE { get; set; }
        //[ForeignKey("COMID")]
        //public virtual BIZ_COMPANY BIZ_COMPANY { get; set; }
        //[ForeignKey("SKUID")]
        //public virtual SYS_SKU SYS_SKU { get; set; }
    }
}
