
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class MST_CONTRACT_DEPARTMENT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal ID { get; set; }
        public int CONTRACTID { get; set; }
        public string DEPARTMENTCODE { get; set; }
    }
}
