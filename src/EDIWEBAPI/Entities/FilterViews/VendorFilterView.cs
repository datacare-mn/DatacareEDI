using EDIWEBAPI.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Entities.FilterViews
{
    public class VendorFilterView : IFilterEntity
    {

        public string COMPANYNAME { get; set; }

        public string REGNO { get; set; }

        public string CEONAME { get; set; }

        public string EMAIL { get; set; }

        public string MOBILE { get; set; }

        [DefaultValue(1)]
        public ENABLED? ENABLED { get; set; }

        public int COMID { get; set; }
    }
}
