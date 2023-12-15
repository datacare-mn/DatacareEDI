using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    public class SYS_MENU  : AuditableEntity<long>
    {
        [Key]
        public decimal MENUID { get; set; }
        public string MENUNAME { get; set; }
        public decimal MODULEID { get; set; }
        public string MENUURL { get; set; }
        public string MENUCAPTION { get; set; }

        public int PARENTMENUID { get; set; }

        public int ORDERINDEX { get; set; }

        [ForeignKey("MODULEID")]
        public virtual SYS_MODULE SYS_MODULE { get; set; }

    }
}
