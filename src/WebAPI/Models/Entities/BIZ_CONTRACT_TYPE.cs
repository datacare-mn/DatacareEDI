using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("BIZ_CONTRACT_TYPE")]
    public class BIZ_CONTRACT_TYPE : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int CONTRACTTYPEID { get; set; }
        [Required]
        public string CONTRACTTYPENAME { get; set; }
        [Required]
        public int STOREID { get; set; }

        [ForeignKey("STOREID")]
        public virtual BIZ_COMPANY BIZ_COMPANY { get; set; }
    }
}
