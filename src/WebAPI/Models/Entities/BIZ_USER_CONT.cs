using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("BIZ_USER_CONT")]
    public class BIZ_USER_CONT : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int USERCONTID { get; set; }
        [Required]
        public int USERID { get; set; }
        [Required]
        public int CONTRACTID { get; set; }
        [ForeignKey("USERID")]
        public virtual BIZ_COM_USER BIZ_COM_USER { get; set; }
        [ForeignKey("CONTRACTID")]
        public virtual BIZ_CONTRACT BIZ_CONTRACT { get; set; }
    }
}
