using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.MasterSku
{
    public class MST_MASTER_DIVISION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public int STOREID { get; set; }

        public string DIVCODE { get; set; }

        public string DIVNAME { get; set; }
        [ForeignKey("STOREID")]
        public virtual SYSTEM_ORGANIZATION SYSTEM_ORGANIZATION { get; set; }
    }
}
