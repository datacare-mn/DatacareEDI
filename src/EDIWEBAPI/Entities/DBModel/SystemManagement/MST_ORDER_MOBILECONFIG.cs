using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class MST_ORDER_MOBILECONFIG
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public string CONTRACTNO { get; set; }
        [Key]
        [Column(Order = 2)]
        [Required]
        public int BUSINESSID { get; set; }

        public string MOBILE { get; set; }

        public DateTime? INSYMD { get; set; }

        public int? INSEMP { get; set; }

        public DateTime? UPDYMD { get; set; }

        public int? UPDEMP { get; set; }

        public DateTime? EXPORTEDDATE { get; set; }
    }
}
