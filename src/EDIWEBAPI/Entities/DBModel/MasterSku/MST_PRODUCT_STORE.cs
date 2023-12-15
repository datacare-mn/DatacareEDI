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
    public class MST_PRODUCT_STORE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public int ORGID { get; set; }
        public int PRODUCTID { get; set; }
        public int STOREID { get; set; }
        public string CONTRACTNO { get; set; }
        public string STORECODE { get; set; }
        public decimal? PRICE { get; set; }
        public int ENABLED { get; set; }
        public int STOCK { get; set; }

    }
}
