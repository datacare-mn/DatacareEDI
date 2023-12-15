using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class ProductDto
    {
        public int ID { get; set; }
        public string BARCODE { get; set; }
        public string NAME { get; set; }
        public string STORENAME { get; set; }
        public int MEASUREID { get; set; }
        public string MEASURENAME { get; set; }
        public string BRANDNAME { get; set; }
        public decimal? PRICE { get; set; }
        public int IMAGEQTY { get; set; }
        public int STOREQTY { get; set; }
        public int ORGID { get; set; }
        public int DEPARTMENTID { get; set; }
        public string DEPARTMENTNAME { get; set; }
        public int ENABLED { get; set; }
        public int? CREATEDBY { get; set; }
        public string CREATEDNAME { get; set; }
        public DateTime? CREATEDDATE { get; set; }
    }
}
