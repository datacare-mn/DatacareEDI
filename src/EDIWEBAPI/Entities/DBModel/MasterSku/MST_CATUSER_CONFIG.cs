using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.MasterSku
{
    public class MST_CATUSER_CONFIG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public int USERID { get; set; }
        [Required]
        public int CLASSID { get; set; }

        public int? INSEMP { get; set; }

        public DateTime? INSYMD { get; set; }
        [ForeignKey("CLASSID")]
        public virtual MST_MASTER_CLASS MST_MASTER_CLASS { get; set; }
        [ForeignKey("USERID")]
        public virtual SYSTEM_USERS SYSTEM_USERS { get; set; }
    }
}
