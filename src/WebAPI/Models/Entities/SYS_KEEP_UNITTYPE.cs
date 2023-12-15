using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("SYS_KEEP_UNITTYPE")]
    public class SYS_KEEP_UNITTYPE : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int TYPEID { get; set; }
        [Required]
        public string TYPENAME { get; set; }
    }
}
