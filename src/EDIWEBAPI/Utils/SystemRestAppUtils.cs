using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Controllers.SysManagement;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Context;
using Newtonsoft.Json.Linq;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using EDIWEBAPI.Entities.License;
using Microsoft.Extensions.Logging;

namespace EDIWEBAPI.Utils
{
    public class SystemRestAppUtils
    {
        public HttpRequestModel HttpModel { get; set; }
        public bool ServerConnected { get; set; }
        public string Token { get; set; }
        

        public SystemRestAppUtils(OracleDbContext _dbcontext, ILogger _log, string appname, int storeid)
        {
            try
            {
                HttpModel = Logics.ManagementLogic.GetHttpModelData(_dbcontext, storeid, appname);
                if (HttpModel != null)
                {
                    Token = PrepareToken(_dbcontext, _log);
                    ServerConnected = true;
                }

            }
            catch (Exception ex)
            {
                ServerConnected = false;
                //MethodBase methodBase = MethodBase.GetCurrentMethod();
                //_log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                throw ex;
            }
        }

        public string PrepareToken(OracleDbContext _dbcontext, ILogger _log)
        {
            var logMethod = "SystemRestAppUtils.PrepareToken";
            try
            {
                //var expTime = HttpModel.ExpireTime.Value.AddSeconds(HttpModel.ExpiresIn.Value);
                if (DateTime.Now > HttpModel.ExpireTime)
                {
                    var TokenData = Login(HttpModel, _log);
                    if (TokenData != null)
                    {
                        Logics.ManagementLogic.UpdateAppToken(_dbcontext, HttpModel.CompanyID, TokenData, HttpModel.AppName);
                        return TokenData.Token;
                    }
                    else
                    {
                        _log.LogInformation($"{logMethod} TOKEN NULL", HttpModel.AppName);
                        return null;
                    }
                }
                return HttpModel.Token;
            }
            catch (Exception ex)
            {
                _log.LogInformation($"{logMethod} :{ex.Message}", HttpModel.AppName);
                return null;
            }

        }
        
        public TokenValue Login(HttpRequestModel reqModel, ILogger _log)
        {
            var logMethod = "SystemRestAppUtils.Login";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(reqModel.BaseApi);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var accuser = new AccountApiUser()
                    {
                        password = reqModel.Password,
                        userName = reqModel.UserName
                    };

                    var response = client.PostAsJsonAsync("api/Authentication", accuser).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> data = response.Content.ReadAsStringAsync();
                        JObject rss = JObject.Parse(data.Result.ToString());
                        dynamic httpreqdata = JObject.Parse(rss.ToString());

                        return new TokenValue()
                        {
                            ExpiresIn = httpreqdata.expires_in,
                            ExpireTime = httpreqdata.expiretime,
                            Token = httpreqdata.access_token
                        };
                    }
                    else
                    {
                        _log.LogInformation($"{logMethod} LOGIN UNSUCCESSFUL :{response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation($"{logMethod} :{ex.Message}");
                throw ex;
            }
        }



        public async Task<ResponseClient> Get(ILogger _log, string geturl, bool isResponceClient = false)
        {
            var logMethod = "SystemRestAppUtils.Get";
            try
            {
                if (Token == null)
                    return ReturnResponce.FailedMessageResponce("Token not found");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(HttpModel.BaseApi);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    var response = await client.GetAsync(geturl);
                    if (!response.IsSuccessStatusCode)
                        return ReturnResponce.FailedMessageResponce(geturl);
                    
                    Task<string> data = response.Content.ReadAsStringAsync();
                    var rss = JObject.Parse(data.Result.ToString());
                    if (isResponceClient)
                        return ReturnResponce.ListReturnResponce(JsonConvert.DeserializeObject<QPayResult>(data.Result.ToString()));
                    
                    return JsonConvert.DeserializeObject<ResponseClient>(data.Result.ToString());
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation($"{logMethod} :{ex.Message}");
                throw ex;
            }
        }






        public async Task<string> GetQR(ILogger _log, string geturl)
        {
            var logMethod = "SystemRestAppUtils.GetQR";
            try
            {
                if (Token != null)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(HttpModel.BaseApi);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                        HttpResponseMessage response = await client.GetAsync(geturl);
                        if (response.IsSuccessStatusCode)
                        {
                            Task<string> data = response.Content.ReadAsStringAsync();
                            JObject rss = JObject.Parse(data.Result.ToString());
                            return data.Result.ToString();
                        }
                        return "Сервертэй холбогдож чадсангүй!";
                    }
                }
                else
                {
                    return "Token not found";
                }

            }
            catch (Exception ex)
            {
                _log.LogInformation($"{logMethod} :{ex.Message}");
                throw ex;
            }
        }
    }


}


public class AccountApiUser
    {
        public string userName { get; set; }

        public string password { get; set; }
    }

