using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_SHORTURL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string SHORTURL { get; set; }
        public string LONGURL { get; set; }
        public string TYPE { get; set; }
        public DateTime INSYMD { get; set; }
        public DateTime LASTREQDATE { get; set; }
        public int VISITCOUNT { get; set; }
        public decimal? RECORDID { get; set; }
        public DateTime? EXPIREDATE { get; set; }
    }
}
