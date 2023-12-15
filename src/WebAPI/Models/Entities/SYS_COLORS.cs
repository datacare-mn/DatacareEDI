using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Interfaces;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("SYS_COLORS")]
    public class SYS_COLORS : AuditableEntity<long>, IDescribableEntity 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int COLORID { get; set; }
        [Required]
        public string COLORNAME { get; set; }

        public string Describe()
        {
            return "{  COLORID : \"" + COLORID + "\", COLORNAME : \"" + COLORNAME + "\"}";
        }
    }
}
