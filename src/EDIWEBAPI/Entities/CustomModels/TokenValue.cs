using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class TokenValue
    {
        public string Token { get; set; }

        public DateTime? ExpireTime { get; set; }

        public int? ExpiresIn { get; set; }
    }
}
