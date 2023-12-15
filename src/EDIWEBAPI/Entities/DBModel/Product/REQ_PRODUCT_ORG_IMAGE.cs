using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EDIWEBAPI.Enums.SystemEnums;
using EDIWEBAPI.Entities.Interfaces;

namespace EDIWEBAPI.Entities.DBModel.Product
{
    public class REQ_PRODUCT_ORG_IMAGE : IProductImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public int HEADERID { get; set; }
        public int? LOGID { get; set; }
        [Required]
        public int IMAGETYPE { get; set; }
        public int VIEWORDER { get; set; }
        public string URL { get; set; }
        [Required]
        public int ENABLED { get; set; }
        public int SEEN { get; set; }
        public int? SEENBY { get; set; }
        public DateTime? SEENDATE { get; set; }
        public DateTime? EXPIREDATE { get; set; }
        public DateTime? CREATEDDATE { get; set; }

    }
}
