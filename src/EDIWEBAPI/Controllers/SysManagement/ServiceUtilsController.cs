
using EDIWEBAPI.Attributes;
using EDIWEBAPI.Context;
using EDIWEBAPI.Controllers.Storeapi;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Entities.FilterViews;
using EDIWEBAPI.Enums;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/serviceutils")]
    public class ServiceUtilsController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<ServiceUtilsController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public ServiceUtilsController(OracleDbContext context, ILogger<ServiceUtilsController> log)
        {
            _dbContext = context;
            _log = log;
        }


        #endregion

        [HttpGet]
        [Authorize]
        [Route("spellmoney/{money}")]
        //[ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetSpellMoney(decimal money)
        {
            return ReturnResponce.ListReturnResponce(CurrencyAmountSpeller.SpellMoney(Convert.ToString(money).TrimStart(), true));
        }

        #region InvoiceNo

        [HttpGet]
        [Authorize]
        [Route("invoiceno")]
        [ServiceFilter(typeof(LogFilter))]
        public  ResponseClient GetInvoiceNo()
        {
          var currentinvoice =  _dbContext.SYSTEM_CONFIGDATA.FirstOrDefault(x=> x.KEYDATA ==  "INVOICENO");
            if (currentinvoice != null)
            {
                string invoiceno = Convert.ToString(currentinvoice.KEYVALUE);
                string today = DateTime.Now.ToString("yyMMdd");
                if (invoiceno != null)
                {
                    if (invoiceno.Length < 6)
                    {
                        invoiceno = "1";
                        string retinvno = Convert.ToString(invoiceno);
                        retinvno = retinvno.PadLeft(4, '0');
                        currentinvoice.KEYVALUE = $"{today}{retinvno}";
                        _dbContext.Entry(currentinvoice).State = System.Data.Entity.EntityState.Modified;
                        _dbContext.SaveChanges();
                        return ReturnResponce.ListReturnResponce(currentinvoice.KEYVALUE);
                    }
                    if (invoiceno.Substring(0, 6) == today)
                    {
                        currentinvoice.KEYVALUE = Convert.ToString(Convert.ToDecimal(invoiceno) + 1);
                        _dbContext.Entry(currentinvoice).State = System.Data.Entity.EntityState.Modified;
                        _dbContext.SaveChanges();
                        return ReturnResponce.ListReturnResponce(currentinvoice.KEYVALUE);

                    }
                    else
                    {
                        invoiceno = "1";
                        string retinvno = Convert.ToString(invoiceno);
                        retinvno = retinvno.PadLeft(4, '0');
                        currentinvoice.KEYVALUE = $"{today}{retinvno}";
                        _dbContext.Entry(currentinvoice).State = System.Data.Entity.EntityState.Modified;
                        _dbContext.SaveChanges();
                        return ReturnResponce.ListReturnResponce(currentinvoice.KEYVALUE);
                    }
                }
                else
                {
                    invoiceno = "1";
                    string retinvno = Convert.ToString(invoiceno);
                    retinvno = retinvno.PadLeft(4, '0');
                    currentinvoice.KEYVALUE = $"{today}{retinvno}";
                    _dbContext.Entry(currentinvoice).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges();
                    return ReturnResponce.ListReturnResponce(currentinvoice.KEYVALUE);
                }
            }
            else
            {
                string today = DateTime.Now.ToString("yyMMdd");
                string retinvno = "1";
                retinvno = retinvno.PadLeft(4, '0');
                SYSTEM_CONFIGDATA invoicenew = new SYSTEM_CONFIGDATA();
                invoicenew.KEYDATA = "INVOICENO";
                invoicenew.KEYVALUE = $"{today}{retinvno}";
                _dbContext.SYSTEM_CONFIGDATA.Add(invoicenew);
                _dbContext.SaveChanges();
                return ReturnResponce.ListReturnResponce(invoicenew.KEYVALUE);
            }
        }

        #endregion

        #region Url shortner

        [HttpGet]
        [Authorize]
        [Route("urlshortnercreate/{originalurl}")]
        public async Task<ResponseClient> GetUrlShortner(string originalurl, MessageType type, decimal? recordId)
        {
            try
            {
                //DateTime orderDate = DateTime.Now.AddDays(-14);
                //DateTime returnDate = DateTime.Now.AddDays(-60);
                //var oldorders = _dbContext.SYSTEM_SHORTURL.Where(x => (x.TYPE == "ORDER" && x.INSYMD <= orderDate) || (x.TYPE == "RETURN" && x.INSYMD <= returnDate));
                //if (oldorders.ToList().Count > 0)
                //{
                //    _dbContext.SYSTEM_SHORTURL.RemoveRange(oldorders);
                //}
                var expireDate = type == MessageType.Return ? DateTime.Now.AddDays(-60) : DateTime.Now.AddDays(-14);
                string shorturl = GetShortUrl(5);

                var url = new SYSTEM_SHORTURL()
                {
                    SHORTURL = Convert.ToString(ConfigData.GetCongifData(ConfigData.ConfigKey.ShortURL)) + shorturl,
                    LONGURL = originalurl.Replace("%2F", "/"),
                    TYPE = type.ToString().ToUpper(),
                    VISITCOUNT = 0,
                    INSYMD = DateTime.Now,
                    LASTREQDATE = DateTime.Now,
                    EXPIREDATE = expireDate,
                    RECORDID = recordId
                };

                Logics.BaseLogic.Insert(_dbContext, url);
                //_dbContext.SYSTEM_SHORTURL.Add(url);
                //_dbContext.SaveChanges();

                return ReturnResponce.ListReturnResponce(url.SHORTURL);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, $"{UsefulHelpers.GetMethodName(System.Reflection.MethodBase.GetCurrentMethod())} => {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }




        [HttpPost]
        [AllowAnonymous]
        [Route("shortlink/{shorturl}")]
        public async Task<ResponseClient> UrlShortner(string shorturl)
        {
            string urls = shorturl.Replace("%2F", "/");
            var url = _dbContext.SYSTEM_SHORTURL.FirstOrDefault(x => x.SHORTURL == urls);
            if (url == null)
                return ReturnResponce.NotFoundResponce();

            url.LASTREQDATE = DateTime.Now;
            url.VISITCOUNT = (url.VISITCOUNT + 1);
            _dbContext.Entry(url).State = System.Data.Entity.EntityState.Modified;
            _dbContext.SaveChanges();
            return ReturnResponce.ListReturnResponce(url.LONGURL);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("mailreadinfo")]
        public ResponseClient MailReadInfo()
        {
            string currentIP = HttpContext.GetClientIPAddress();
            RequestApprove(currentIP);

          return ReturnResponce.SuccessMessageResponce("ok");
        }


        private async Task RequestApprove(string ip)
        {
            try
            {
                string api = $"json/{ip}";
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://ip-api.com/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(api).Result;
                if (response.IsSuccessStatusCode)
                {
                    Task<string> data = response.Content.ReadAsStringAsync();
                    JObject rss = JObject.Parse(data.Result.ToString());
                    JObject json = JObject.Parse(rss.ToString());
                    if (Convert.ToString(json["status"]) == "success")
                    {
                        SYSTEM_LOGIN_REQUEST req = new SYSTEM_LOGIN_REQUEST();
                        req.ID = Convert.ToDecimal(_dbContext.GetTableID("SYSTEM_LOGIN_REQUEST"));
                        req.CITY = Convert.ToString(json["city"]);
                        req.IPADDRESS = Convert.ToString(json["query"]);
                        req.ISP = Convert.ToString(json["org"]);
                        req.COUNTRY = Convert.ToString(json["country"]);
                        req.COUNTRYCODE = Convert.ToString(json["countryCode"]);
                        req.ISPCOMPANYDET = Convert.ToString(json["as"]);
                        req.LAT = Convert.ToString(json["lat"]);
                        req.LON = Convert.ToString(json["lon"]);
                        req.REGIONNAME = Convert.ToString(json["regionName"]);
                        req.REQTIMEZONE = Convert.ToString(json["timezone"]);
                        req.REQUESTDATE = DateTime.Now;
                        req.USERNAME = "PUBLICMAILREAD";
                        _dbContext.SYSTEM_LOGIN_REQUEST.Add(req);
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        SYSTEM_LOGIN_REQUEST req = new SYSTEM_LOGIN_REQUEST();
                        req.ID = Convert.ToDecimal(_dbContext.GetTableID("SYSTEM_LOGIN_REQUEST"));
                        req.REQUESTDATE = DateTime.Now;
                        req.USERNAME = "PUBLICMAILREAD";
                        req.IPADDRESS = ip;
                        req.FAILDETAIL = Convert.ToString(json);
                        _dbContext.SYSTEM_LOGIN_REQUEST.Add(req);
                        _dbContext.SaveChanges();

                    }
                }

            }
            catch (Exception ex)
            {

            }
        }




        [HttpGet]
        [Authorize]
        [Route("feedbackpost/{title}/{desc}/{rate}")]
        public async Task<ResponseClient> FeedBackPost(string title, string desc, int rate)
        {
            try
            {
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                string usermail = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserMail));

                var feedback = new SYSTEM_FEEDBACK()
                {
                    ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_FEEDBACK")),
                    RATEINDEX = rate,
                    REGDATE = DateTime.Now,
                    USERNAME = usermail,
                    FEEDBACKNAME = title,
                    FEEDBACKDESC = desc
                };

                Logics.BaseLogic.Insert(_dbContext, feedback);
                return ReturnResponce.SuccessMessageResponce("Таны хүсэлтийг хүлээн авлаа танд баярлалааа");
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }

        [HttpPost]
        [Authorize(Policy = "EdiApiUser")]
        [Route("showfeedback")]
        public async Task<ResponseClient> ShowBackPost([FromBody]FeedbackFilterView filter)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var result = _dbContext.GET_FEEDBACKS(filter);
            if (result == null)
                return ReturnResponce.NotFoundResponce();

            return ReturnResponce.ListReturnResponce(result);
        }


        private string GetShortUrl(int length)
        {
            string shortcurl = Cryptography.CreateShorturl(length);
            var shorturl =  _dbContext.SYSTEM_SHORTURL.FirstOrDefault(x=> x.SHORTURL == shortcurl);
            if (shorturl == null)
            {
                return Convert.ToString(shortcurl);
            }
            else
            {
               return  GetShortUrl(length);
            }
        }

        private List<STORENOTIFDATA> GetNotifcationData(string contractno, string menuname)
        {
            return  _dbContext.GET_STORENOTIFDATA(contractno, menuname).ToList();
        }


        #endregion


        #region NotifcationData
        [HttpGet]
        [Route("notifcationdata/{notifcationtype}/{storeid}/{comid}/{recordid}")]
        [AllowAnonymous]
        public async Task<ResponseClient> SendNotifcationData(SystemEnums.NotifcationType notifcationtype, int? storeid, int? comid, decimal recordid)
        {
            try
            {
                if (notifcationtype == SystemEnums.NotifcationType.Захиалга || notifcationtype == SystemEnums.NotifcationType.Буцаалт)
                {
                    var notifdata = new SYSTEM_NOTIFCATION_DATA()
                    {
                        COMID = comid,
                        ID = Convert.ToString(Guid.NewGuid()),
                        NOTIFMODULETYPE = (int)notifcationtype,
                        CREATEDDATE = DateTime.Now,
                        RECORDID = recordid,
                        STOREID = storeid,
                        ISSEEN = 0
                    };

                    var currentstore = _dbContext.SYSTEM_ORGANIZATION.FirstOrDefault(x => x.ID == storeid);
                    var notdata = _dbContext.SYSTEM_ORGANIZATION.Where(x => x.ID == comid).Select(x => new
                    {
                        regno = x.REGNO,
                        companyid = x.ID,
                        companyname = x.COMPANYNAME,
                        id = notifdata.ID,
                        createddate = notifdata.CREATEDDATE,
                        moduletype = notifdata.NOTIFMODULETYPE,
                        recordid = notifdata.RECORDID,
                        storeid = notifdata.STOREID,
                        storename = currentstore.COMPANYNAME,
                        isseen = notifdata.ISSEEN
                    });

                    var postdata = JsonConvert.SerializeObject(notdata);
                    var data = PushNotifDataOrder(postdata, "/api/sendnotif").Result;

                    Logics.BaseLogic.Insert(_dbContext, notifdata);
                    //_dbContext.Entry(notifdata).State = System.Data.Entity.EntityState.Added;
                    //_dbContext.SaveChanges();
                }
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }



        [HttpGet]
        [Route("notifcationseen/{notificationtype}/{notificationid}/{recordid}")]
        [Authorize]
        public ResponseClient SeenNotifcationData(int notificationtype, string notificationid, int recordid)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                var contractNo = string.Empty;
                if ((int)SystemEnums.NotifcationType.Захиалга == notificationtype)
                {
                    var data = _dbContext.REQ_ORDER.FirstOrDefault(x => x.ORDERID == recordid);
                    if (data != null)
                        contractNo = data.CONTRACTNO;
                }
                else if ((int)SystemEnums.NotifcationType.Буцаалт == notificationtype)
                {
                    var data = _dbContext.REQ_RETURN_ORDER.FirstOrDefault(x => x.RETORDERID == recordid);
                    if (data != null)
                        contractNo = data.CONTRACTNO;
                }
                
                if (string.IsNullOrEmpty(contractNo))
                    return ReturnResponce.NotFoundResponce();
                
                var notification = _dbContext.SYSTEM_NOTIFCATION_DATA.FirstOrDefault(x => x.ID == notificationid);
                if (notification == null)
                    return ReturnResponce.NotFoundResponce();

                notification.ISSEEN = 1;
                Logics.BaseLogic.Update(_dbContext, notification);

                var sdate = DateTime.Today.AddMonths(-12);
                var edate = DateTime.Now;

                //var businessid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
                //var contractlist = DbUtils.GetBusinessContractList(HttpContext, _dbContext);
                
                var response = ((int)SystemEnums.NotifcationType.Захиалга == notificationtype) ?
                    Logics.OrderLogic.GetOrderHeader(_dbContext, notification.STOREID.Value, comid, "%", contractNo, sdate, edate, "%", recordid) :
                    Logics.OrderLogic.GetReturnHeader(_dbContext, notification.STOREID.Value, comid, "%", contractNo, sdate, edate, "%", recordid);

                //if (!response.Success)
                    return response;

                //OrderController.GetOrderHeaderByID(Convert.ToInt32(notification.RECORDID), Convert.ToInt32(notification.STOREID), HttpContext).Result :
                //OrderController.GetReturnOrderHeaderByID(Convert.ToInt32(notification.RECORDID), Convert.ToInt32(notification.STOREID), HttpContext).Result;

                //var value = JsonConvert.DeserializeObject<List<object>>(json);
                //UsefulHelpers.SerializeLower();
                //return ReturnResponce.ListReturnResponce(value);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpGet]
        [Route("unseennotifcation")]
        [Authorize]
        public ResponseClient UnSeenNotifcation()
        {
            try
            {
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
                var currentcompany = _dbContext.SYSTEM_ORGANIZATION.FirstOrDefault(x => x.ID == comid);
                var query = from c in _dbContext.SYSTEM_NOTIFCATION_DATA
                            join org in _dbContext.SYSTEM_ORGANIZATION
                            on c.STOREID equals org.ID
                            where c.COMID == comid && c.ISSEEN == 0
                            select new { currentcompany.REGNO, COMPANYID = currentcompany.ID, c.ID, c.CREATEDDATE, c.NOTIFMODULETYPE, c.RECORDID, c.STOREID, STORENAME = org.COMPANYNAME, c.ISSEEN };
                return ReturnResponce.ListReturnResponce(query);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }


        async Task<string> PushNotifDataOrder(string json, string url)
        {
            var baseapi = Convert.ToString(ConfigData.GetCongifData(ConfigData.ConfigKey.NotifcationServerAddress));
            var httpClient = new HttpClient();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseapi);
                client.DefaultRequestHeaders.Accept.Clear();
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    Task<string> datastring = response.Content.ReadAsStringAsync();
                    return datastring.Result.ToString();
                }
                return "";
            }
        }



        #endregion


        #region SMS

        [HttpGet]
        [AllowAnonymous]
        [Route("getrequests/{count}/{key}")]
        public async Task<ResponseClient> GetRequests(int count, string key)
        {
            try
            {
                if (!SendData.MailSendController.HasPrivilige(key))
                    return ReturnResponce.AccessDeniedResponce();

                var requests = Logics.MailLogic.GetSMSRequests(_dbContext, count);
                if (requests != null && requests.Any())
                    return ReturnResponce.ListReturnResponce(requests.ToList());
                else
                    return ReturnResponce.SuccessMessageResponce(string.Empty);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("message/{msg}/{phonenumber}")]
        public async Task<ResponseClient> SmsSend(string msg, string phonenumber, string messageid = null)
        {
            try
            {
                Logics.MailLogic.AddSMS(_dbContext, MessageType.None, msg, phonenumber);
                return ReturnResponce.SuccessMessageResponce("");
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("sendrequest/{messageid}/{key}")]
        public async Task<ResponseClient> SendRequest(string messageid, string key)
        {
            try
            {
                if (!SendData.MailSendController.HasPrivilige(key))
                    return ReturnResponce.AccessDeniedResponce();

                var result = await Logics.MailLogic.SendSMS(_dbContext, _log, messageid);
                return result;
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpGet]
        [Route("messagearchive/{sdate}/{edate}/{issent}")]
        [Authorize]
        public async Task<ResponseClient> GetMessage(DateTime sdate, DateTime edate, int issent)
        {
            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            if (orgType == 1)
                return ReturnResponce.AccessDeniedResponce();

            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var response = Logics.MailLogic.GetSmsLogs(_dbContext, sdate, edate.AddDays(1).AddSeconds(-1),
                orgType == 2 ? comid : 0, issent);

            return ReturnResponce.ListReturnResponce(response);
        }





        [HttpGet]
        [Route("resendmessage/{messageid}")]
        [Authorize]
        public async Task<ResponseClient> ResendMessage(string messageid)
        {
            return await Logics.MailLogic.SendSMS(_dbContext, _log, messageid);
        }

        private bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^(\+[0-9]{9})$").Success;
        }

        #endregion

        private  string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("getmessagedata")]
        public ResponseClient GetMessageData()
        {
            string urlAddress = "http://10.0.10.62/default/en_US/send.html?u=admin&p=admin&l=1&n=91118340&m=hi";
            try
            {
                goip  goips = new goip();
                goips.line = "1";
                goips.action = "SMS";
                goips.telnum = "91118340";
                goips.smscontent = "hi sainyy";
                goips.send = "Send";

                string postadata= "line=1&action=SMS&telnum=91118340&smscontent=hi&send=Send";

                var webRequest = WebRequest.Create("http://10.0.10.62/default/en_US/sms_info.html?type=sms");
                webRequest.Method = "POST";
                webRequest.Credentials = new NetworkCredential(Base64Encode("admin"), Base64Encode("admin"));
                var bytes = Encoding.UTF8.GetBytes(postadata);
                webRequest.ContentLength = bytes.Length;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                using (var requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    StreamReader reader = new StreamReader(requestStream);
                    string responseFromServer = reader.ReadToEnd();

                }
            }
            catch (Exception ex)
            {

            }
            

          //string data =SendWebRequest2(urlAddress);
            ////string urlAddress = "http://www.google.com";
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);

            //request.ProtocolVersion = System.Net.HttpVersion.Version10;
            //request.KeepAlive = false;
            //ServicePointManager.DefaultConnectionLimit = 2;
            //ServicePointManager.Expect100Continue = false;

            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    Stream receiveStream = response.GetResponseStream();
            //    StreamReader readStream = null;

            //    if (response.CharacterSet == null)
            //    {
            //        readStream = new StreamReader(receiveStream);
            //    }
            //    else
            //    {
            //        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            //    }

            //    string data = readStream.ReadToEnd();

            //    response.Close();
            //    readStream.Close();

            //    return ReturnResponce.SuccessMessageResponce(data);
            //}
            //  WebClient client = new WebClient();

            ////  Uri url = new Uri("http://10.0.10.62/default/en_US/send.html?u=admin&p=admin&l=1&n=91118340&m=hi", false);
            ////  client.DownloadStringAsync(url);
            // string data =  client.DownloadStringTaskAsync(urlAddress).Result;
            return ReturnResponce.SuccessMessageResponce("");
        }
        async Task<string> SendWebRequest(string requestUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(requestUrl))
                    return await response.Content.ReadAsStringAsync();
            }
        }

        string SendWebRequest2(string requestUrl)
        {
            ServicePointManager.DefaultConnectionLimit = 2;
            ServicePointManager.Expect100Continue = false;
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = client.GetAsync(requestUrl).GetAwaiter().GetResult())
                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        static string Get_HTML(string Url)
        {
            System.Net.WebResponse Result = null;
            string Page_Source_Code;
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(Url);
               
                Result = req.GetResponse();
                System.IO.Stream RStream = Result.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(RStream);
                new System.IO.StreamReader(RStream);
                Page_Source_Code = sr.ReadToEnd();
                sr.Dispose();
            }
            catch
            {
                // error while reading the url: the url dosen’t exist, connection problem...
                Page_Source_Code = "";
            }
            finally
            {
                if (Result != null) Result.Close();
            }
            return Page_Source_Code;
        }
    }

    public class goip
    {
        public string line { get; set; }

        public string smskey { get; set; }

        public string action { get; set; }

        public string telnum { get; set; }

        public string smscontent { get; set; }

        public string send { get; set; }
    }
}
