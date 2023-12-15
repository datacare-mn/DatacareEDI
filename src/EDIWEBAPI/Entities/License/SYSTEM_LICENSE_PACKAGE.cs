using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    /// <summary>
    /// Системийн лицензийн багцын бүртгэл
    /// </summary>
    public class SYSTEM_LICENSE_PACKAGE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int PACKAGEID { get; set; }

        public string PACKAGENAME { get; set; }

        public string PACKAGECAPTION { get; set; }

        public int? USERCOUNT { get; set; }

        public int? MSGCOUT { get; set; }
    }
}
