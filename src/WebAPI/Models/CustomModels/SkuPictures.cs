using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.Entities;

namespace WebAPI.Models.CustomModels
{
    public class SkuPictures
    {
       public SYS_SKU sku { get; set; }

       public List<SYS_SKU_PUCTURES> pic { get; set; }
    }
}
