using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using EDIWEBAPI.Entities.CustomModels;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using EDIWEBAPI.Context;
using Microsoft.Extensions.Logging;

namespace EDIWEBAPI.Controllers.SendData
{
    public class Messager
    {
        public static string DEVELOPER_PHONE_NO = "90005881";
        public static async Task<ResponseClient> Send(string msg, string phonenumber)
        {
            if (phonenumber == null || Convert.ToString(phonenumber).Length != 8)
                return ReturnResponce.NotFoundResponce();

            var messagedata = new MessageRequest()
            {
                MessageText = msg,
                PhoneNumber = phonenumber,
                SystemKey = "171911",
                SystemName = "urto.mn"
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigData.GetCongifData(ConfigData.ConfigKey.MessageServiceURL));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
                var text = JsonConvert.SerializeObject(messagedata, settings);
                var content = new StringContent(text, Encoding.UTF8, "application/json");
                var responsePost = await client.PostAsync("SMSService/rest/sendsms", content);
                //   HttpResponseMessage responsePost = await client.PostAsync("api/message/sendsms", content);
                if (responsePost.ReasonPhrase == "OK")
                {
                    Task<string> data = responsePost.Content.ReadAsStringAsync();
                    JObject rss = JObject.Parse(data.Result.ToString());
                    var rest = JsonConvert.DeserializeObject<ResponseClient>(data.Result.ToString());
                    return rest.Success ?
                        ReturnResponce.SuccessMessageResponce(rest.Message) :
                        ReturnResponce.FailedMessageResponce(rest.Message);
                }
                else
                {
                    return ReturnResponce.FailedMessageResponce(responsePost.ReasonPhrase);
                }
            }
        }
        
    }
}
