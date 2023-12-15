using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_REQUEST_ACTION_LOG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string ID { get; set; }

        public string OSNAME { get; set; }

        public string OSVERSION { get; set; }

        public string BROWSER { get; set; }

        public string BROWSERVERSION { get; set; }

        public int COMID { get; set; }

        public int USERID { get; set; }

        public DateTime? REQUESTDATE { get; set; }
        public string REQUESTYEARMONTH { get; set; }

        public decimal? REQUESTDATA { get; set; }

        public string CONTROLLER { get; set; }

        public string ROUTE { get; set; }
        public string PARAMETER { get; set; }
        public byte SUCCESS { get; set; }
        public string MESSAGE { get; set; }
    }
}
