using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class ReturnOrderHeader
    {
        [JsonProperty(PropertyName = "storecd")]
        public string jumcd { get; set; }

        //request date
        public string ilja { get; set; }

        //return no
        public string retno { get; set; }

        //contract code
        public string ctrcd { get; set; }

        //contract name
        public string ctrnm { get; set; }

        //return qty

        public decimal? qty { get; set; }

        //vatgui dun

        public decimal? supply { get; set; }

        //vat
        public decimal? supplyvat { get; set; }

        // vattai dun
        public decimal? supplyamt { get; set; }

        public string notifinfo { get; set; }


        //medegdel xamgiin suuld ilgeesen ognoo
        public string sndymd { get; set; }
    }
}
