using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class MST_SKU_SELECT
    {
        public int? SKUID { get; set; }

        public string SKUCD { get; set; }
        public string SKUNAME { get; set; }

        public string MGLNAME { get; set; }

        public string BILLNAME { get; set; }

        public string MODELNO { get; set; }

        public string COLOR { get; set; }

        public string MAKEDBY { get; set; }

        public int? WEIGHT { get; set; }

        public string SKUSIZE { get; set; }

        public string BOXCODE { get; set; }

        public int? BOXWEIGHT { get; set; }

        public int? BOXQTY { get; set; }

        public int? BOXCBM { get; set; }

        public string DESCRIPTION { get; set; }

        public DateTime? INSYMD { get; set; }

        public string INSEMP { get; set; }

        public DateTime? UPDYMD { get; set; } 

        public string UPDEMP { get; set; }

        public int? ISCALVAT { get; set; }

        public string BRANDNAME { get; set; }

        public string UOMVALUE { get; set; }

        public int? ORGID { get; set; }

        public string IMAGEURL { get; set; }


        public string ORIGINNAME { get; set; }

        public string STORECOUNT { get; set; }

        public int? LETTERIMAGECOUNT { get; set; }

        public string MEASURE { get; set; }

        public string KEEPUNIT { get; set; }
        public int? ISACTIVE { get; set; }

        public int? BALANCE { get; set; }

    }
}
