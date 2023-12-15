using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class SaleTotal
    {
        //[JsonProperty(PropertyName = "storecd")]
        public string branch { get; set; } //jumcd
        public string ctrcd { get; set; }
        public string barcode { get; set; } //Баркод
        public string prodname { get; set; } //Барааны нэр
        public decimal? supply { get; set; } //system date iin supply price VATtai 
        public decimal? qty_dd { get; set; } //тоо ширхэг өдрийн
        public decimal? supply_dd { get; set; } //нийлүүлэх үнэ өдрийн 
        public decimal? qty_mm { get; set; } // Тоо ширхэг өдрийн
        public decimal? supply_mm { get; set; } //нийлүүлэх үнэ сарын 
        public string subcatcd { get; set; } //Дэд категори
        public string saledate { get; set; }
    }
}
