using EDIWEBAPI.Attributes;
using EDIWEBAPI.Context;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/test")]
    public class TestController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<TestController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        /// 

        public TestController(OracleDbContext context, ILogger<TestController> log)
        {
            _dbContext = context;
            _log = log;
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
      //  [AllowAnonymous]
        
        [ServiceFilter(typeof(LogFilter))]
        [ServiceFilter(typeof(LicenseAttribute))]

        public ResponseClient Get(int id)
        {
            
            var message = "sdasdasd";
            if (message != null)
            {
                return ReturnResponce.ListReturnResponce(message);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        [HttpGet]
        [Route("datanotlicensed/{id}")]
        [Authorize]
        [ServiceFilter(typeof(LicenseAttribute))]
        //  [AllowAnonymous]

        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetData(int id)
        {
            var message = "sdasdasd";
            if (message != null)
            {
                return ReturnResponce.ListReturnResponce(message);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }


        [HttpGet]
        [Route("data/{id}")]
        [Authorize]
        [ServiceFilter(typeof(LicenseAttribute))]
        //  [AllowAnonymous]

        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetDataNot(int id)
        {
            var message = "sdasdasd";
            if (message != null)
            {
                return ReturnResponce.ListReturnResponce(message);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        [HttpPost]
        [Route("sendsms/{msg}")]
        [AllowAnonymous]

        public ResponseClient PostMessage([FromBody] string msg)
        {
            string token = "EAAdhJ5TY6HUBAKRyphb56ki2xGTv5z7J497vymUFQKlpaTs1wEORTBy8n2ZAx6LcPwBTtYZAwpE431SELF893d2t1oJbhyD9mYxUPqUlZC2aAv775ltUKOga4iVdEsSzLpFcSupbTjEtKiLZBwOA4ciZAHphGUp6Oyw3FBnt6tzeAdtZBgh0iZB";
            string url = $"https://graph.facebook.com/v2.6/2077147465836661/messages";
            FacebookMessage fbmessage = new FacebookMessage();
            message sms = new message();
            sms.text = msg;
            recipient rst = new recipient();
            rst.id = "2077147465836661";// "2077147465836661";

            fbmessage.message = sms;
            fbmessage.recipient = rst;
            fbmessage.messaging_type = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(JsonConvert.SerializeObject(fbmessage).Replace("null", "\"\""), Encoding.UTF8, "application/json");
                var responsePost = client.PostAsync($"access_token = '{token}'", content);
                if (responsePost.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return ReturnResponce.SuccessMessageResponce("00");
                }
                else
                {
                    return ReturnResponce.FailedMessageResponce("asdas");
                }

            }
        }



        #endregion
    }

    public class FacebookMessage
    {
        public string messaging_type { get; set; }

        public recipient recipient { get; set; }

        public message message { get; set; }
    }

    public class recipient
    {
        public string id { get; set; }
    }

    public class message
    {
        public string  text { get; set; }
    }


}
