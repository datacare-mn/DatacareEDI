using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.MasterSku
{
    public class MST_SKU_IMAGES
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public int SKUID { get; set; }

        public string IMAGEURL { get; set; }
        [Required]
        public int LETTERIMAGE { get; set; }

        public int INDEXID { get; set; }
    }
}
