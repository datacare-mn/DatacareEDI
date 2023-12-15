using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_USER_DEVICE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal ID { get; set; }
        public string USERMAIL { get; set; }
        public string IPADDRESS { get; set; }
        public string LOCALADDRESS { get; set; }
        public string TRACEIDENTIFIER { get; set; }
        public string OSNAME { get; set; }
        public string BROWSERNAME { get; set; }
        public decimal LASTREQUESTID { get; set; }
        public DateTime? LASTLOGDATE { get; set; }
        public int BLOCKED { get; set; }
        public DateTime? BLOCKEDDATE { get; set; }
        public DateTime? MAILEXPIREDATE { get; set; }
        public int WARN { get; set; }
        public DateTime? STARTWARNDATE { get; set; }
        public DateTime? STOPWARNDATE { get; set; }
    }
}
