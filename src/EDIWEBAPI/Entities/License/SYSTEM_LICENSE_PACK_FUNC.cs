using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    public class SYSTEM_LICENSE_PACK_FUNC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public int FUNCTIONID { get; set; }
        [Required]
        public int PACKAGEID { get; set; }

        public int PRICE { get; set; }
        [ForeignKey("FUNCTIONID")]
        public virtual SYSTEM_LICENSE_FUNCTION SYSTEM_LICENSE_FUNCTION { get; set; }
        [ForeignKey("PACKAGEID")]
        public virtual SYSTEM_LICENSE_PACKAGE SYSTEM_LICENSE_PACKAGE { get; set; }
    }
}
