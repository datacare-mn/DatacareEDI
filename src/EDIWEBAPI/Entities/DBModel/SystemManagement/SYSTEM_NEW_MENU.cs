using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_NEW_MENU
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]

        public int ID { get; set; }
        public int? PARENTID { get; set; }
        public string TITLE { get; set; }
        public string ROUTE { get; set; }
        public string ICON { get; set; }
        public int TYPE { get; set; }
        public int VIEWORDER { get; set; }
        public string SORTEDORDER { get; set; }
        public int ENABLED { get; set; }
        public int SYS { get; set; }
        public int SHOP { get; set; }
        public int BUSINESS { get; set; }
        public int HASROLE { get; set; }
    }
}
