using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    /// <summary>
    /// Системийн үнэ функцуудын бүртгэл
    /// </summary>
    public class SYSTEM_LICENSE_FUNCTION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public string CONTROLLER { get; set; }

        public string ROUTE { get; set; }

        public string DESCR { get; set; }
        [Required]
        public int PRICE { get; set; }
    }
}
