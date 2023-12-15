using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_USERS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string USERMAIL { get; set; }
        public string USERPASSWORD { get; set; }
        public string USERPIC { get; set; }
        public string LASTNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public DateTime REGDATE { get; set; }
        public string PHONE { get; set; }
        public ENABLED? ENABLED { get; set; }
        public int? ROLEID { get; set; }
        public int? COOPERATION { get; set; }
        [Required]
        public int ORGID { get; set; }
        [Required]
        public int ISADMIN { get; set; }
        public DateTime? ROLECHANGEDATE { get; set; }
        public int? ISAGREEMENT { get; set; }
        public DateTime? AGREEMENTDATE { get; set; }
        public int? OLDROLEID { get; set; }
        [ForeignKey("ORGID")]
        public virtual SYSTEM_ORGANIZATION SYSTEM_ORGANIZATION { get; set; }
    }
}
