using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class MessageRequest
    {
        public string SystemName { get; set; }
        public string SystemKey { get; set; }
        public string PhoneNumber { get; set; }
        public string MessageText { get; set; }
    }
}
