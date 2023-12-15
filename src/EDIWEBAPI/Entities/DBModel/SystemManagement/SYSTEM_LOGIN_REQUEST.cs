using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_LOGIN_REQUEST
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal ID { get; set; }
        public string OSNAME { get; set; }
        public string OSVERSION { get; set; }
        public string BROWSERNAME { get; set; }
        public string BROWSERVERSION { get; set; }
        public string COUNTRY { get; set; }
        public string CITY { get; set; }
        public string COUNTRYCODE { get; set; }
        public string ISP { get; set; }
        public string ISPCOMPANYDET { get; set; }
        public string LAT { get; set; }
        public string LON { get; set; }
        public string IPADDRESS { get; set; }
        public string LOCALADDRESS { get; set; }
        public string REGIONNAME { get; set; }
        public string REQTIMEZONE { get; set; }
        public string USERNAME { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public string FAILDETAIL { get; set; }
    }
}
