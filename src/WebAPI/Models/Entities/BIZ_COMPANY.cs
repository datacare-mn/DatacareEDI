using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("BIZ_COMPANY")]
    public class BIZ_COMPANY : AuditableEntity<long>
    {
        [Key]
        [Required]
        public int COMID { get; set; }
        [Required]
        public string NAME { get; set; }

        public string ADDRESS { get; set; }

        public string CEONAME { get; set; }

        public string PHONE { get; set; }

        public string FAX { get; set; }

        public string WEB { get; set; }
        [Required]
        public string MAIL { get; set; }

        public string LOGO { get; set; }
        [Required]
        public int COMTYPE { get; set; }

        public string LOCATION { get; set; }
        [Required]
        public string COMREG { get; set; }

        public string SLOGAN { get; set; }

        public string WSTIME { get; set; }

        public string WETIME { get; set; }

        public string TSTIME { get; set; }

        public string TETIME { get; set; }

        public int? MEEMINUTE { get; set; }

        public int? DATAPERMISSION { get; set; }
        

    }
}
