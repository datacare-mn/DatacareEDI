using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Utils
{
    public static class ConfigData
    {
        public enum ConfigKey
        {
            FileServerURL = 1,
            StorePaymentMail = 2,
            ResetPasswordMail = 3,
            MessageServiceURL = 4,
            ShortURL = 5,
            HostDevelopmentMode  = 6,
            NotifcationServerAddress = 7,
            DisableDeviceURL = 8,
            MaxPricePercent = 9,
            MinUserAmount = 10,
            EditCompanyURL = 11
        }

        public static string GetConfigData(ConfigKey key, string defaultValue)
        {
            var data = GetCongifData(key);
            return string.IsNullOrEmpty(data) ? defaultValue : data;
        }

        public static string GetCongifData(ConfigKey jsonkey)
        {
            string jsonData = File.ReadAllText(@"ediconfig.json");
            string data = Convert.ToString(JsonConvert.DeserializeObject(jsonData));
            dynamic request = JObject.Parse(data.Replace("[", "").Replace("]", ""));

            if (jsonkey == ConfigKey.StorePaymentMail)
            {
                return request.StorePaymentMail;
            }
            if (jsonkey == ConfigKey.FileServerURL)
            {
                return request.FileServerURL;
            }
            if (jsonkey == ConfigKey.ResetPasswordMail)
            {
                return request.ResetPasswordMail;
            }
            if (jsonkey == ConfigKey.MessageServiceURL)
            {
                return request.MessageServiceURL;
            }
            if (jsonkey == ConfigKey.ShortURL)
            {
                return request.ShortURL;
            }
            if (jsonkey == ConfigKey.HostDevelopmentMode)
            {
                return request.HostDevelopmentMode;
            }
            if (jsonkey == ConfigKey.NotifcationServerAddress)
            {
                return request.NotifcationServerAddress;
            }
            if (jsonkey == ConfigKey.DisableDeviceURL)
            {
                return request.DisableDeviceURL;
            }
            if (jsonkey == ConfigKey.EditCompanyURL)
            {
                return request.EditCompanyURL;
            }
            if (jsonkey == ConfigKey.MaxPricePercent)
            {
                return request.MaxPricePercent;
            }
            if (jsonkey == ConfigKey.MinUserAmount)
            {
                return request.MinUserAmount;
            }

            return "";
        }


        

    }
}
