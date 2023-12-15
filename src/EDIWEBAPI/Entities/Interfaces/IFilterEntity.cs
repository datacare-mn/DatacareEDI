using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.Interfaces
{
    public class IFilterEntity
    {
        [DefaultValue("id")]
        public string orderColumn { get; set; }

        [DefaultValue(0)]
        public int startRow { get; set; }

        [DefaultValue(10)]
        public int rowCount { get; set; }
    }
}
