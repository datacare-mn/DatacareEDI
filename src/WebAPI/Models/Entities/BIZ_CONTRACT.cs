
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("BIZ_CONTRACT")]
    public class BIZ_CONTRACT : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int CONTRACTID { get; set; }
        [Required]
        public int STOREID { get; set; }
        [Required]
        public int COMID { get; set; }
        [Required]
        public string CONTRACTNAME { get; set; }

        public string CONTRACTDESC { get; set; }

        public DateTime CONTRACTDATE { get; set; }
        [Required]
        public int CONTRACTTYPE { get; set; }
        [ForeignKey("CONTRACTTYPE")]
        public virtual BIZ_CONTRACT_TYPE BIZ_CONTRACT_TYPE { get; set; }
}
}
