using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    public class SYSTEM_LICENSE_DETAIL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal? ID { get; set; }

        public int FUNCTIONID { get; set; }

        public int LICENSEID { get; set; }
        [ForeignKey("FUNCTIONID")]
        public virtual SYSTEM_LICENSE_FUNCTION SYSTEM_LICENSE_FUNCTION { get; set; }
        [ForeignKey("LICENSEID")]
        public virtual SYSTEM_LICENSE SYSTEM_LICENSE { get; set; }
    }
}
