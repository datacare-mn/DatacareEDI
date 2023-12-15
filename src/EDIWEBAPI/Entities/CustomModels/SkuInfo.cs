using EDIWEBAPI.Entities.DBModel.MasterSku;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class SkuInfo
    {
        public int? SKUID { get; set; }

        public string SKUCD { get; set; }
        public string SKUNAME { get; set; }

        public string MGLNAME { get; set; }

        public string BILLNAME { get; set; }
        public int? BRANDID { get; set; }
        public int? ORIGINID { get; set; }

        public string MODELNO { get; set; }

        public string COLOR { get; set; }

        public string MAKEDBY { get; set; }
        public int? UOMID { get; set; }

        public int? WEIGHT { get; set; }

        public int? MEASURE { get; set; }

        public int? KEEPUNIT { get; set; }

        public int? KEEPUNITVALUE { get; set; }

        public string SKUSIZE { get; set; }

        public string BOXCODE { get; set; }

        public int? BOXWEIGHT { get; set; }

        public int? BOXQTY { get; set; }

        public int? BOXCBM { get; set; }

        public string DESCRIPTION { get; set; }

        public DateTime? INSYMD { get; set; }

        public int? INSEMP { get; set; }

        public DateTime? UPDYMD { get; set; }

        public int? UPDEMP { get; set; }

        public int? ISCALVAT { get; set; }

        public int? UOMVALUE { get; set; }

        public int? MEASUREVALUE { get; set; }
        public int? ORGID { get; set; }

        public int? ISACTIVE { get; set; }

        public int? BALANCE { get; set; }

       public List<MST_SKU_IMAGES> IMAGES { get; set; }


    }
}
