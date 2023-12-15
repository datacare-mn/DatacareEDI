using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("BIZ_STORE_BUSINESS")]
    public class BIZ_STORE_BUSINESS : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public int STOREID { get; set; }
        [Required]
        public int BUSINESSID { get; set; }

        public DateTime INSYMD { get; set; }

        public string INSEMP { get; set; }
    }
}
