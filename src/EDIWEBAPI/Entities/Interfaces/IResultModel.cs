using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.Interfaces
{
    public class IResultModel
    {
        [DefaultValue(0)]
        public int totalCount
        {
            get; set;
        }




    }
}
