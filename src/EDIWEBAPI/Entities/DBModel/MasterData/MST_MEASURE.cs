
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.MasterData
{
    public class MST_MEASURE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public int STATUS { get; set; }
        public int VIEWORDER { get; set; }
        public int BARCODE { get; set; }
    }
}
