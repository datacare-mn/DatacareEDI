using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Utils
{
    public static class CompanyLogUtils
    {
        public static void SaveCompanyLog(HttpContext context)
        {
            string comname =Convert.ToString(UsefulHelpers.GetIdendityValue(context, Enums.SystemEnums.UserProperties.CompanyName));
            string jsonData = File.ReadAllText(@"appsettings.json");
            string data = Convert.ToString(JsonConvert.DeserializeObject(jsonData));
            dynamic request = JObject.Parse(data.Replace("[", "").Replace("]", ""));

            string logpath = request.LogFilePath;

            string path = logpath + $"//{DateTime.Today.ToString("yyyy-MM-dd")}//" + comname;
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
        }

    }
}
