using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class ProductView
    {
        public DBModel.MasterSku.MST_PRODUCT Product { get; set; }
        public List<ProductStore> Details { get; set; }
    }
}
