using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.QueryModel
{
    public class SkuListCompany
    {
        public int SKUID { get; set; }

        public string SKUCD { get; set; }

        public string SKUNAME { get; set; }

        public string MGLNAME  { get; set; }

        public string BILLNAME { get; set; }

        public string MAKEDBY { get; set; }

        public int? SIZE { get; set; }

        public int? MyProperty { get; set; }

        public int? BOXWEIGHT { get; set; }

        public int? BOXQTY { get; set; }

        public int? BOXCBM { get; set; }

        public string DESCRIPTION { get; set; }

        public string BRANDNAME { get; set; }

        public string MEASURENAME { get; set; }

        public string KEEPVALUE { get; set; }

        public string ORIGINNAME { get; set; }

        public string COLORNAME { get; set; }

        public string MODELNO { get; set; }

        public string ISACTIVE { get; set; }

        public int? ISCALVAT { get; set; }

        public string BALANCE { get; set; }


        public string INSYMD { get; set; }

        public string INSEMP { get; set; }

        public int? STORECOUNT { get; set; }

        public string IMAGEURL { get; set; }





    }
}
