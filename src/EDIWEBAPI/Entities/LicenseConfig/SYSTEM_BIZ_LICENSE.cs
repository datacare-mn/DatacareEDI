
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.LicenseConfig
{
    public class SYSTEM_BIZ_LICENSE
    {
        [Key, Column(Order = 0)]
        [Required]
        public int YEAR { get; set; }
        [Key, Column(Order = 1)]
        [Required]
        public string MONTH { get; set; }

        [Key, Column(Order = 2)]
        [Required]
        public int BUSINESSID { get; set; }

        public int SCORE { get; set; }

        public decimal? AMOUNT { get; set; }
        public decimal? ACTUALAMOUNT { get; set; }

        public DateTime CREATEDATE { get; set; }

        [Key, Column(Order = 3)]
        [Required]
        public int STOREID { get; set; }

        public int? SKUCNT { get; set; }
        public int? SKUSCORE { get; set; }
        public int? CONTRACTCNT { get; set; }
        public int? CONTRACTSCORE { get; set; }
        public int? SALESCORE { get; set; }

        public int? USERCNT { get; set; }

        public string SALEAMT { get; set; }

        public int PAYED { get; set; }

        public string PAYCTRCD { get; set; }

        public string PAYCYCLE { get; set; }

        public string PAYJUMCD { get; set; }

        [ForeignKey("BUSINESSID")]
        public virtual SYSTEM_ORGANIZATION SYSTEM_ORGANIZATION { get; set; }
    }
}
