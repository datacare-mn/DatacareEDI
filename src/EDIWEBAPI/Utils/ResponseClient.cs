using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Utils
{
    public class ResponseClient
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Value { get; set; }
        public int RowCount { get; set; }
        public ResponseClient()
        {
            this.RowCount = 0;
        }
    }
}
