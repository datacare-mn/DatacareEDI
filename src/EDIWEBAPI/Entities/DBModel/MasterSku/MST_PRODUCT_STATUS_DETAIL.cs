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
    public class MST_PRODUCT_STATUS_DETAIL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public string STATUS { get; set; }
        public string NAME { get; set; }
        public int TYPEID { get; set; }
        public int ENABLED { get; set; }
        public int VIEWORDER { get; set; }
        public int AUTO { get; set; }
        public int EDITABLE { get; set; }
        public int DURATION { get; set; }
        public int DECISION { get; set; }

    }
}
