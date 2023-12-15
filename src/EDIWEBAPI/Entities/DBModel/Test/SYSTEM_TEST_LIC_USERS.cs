using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_TEST_LIC_USERS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public int? ROLEID { get; set; }
        public int ORGID { get; set; }
        public DateTime? ROLECHANGEDATE { get; set; }
        public int? ISAGREEMENT { get; set; }
        public DateTime? AGREEMENTDATE { get; set; }
        public int? OLDROLEID { get; set; }
        public int? LOGCOUNT { get; set; }
        public int? REQUESTCOUNT { get; set; }
        public int? RESTORED { get; set; }
        public int? DELETED { get; set; }
        public DateTime? DELETEDDATE { get; set; }
        public DateTime? RESTOREDDATE { get; set; }
    }
}
