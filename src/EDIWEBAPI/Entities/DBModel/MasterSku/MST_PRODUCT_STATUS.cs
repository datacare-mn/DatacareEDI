using EDIWEBAPI.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Entities.DBModel.MasterSku
{
    public class MST_PRODUCT_STATUS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string ID { get; set; }
        public string NAME { get; set; }
        public string COLOR { get; set; }
        public string STORENAME { get; set; }
        public string ACTIONNAME { get; set; }
        public int GROUPID { get; set; }
        public int DECISION { get; set; }
        public int CHOOSABLE { get; set; }
        public int VIEWORDER { get; set; }
    }
}
