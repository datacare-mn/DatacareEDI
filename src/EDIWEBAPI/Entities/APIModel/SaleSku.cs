using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class SaleSku
    {
        public string branch { get; set; } //skucd
        public string ctrcd { get; set; }
        public string barcode { get; set; }
        public string prodname { get; set; } //skunm
        public decimal? qty_dd { get; set; } //тоо ширхэг өдрийн
        public decimal? supply_dd { get; set; } //нийлүүлэх үнэ өдрийн 
        public decimal? qty_mm { get; set; } // Тоо ширхэг өдрийн
        public decimal? supply_mm { get; set; } //нийлүүлэх үнэ сарын 
        public string subcatcd { get; set; } //Дэд категори
        public decimal? supply { get; set; }
    }
}
