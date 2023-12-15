using EDIWEBAPI.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.FilterViews
{
    public class AnnouncementFilterView : IFilterEntity
    {
        public DateTime BEGINDATE { get; set; }
        public DateTime ENDDATE { get; set; }
    }
}
