using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.MasterSku
{
    public class MST_MASTER_DEPART
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public string DEPCODE { get; set; }

        public string DEPNAME { get; set; }

        public int DIVID { get; set; }
        [ForeignKey("DIVID")]
        public virtual MST_MASTER_DIVISION MST_MASTER_DIVISION { get; set; }
    }
}
