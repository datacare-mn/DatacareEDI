using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    public class MEET_ROOM : AuditableEntity<long>
    {
        [Key]
        [Required]
        public decimal ROOMID { get; set; }
        [Required]
        public string ROOMNUMBER { get; set; }

        public string ROOMDESC { get; set; }
        [Required]
        public int STOREID { get; set; }

        public decimal ROOMCAPACITY { get; set; }

        [ForeignKey("STOREID")]
        public virtual BIZ_COMPANY BIZ_COMPANY { get; set; }
    }
}
