using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class RequestInfo
    {
        public string status { get; set; }

        public string country { get; set; }

        public string countryCode { get; set; }

        public string region { get; set; }

        public string regionName { get; set; }

        public string city { get; set; }

        public float? zip { get; set; }

        public float lat { get; set; }

        public float lon { get; set; }

        public string timezone { get; set; }

        public string org { get; set; }

        public string assest { get; set; }

        public int? mobile { get; set; }

        public int? proxy { get; set; }

        public string query { get; set; }
    }
}
