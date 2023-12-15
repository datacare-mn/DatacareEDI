using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("SYS_ORIGIN")]
    public class SYS_ORIGIN : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ORIGINID { get; set; }
        [Required]
        public string SHORTNAME { get; set; }
        [Required]
        public string ORIGINNAME { get; set; }
    }
}
