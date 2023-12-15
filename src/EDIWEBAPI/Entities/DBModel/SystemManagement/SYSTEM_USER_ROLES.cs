using EDIWEBAPI.Entities.DBModel.MasterData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_USER_ROLES
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public int USERID { get; set; }
        [Required]
        public int ROLEID { get; set; }
    }
}
