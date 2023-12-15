using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class DisableRequest
    {
        public int ID { get; set; }
        public string UserMail { get; set; }
        public string Method { get; set; }
    }
}
