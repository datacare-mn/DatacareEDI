using EDIWEBAPI.Entities.DBModel.SystemManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    public class SYSTEM_LICENSE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public string LICENSEKEY { get; set; }

        public int? PRICE { get; set; }

        public int? USERCOUNT { get; set; }

        public int? MSGCOUNT { get; set; }
        public DateTime? CREATEDDATE { get; set; }

        public DateTime? STARTDATE { get; set; }

        public DateTime? ENDDATE { get; set; }

        public string ENABLED { get; set; }

        public int? CREATEUSER { get; set; }

        public int? COMID { get; set; }

        public int? UPDATEUSER { get; set; }

        public DateTime? UPDATEDATE { get; set; }

        public string DESCR { get; set; }
        [JsonIgnore]
        [ForeignKey("COMID")]
        public virtual SYSTEM_ORGANIZATION SYSTEM_ORGANIZATION { get; set; }

        [JsonIgnore]
        [ForeignKey("CREATEUSER")]
        public virtual SYSTEM_USERS SYSTEM_USERS { get; set; }
        [JsonIgnore]
        [ForeignKey("UPDATEUSER")]
        public virtual SYSTEM_USERS UPDATED_SYSTEM_USERS { get; set; }
    }
}
