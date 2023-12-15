using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_USER_ACTION_LOG 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string ID { get; set; }

        public int COMID { get; set; }

        public int USERID { get; set; }

        public DateTime LOGDATE { get; set; }

        public string CONTROLLER { get; set; }

        public string ROUTE { get; set; }

        public string ARGUMENT { get; set; }

        public string IPADDRESS { get; set; }
    }
}
