using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class PR_PNT1001
    {
        public string strymd { get; set; }

        public string stpymd { get; set; }

        public string ctrcd { get; set; }

        public string ctrnm { get; set; }

        public decimal? amt { get; set; }

        public decimal? frbtamt { get; set; }

        public decimal? ifrbtamt { get; set; }

        public decimal? evnamt { get; set; }

        public decimal? normalstk { get; set; }

        public decimal? payamt { get; set; }
    }
}
