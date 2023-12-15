using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.Interfaces;

namespace EDIWEBAPI.Entities.FilterViews
{
    public class ProductRequestFilterView : IFilterEntity
    {
        public int ORGID { get; set; }
        public string REGNO { get; set; }
        public int STOREID { get; set; }
        public int REQUESTID { get; set; }
        public string CONTRACTNO { get; set; }
        public int DEPARTMENTID { get; set; }
        public string STATUS { get; set; }
        public int USERID { get; set; }
        public DateTime BEGINDATE { get; set; }
        public DateTime ENDDATE { get; set; }
    }
}
