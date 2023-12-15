using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("BIZ_USER_DEPART")]
    public class BIZ_USER_DEPART : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int USERDEPID { get; set; }
        [Required]
        public int USERID { get; set; }
        [Required]
        public int DEPARTID { get; set; }
        [ForeignKey("USERID")]
        public virtual BIZ_COM_USER BIZ_COM_USER { get; set; }
        [ForeignKey("DEPARTID")]
        public virtual BIZ_DEPART BIZ_DEPART { get; set; }
    }
}
