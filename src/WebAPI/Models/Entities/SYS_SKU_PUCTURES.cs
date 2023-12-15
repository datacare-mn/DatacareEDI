using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("SYS_SKU_PICTURES")]
    public class SYS_SKU_PUCTURES : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? PICTUREID { get; set; }
        public int? SKUID { get; set; }
        public string PICURL { get; set; }

        public int LETTERIMAGE { get; set; }

        [ForeignKey("SKUID")]
        public virtual SYS_SKU SYS_SKU { get; set; }
    }
}
