using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_STORECYCLE_CONFIG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public int DAYTYPE { get; set; }

        public string CYCLEINDEX { get; set; }

        public int STARTDAY { get; set; }

        public int DURATION { get; set; }

        public string DAYNAMES { get; set; }

        public int STOREID { get; set; }
        [ForeignKey("STOREID")]
        public virtual SYSTEM_ORGANIZATION SYSTEM_ORGANIZATION { get; set; }
    }
}
