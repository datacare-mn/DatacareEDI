using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class HttpRequestModel
    {
        public string AppName { get; set; }
        public int CompanyID { get; set; }

        public string BaseApi { get; set; }

        public string  UserName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public int? ExpiresIn { get; set; }

        public DateTime? ExpireTime { get; set; }
    }
}
