using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_MENU
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]

        public int MENUID { get; set; }

        public int? PARENTID { get; set; }

        public string MENUNAME { get; set; }

        public string MENUCAPTION { get; set; }

        public string MENUURL { get; set; }

        public string MENUICON { get; set; }

        public int MODULETYPE { get; set; }

        public int? ORDER { get; set; }
        [ForeignKey("PARENTID")]
        public virtual SYSTEM_MENU SYSTEM_MENUs { get; set; }
    }
}
