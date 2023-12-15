using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.DBModel.Product;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class ProductRequstView
    {
        private REQ_PRODUCT Product { get; set; }
        private List<ImageDto> Images { get; set; }
    }
}
