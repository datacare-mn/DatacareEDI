using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.LicenseConfig
{
    public class SYSTEM_BIZ_LIC_DETAIL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string ID { get; set; }
        public int? YEAR { get; set; }
        public string MONTH { get; set; }
        public int? BIZID { get; set; }
        public int? USERID { get; set; }
        public int? LOGCOUNT { get; set; }
        public int? REQUESTCOUNT { get; set; }
        public int? STOREID { get; set; }
        public decimal? LICPRICE { get; set; }
        public int? SCORE { get; set; }
        public int? DAYCOUNT { get; set; }
        public int? ROLEID { get; set; }
        public decimal? ACTUALPRICE { get; set; }
        public DateTime? STARTDATE { get; set; }
        public DateTime? ENDDATE { get; set; }
        public int? STATUSCOUNT { get; set; }
    }
}
