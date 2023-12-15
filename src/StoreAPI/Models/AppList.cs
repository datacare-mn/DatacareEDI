using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI.Models
{
    public class AppList
    {
        public string APPKEY { get; set; }

        public int APPID { get; set; }

        public string APPNAME { get; set; }

        public string CurrentVersion { get; set; }
    }
}
