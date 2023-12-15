using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    public class BIZ_COM_BRANCH : AuditableEntity<long>
    {
        [Key]
        [Required]
        public decimal ID { get; set; }
        [Required]
        public string BRANCHNAME { get; set; }

        public string LOCATION { get; set; }

        public string ADDRESS { get; set; }
        [Required]
        public int COMID { get; set; }

        public string BRANCHIMAGE { get; set; } 

        [ForeignKey("COMID")]
        public virtual BIZ_COMPANY BIZ_COMPANY { get; set; }
    }
}
