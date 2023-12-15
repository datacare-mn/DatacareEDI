using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.MasterSku
{
    public class MST_MASTER_CLASS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public string CLASSNAME { get; set; }

        public int DEPID { get; set; }

        public string CLASSCODE { get; set; }
        [ForeignKey("DEPID")]
        public virtual MST_MASTER_DEPART MST_MASTER_DEPART { get; set; }
    }
}
