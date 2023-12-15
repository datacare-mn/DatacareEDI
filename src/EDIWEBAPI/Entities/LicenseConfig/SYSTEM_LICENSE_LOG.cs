
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.LicenseConfig
{
    public class SYSTEM_LICENSE_LOG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal ID { get; set; }
        public decimal HEADERID { get; set; }
        public decimal? OLDVALUE { get; set; }
        public decimal? NEWVALUE { get; set; }
        public string NOTE { get; set; }
        public decimal? CREATEDBY { get; set; }
        public DateTime? CREATEDDATE { get; set; }
    }
}
