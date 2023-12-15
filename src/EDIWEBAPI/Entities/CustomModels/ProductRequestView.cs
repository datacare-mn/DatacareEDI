using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class ProductRequestView
    {
        public Entities.DBModel.Product.REQ_PRODUCT Product { get; set; }
        public List<ImageDto> Details { get; set; }
    }
}
