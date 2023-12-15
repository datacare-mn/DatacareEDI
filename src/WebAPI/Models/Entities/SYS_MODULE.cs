using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    public class SYS_MODULE : AuditableEntity<long>
    {
        [Key]
        [Required]
        public decimal MODULEID { get; set; }
        [Required]
        public string MODULENAME { get; set; }

    }
}
