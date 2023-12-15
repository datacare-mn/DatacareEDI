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
    public class MST_PRODUCT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public string BARCODE { get; set; }
        public string NAME { get; set; }
        public string STORENAME { get; set; }
        public int MEASUREID { get; set; }
        public string BRANDNAME { get; set; }
        public decimal? PRICE { get; set; }
        public int IMAGEQTY { get; set; }
        public int STOREQTY { get; set; }
        public int ORGID { get; set; }
        public int DEPARTMENTID { get; set; }
        public int ENABLED { get; set; }
        public int? CREATEDBY { get; set; }
        public DateTime? CREATEDDATE { get; set; }

    }
}
