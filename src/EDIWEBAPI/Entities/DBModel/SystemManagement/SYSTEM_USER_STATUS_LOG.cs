using EDIWEBAPI.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_USER_STATUS_LOG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public int USERID { get; set; }
        public int ORGID { get; set; }
        public int ENABLED { get; set; }
        public int LOGYEAR { get; set; }
        public string LOGMONTH { get; set; }
        public int LOGBY { get; set; }
        public DateTime LOGDATE { get; set; }

    }
}
