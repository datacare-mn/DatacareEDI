using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace EDIWEBAPI.Entities.FilterViews
{
    public class FeedbackFilterView
    {
        public string feedbackName { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        [DefaultValue("id")]
        public string orderColumn { get; set; }

        [DefaultValue(0)]
        public int startRow { get; set; }

        [DefaultValue(0)]
        public int rowCount { get; set; }

    }
}
