using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("BIZ_DEPART")]
    public class BIZ_DEPART : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int DEPID { get; set; }
        [Required]
        public string DEPNAME { get; set; }
        [Required]
        public int COMID { get; set; }
        [ForeignKey("COMID")]
        public virtual BIZ_COMPANY BIZ_COMPANY { get; set; }
    }
}
