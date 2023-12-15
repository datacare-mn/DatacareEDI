using EDIWEBAPI.Entities.Interfaces;
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
    public class MST_PRODUCT_REQUEST : IBasicEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public int GROUPID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string REQUESTCLASS { get; set; }
        public string IMAGECLASS { get; set; }
        public string LOGCLASS { get; set; }
        public int ENABLED { get; set; }
        public int VIEWORDER { get; set; }
        public int IMAGE1 { get; set; }
        public int IMAGE2 { get; set; }
        public int IMAGE3 { get; set; }
        public int IMAGE4 { get; set; }
        public int IMAGE5 { get; set; }
        public int IMAGE6 { get; set; }
        public int IMAGE7 { get; set; }
        public string NOTE { get; set; }

    }
}
