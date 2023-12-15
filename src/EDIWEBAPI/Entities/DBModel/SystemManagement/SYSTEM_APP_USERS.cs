using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_APP_USERS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public string APIUSER { get; set; }

        public string APPNAME { get; set; }

        public DateTime? CREATEDDATE { get; set; }

        public string APIADDRESS { get; set; }

        public string APITOKEN { get; set; }

        public DateTime? APIEXPIRETIME { get; set; }

        public int? APIEXPRIREIN { get; set; }

        public string APIDESC { get; set; }

        public string APIPASS { get; set; }

        public int? STOREID { get; set; }
    }
}
