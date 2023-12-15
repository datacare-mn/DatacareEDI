using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    public class AAA_TEST : AuditableEntity<long>
    {
        [Key]
        public string TEST { get; set; }
    }
}
