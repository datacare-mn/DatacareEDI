using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.MasterSku
{
    public class MST_BRAND
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public string BRANDNAME { get; set; }
        public string LOGO { get; set; }
        public int ORGID { get; set; }
        [ForeignKey("ORGID")]
        public virtual SYSTEM_ORGANIZATION SYSTEM_ORGANIZATION { get; set; }
    }
}
