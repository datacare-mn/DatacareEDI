using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.Entities;

namespace WebAPI.Models.CustomModels
{
    public class SkuInfoVirtual
    {
        public object Sku { set; get; }
        public List<SkuInfoImageData> pic { set; get; }
    }

    public struct SkuInfoImageData
    {
        public SkuInfoImageData(int intValue, string strValue)
        {
            LETTER = intValue;
            IMAGEURL = strValue;
        }

        public int LETTER { get; private set; }
        public string IMAGEURL { get; private set; }
    }
}
