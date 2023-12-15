using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.CustomModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EDIWEBAPI.Utils
{
    public  class HttpRestUtils
    {
       // readonly ILogger<HttpRestUtils> _log;
        public HttpRequestModel HttpModel { get; set; }
        public OracleDbContext _dbcontext { get; set; }
        public bool  StoreServerConnected { get; set; }
        public string Token { get; set; }
        public  HttpRestUtils(int comid, OracleDbContext dbcontext)
        {
            try
            {
                _dbcontext = dbcontext;
                HttpModel = Logics.ManagementLogic.GetHttpModelData(_dbcontext, comid);
                Token = PrepareToken(comid, _dbcontext);
                StoreServerConnected = true;
            }
            catch (Exception ex)
            {
                StoreServerConnected = false;
                throw ex;
            }
        }
        public  string PrepareToken(int comid, OracleDbContext _dbcontext)
        {
            try
            {
                var expTime = HttpModel.ExpireTime.Value.AddSeconds(HttpModel.ExpiresIn.Value);
                if (DateTime.Now > expTime)
                {
                    var TokenData = Login(HttpModel);
                    if (TokenData != null)
                    {
                        Logics.ManagementLogic.UpdateAppToken(_dbcontext, comid, TokenData);
                        return TokenData.Token;
                    }
                    else
                        return null;
                }
                return HttpModel.Token;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public async  Task<ResponseClient> Get(string geturl)
        {
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
                           return JsonConvert.DeserializeObject<ResponseClient>(data.Result.ToString());
                        }
                        return ReturnResponce.FailedMessageResponce(geturl);
                    }
                }
                else
                {
                    return ReturnResponce.FailedMessageResponce("Token not found");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseClient> Post(string posturl, string postdata)
        {
            try
            {
                if (Token == null)
                    return ReturnResponce.FailedMessageResponce("Token not found");

                var httpClient = new HttpClient();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(HttpModel.BaseApi);
                    client.DefaultRequestHeaders.Accept.Clear();
                    var content = new StringContent(postdata, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    var response = await client.PostAsync(posturl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> data = response.Content.ReadAsStringAsync();
                        JObject rss = JObject.Parse(data.Result.ToString());
                        return JsonConvert.DeserializeObject<ResponseClient>(data.Result.ToString());
                    }
                    return ReturnResponce.FailedMessageResponce(posturl);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<ResponseClient> Post(string posturl, IFormFile file)
        {
            try
            {
                if (Token == null)
                    return ReturnResponce.FailedMessageResponce("Token not found");

                var httpClient = new HttpClient();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(HttpModel.BaseApi);
                    client.DefaultRequestHeaders.Accept.Clear();

                    byte[] byteData;
                    using (var br = new BinaryReader(file.OpenReadStream()))
                        byteData = br.ReadBytes((int)file.OpenReadStream().Length);

                    var bytes = new ByteArrayContent(byteData);
                    var multiContent = new MultipartFormDataContent();
                    multiContent.Add(bytes, "file", file.FileName);

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    var response = await client.PostAsync(posturl, multiContent);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> data = response.Content.ReadAsStringAsync();
                        JObject rss = JObject.Parse(data.Result.ToString());
                        return JsonConvert.DeserializeObject<ResponseClient>(data.Result.ToString());
                    }
                    return ReturnResponce.FailedMessageResponce(posturl);
                }
            }
            catch
            {
                throw;
            }
        }




        public  TokenValue Login(HttpRequestModel reqModel)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress =new Uri(reqModel.BaseApi);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    LoginUserHttp loginUser = new LoginUserHttp();
                    loginUser.userid = reqModel.UserName;
                    loginUser.userpass = reqModel.Password;
                    var response = client.PostAsJsonAsync("api/login", loginUser).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> data = response.Content.ReadAsStringAsync();
                        JObject rss = JObject.Parse(data.Result.ToString());
                        dynamic httpreqdata = JObject.Parse(rss.ToString());
                        TokenValue tValue = new TokenValue();
                        tValue.ExpiresIn = httpreqdata.expires_in;
                        tValue.ExpireTime = httpreqdata.expiretime;
                        tValue.Token = httpreqdata.access_token;
                        return tValue;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
