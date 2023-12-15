using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class UserDetailMenuData
    {
        public int? MENUID { get; set; }

        public int? PARENTID { get; set; }

        public string MENUNAME { get; set; }

        [JsonProperty(PropertyName ="title")]
        public string MENUCAPTION { get; set; }

        [JsonProperty(PropertyName = "route")]
        public string MENUURL { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string MENUICON { get; set; }

        public int? ORDER { get; set; }
    }
}
