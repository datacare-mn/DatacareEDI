using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("BIZ_STORE_COMPANY")]
    public class BIZ_STORE_COMPANY : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int STORECOMID { get; set; }
        [Required]
        public int STOREID { get; set; }
        [Required]
        public int COMID { get; set; }
        [ForeignKey("STOREID")]
        public virtual BIZ_COMPANY BIZ_STORE { get; set; }
        [ForeignKey("COMID")]
        public virtual BIZ_COMPANY BIZ_COMPANY { get; set; }
    }
}
