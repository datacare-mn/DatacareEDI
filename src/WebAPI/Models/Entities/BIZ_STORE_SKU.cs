using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("BIZ_STORE_SKU")]
    public class BIZ_STORE_SKU
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public int STOREID { get; set; }
        [Required]
        public int COMID { get; set; }
        [Required]
        public int SUPPLYPRICE { get; set; }
        [Required]
        public int BRANCHID { get; set; }
        [Required]
        public int SALEPRICE { get; set; }
        [Required]
        public int SKUID { get; set; }
    }
}
