using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Entities.FilterViews
{
    public class OrganizationFilterView
    {
        public string organizationName { get; set; }
        public string organizationRegisterNumber { get; set; }
        public string organizationPhone { get; set; }
        public int orgnizationType { get; set; }
        public string director { get; set; }
        public string webSite { get; set; }

        [DefaultValue(2)]
        public int enabled { get; set; }

        [DefaultValue("id")]
        public string orderColumn { get; set; }

        [DefaultValue(0)]
        public int startRow { get; set; }

        [DefaultValue(0)]
        public int rowCount { get; set; }

    }
}
