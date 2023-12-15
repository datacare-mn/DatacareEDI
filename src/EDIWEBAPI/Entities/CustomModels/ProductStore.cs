using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class ProductStore
    {
        public int STOREID { get; set; }
        public string STORENAME { get; set; }
        public string CONTRACTNO { get; set; }
        public decimal? PRICE { get; set; }
        public int ENABLED { get; set; }
        public int STOCK { get; set; }
    }
}
