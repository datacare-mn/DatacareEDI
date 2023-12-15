using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class RequestMultiNote
    {
        public string Status { get; set; }
        public string Note { get; set; }
        public List<RequestModel> Requests { get; set; }
    }
}
