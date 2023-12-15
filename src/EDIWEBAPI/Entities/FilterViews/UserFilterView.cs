using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;


namespace EDIWEBAPI.Entities.FilterViews
{
    public class UserFilterView
    {
        public string firstName { get; set; }

        public string lastName { get; set; }

        public string userMail { get; set; }

        public DateTime regStartDate { get; set; }

        public DateTime regEndDate { get; set; }

        public string phone { get; set; }

        public int isAdmin { get; set; }

        public int roleId { get; set; }

        [DefaultValue(0)]
        public int orgId { get; set; }

        [DefaultValue("id")]
        public string orderColumn { get; set; }

        [DefaultValue(0)]
        public int startRow { get; set; }

        [DefaultValue(0)]
        public int rowCount { get; set; }

    }
}
