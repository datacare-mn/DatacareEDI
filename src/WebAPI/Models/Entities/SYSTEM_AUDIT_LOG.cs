using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Entities
{
    [Table("SYSTEM_AUDIT_LOG")]
    public class SYSTEM_AUDIT_LOG 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string LOGID { get; set; }

        public string USERNAME { get; set; }

        public int COMID { get; set; }

        public DateTime EVENTDATE { get; set; }

        public string EVENTTYPE { get; set; }

        public string TABLENAME { get; set; }

        public string RECORDID { get; set; }

        public string COLUMNNAME { get; set; }

        public string ORIGINALVALUE { get; set; }

        public string NEWVALUE { get; set; }
    }
}
