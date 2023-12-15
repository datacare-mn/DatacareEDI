using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Helpers
{
    public class ResponseClient
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Value { get; set; }

    }
}
