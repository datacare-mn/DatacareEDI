
using EDIWEBAPI.Entities.DBModel.MasterData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_MENU_ROLE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public int MENUID { get; set; }

        public int ROLEID { get; set; }
        [ForeignKey("MENUID")]
        public virtual SYSTEM_MENU SYSTEM_MENU { get; set; }
        [ForeignKey("ROLEID")]
        public virtual SYSTEM_ROLES SYSTEM_ROLES { get; set; }


    }
}
