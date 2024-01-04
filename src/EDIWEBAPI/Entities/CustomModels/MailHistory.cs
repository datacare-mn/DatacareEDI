using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class MailHistory
    {
        public DateTime sdate { get; set; }
        public DateTime edate { get; set; }
        public int issent { get; set; }
        public  string mail { get; set; }
    }
}


