using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI.Helpers
{
    public class ResponceClient
    {
        public bool  Success { get; set; }

        public object Value { get; set; }

        public string Message { get; set; }
    }
}
