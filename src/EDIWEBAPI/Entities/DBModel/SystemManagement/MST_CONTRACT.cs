
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class MST_CONTRACT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int CONTRACTID { get; set; }

        public string CONTRACTNO { get; set; }

        public int STOREID { get; set; }

        public int? BUSINESSID { get; set; }

        public int? CONTRACTTYPE { get; set; }

        public string CONTRACDESC { get; set; }

        public string DEPARTMENTCODE { get; set; }

        public int ISACTIVE { get; set; }
    }
}
