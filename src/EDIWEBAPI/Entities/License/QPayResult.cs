using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    public class QPayResult
    {
        [JsonProperty("type")]
        public string type { get; set; }
        public string result_code { get; set; }

        public string invoiceno { get; set; }

        public string result_msg { get; set; }

        [JsonProperty("json_data")]
        public QPayResultJson json_data { get; set; }
    }

    public class QPayResultJson
    {
        public string invoice_id { get; set; }

        public string qPay_QRcode { get; set; }

        public string qPay_QRimage { get; set; }
    }
}
