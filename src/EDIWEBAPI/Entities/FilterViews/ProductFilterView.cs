using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.Interfaces;

namespace EDIWEBAPI.Entities.FilterViews
{
    public class ProductFilterView : IFilterEntity
    {
        public int ORGID { get; set; }
        public string BARCODE { get; set; }
        public string NAME { get; set; }
        public string STORENAME { get; set; }
        public string BRANDNAME { get; set; }
        public int MEASUREID { get; set; }
        public int DEPARTMENTID { get; set; }
    }
}
