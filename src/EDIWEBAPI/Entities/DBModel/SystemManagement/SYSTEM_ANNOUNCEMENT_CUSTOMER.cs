using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_ANNOUNCEMENT_CUSTOMER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal ID { get; set; }
        public decimal HEADERID { get; set; }
        public decimal ORGID { get; set; }
    }
}
