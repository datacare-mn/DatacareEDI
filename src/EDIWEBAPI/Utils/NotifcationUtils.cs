using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Utils
{
    public class NotifcationUtils
    {
        public string NotifcationID { get; set; }

        public string StoreName { get; set; }

        public DateTime? CreationDate { get; set; }

        public string LocalData { get; set; }
    }

    public class SYSTEM_NOTIFCATION_DATA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string ID { get; set; }

        public int? COMID { get; set; }

        public DateTime? CREATEDDATE { get; set; }

        public int? NOTIFMODULETYPE { get; set; }

        public decimal? RECORDID { get; set; }

        public int? STOREID { get; set; }

        public int? ISSEEN { get; set; }
    }
}
