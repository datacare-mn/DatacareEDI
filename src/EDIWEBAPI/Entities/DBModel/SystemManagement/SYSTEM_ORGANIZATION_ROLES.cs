using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_ORGANIZATION_ROLES
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public string ROLENAME { get; set; }

        public int? ISSYSTEM { get; set; }
        [Required]
        public int ORGID { get; set; }
        [ForeignKey("ORGID")]

       
        public virtual SYSTEM_ORGANIZATION SYSTEM_ORGANIZATION { get; set; }
    }
}
