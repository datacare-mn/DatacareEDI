using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{

    [Table("MEET_SCHEREQ_ATTACH")]
    public class MEET_SCHEREQ_ATTACH : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ATACHID { get; set; }
        [Required]
        public string ATTACHURL { get; set; }
        [Required]
        public int REQID { get; set; }
    }
}
