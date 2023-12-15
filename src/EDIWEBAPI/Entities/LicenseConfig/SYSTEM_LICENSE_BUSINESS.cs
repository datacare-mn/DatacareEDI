
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.LicenseConfig
{
    public class SYSTEM_LICENSE_BUSINESS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal ID { get; set; }
        public int BUSINESSID { get; set; }
        public int YEAR { get; set; }
        public string MONTH { get; set; }
        public decimal YEARANDMONTH { get; set; }
        public int USERQTY { get; set; }
        public int REPORTQTY { get; set; }
        public decimal? USERFEE { get; set; }
        public decimal? REPORTFEE { get; set; }
        public decimal? TOTAL { get; set; }
        public decimal? AMOUNT { get; set; }
    }
}
