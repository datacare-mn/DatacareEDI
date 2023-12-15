using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("SYS_BRAND")]
    public class SYS_BRAND : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int BRANDID { get; set; }

        public string BRANDIMAGE { get; set; }
        [Required]
        public string BRANDNAME { get; set; }
        [Required]
        public int COMID { get; set; }
        [ForeignKey("COMID")]
        public virtual BIZ_COMPANY BIZ_COMPANY { get; set; }
    }
}
