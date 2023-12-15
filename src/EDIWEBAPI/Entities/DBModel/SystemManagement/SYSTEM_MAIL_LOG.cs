using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_MAIL_LOG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string ID { get; set; }
        public EDIWEBAPI.Enums.SystemEnums.MessageType TYPE { get; set; }
        public string MAIL { get; set; }
        public string MAILFROM { get; set; }
        public string MAILSUBJECT { get; set; }
        public string MAILBODY { get; set; }
        public int? STOREID { get; set; }
        public DateTime? SENDDATE { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public string REQUESTBY { get; set; }
        public int? ISSEND { get; set; }
        public int? PRIORITY { get; set; }
        public int? TRY { get; set; }
        public string ERROR { get; set; }
        public string CC { get; set; }
        public string ATTACHMENT { get; set; }
    }
}
