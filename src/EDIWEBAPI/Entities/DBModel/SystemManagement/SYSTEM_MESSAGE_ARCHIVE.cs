
using System;
using System.ComponentModel.DataAnnotations;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_MESSAGE_ARCHIVE
    {
        [Key]
        [Required]
        public string ID { get; set; }
        public EDIWEBAPI.Enums.SystemEnums.MessageType TYPE { get; set; }
        public string PHONENUMBER { get; set; }
        public string SMS { get; set; }
        public string SMSFROM { get; set; }
        public int? STOREID { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public DateTime? SENDDATE { get; set; }
        public int? PRIORITY { get; set; }
        public int? TRY { get; set; }
        public string ERROR { get; set; }
        public int? ISSENT { get; set; }
    }
}
