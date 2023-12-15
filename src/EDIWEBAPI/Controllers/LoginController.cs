using EDIWEBAPI.Context;
using EDIWEBAPI.Controllers.SendData;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        #endregion

        #region Initialize
        public LoginController(OracleDbContext context, IOptions<JwtIssuerOptions> jwtOptions, ILoggerFactory loggerFactory)
        {
            _dbContext = context;
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
            _logger = loggerFactory.CreateLogger<LoginController>();
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

        }

        private bool isLocal(string id) => id.StartsWith("10.0") || id.StartsWith("127.0") || id.StartsWith("::");

        private string GetLocalAddress()
        {
            try
            {
                var address = ((Microsoft.AspNetCore.Http.Internal.DefaultConnectionInfo)HttpContext.Connection).LocalIpAddress;
                return address == null ? string.Empty : address.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"LoginController.GetLocalAddress : {ex.Message}");
                return string.Empty;
            }
        }

        private async Task<SYSTEM_LOGIN_REQUEST> RequestApprove(string ip, string username, bool failed)
        {
            try
            {
                var user_data = ((Microsoft.AspNetCore.Server.Kestrel.Internal.Http.FrameRequestHeaders)HttpContext.Request.Headers).HeaderUserAgent.ToString();
                var ua = new Utils.UserAgent.UserAgent(user_data);

                var req = new SYSTEM_LOGIN_REQUEST()
                {
                    REQUESTDATE = DateTime.Now,
                    USERNAME = username,
                    IPADDRESS = ip,
                    LOCALADDRESS = GetLocalAddress(),
                    OSNAME = ua.OS.Name,
                    OSVERSION = ua.OS.Version,
                    BROWSERNAME = ua.Browser.Name,
                    BROWSERVERSION = ua.Browser.Version
                };

                if (isLocal(ip))
                {
                    req.FAILDETAIL = "LOCAL";
                }
                else
                {
                    var client = new HttpClient()
                    {
                        BaseAddress = new Uri("http://ip-api.com/"),
                    };
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.GetAsync($"json/{ip}").Result;
                    if (!response.IsSuccessStatusCode)
                        return null;

                    Task<string> data = response.Content.ReadAsStringAsync();
                    JObject rss = JObject.Parse(data.Result.ToString());
                    JObject json = JObject.Parse(rss.ToString());
                    if (Convert.ToString(json["status"]) == "success")
                    {

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
                        req.FAILDETAIL = failed ? "-1" : "";
                    }
                    else
                    {
                        req.FAILDETAIL = Convert.ToString(json);
                    }
                }

                req.ID = Convert.ToDecimal(Logics.BaseLogic.GetNewId(_dbContext, "SYSTEM_LOGIN_REQUEST"));
                Logics.BaseLogic.Insert(_dbContext, req);
                return req;
            }
            catch (Exception ex)
            {
                _logger.LogError($"LoginController.RequestApprove : {UsefulHelpers.GetExceptionMessage(ex)}");
                return null;
            }
        }

        #endregion


        /// <summary>
        ///	#Нууц үгээ мартсан
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="email"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("forgotpassword/{email}")]
        public async Task<ResponseClient> ForgotPassword(string email)
        {
            try
            {
                var currentuser = Logics.ManagementLogic.GetUser(_dbContext, email);
                if (currentuser == null)
                    return ReturnResponce.NotRegisteredEmail();

                string pass = Cryptography.CreatePassword();
                currentuser.USERPASSWORD = Cryptography.Sha256Hash(pass);

                Logics.ManagementLogic.Update(_dbContext, currentuser);

                //return await controller.Post(currentuser.USERMAIL, pass, SystemSendMailType.НууцҮгМартсан, currentuser.USERMAIL);
                return Emailer.Send(_dbContext, _logger, currentuser.USERMAIL, pass, MessageType.ForgotPassword,
                    currentuser.USERMAIL, currentuser.USERMAIL);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public class CookieData
        {
            public string Email { get; set; }
            public string LastLogDate { get; set; }
            public string TraceIdentifier { get; set; }
        }

        private CookieData GetCookieData(string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<CookieData>(data);
            }
            catch 
            {
                return null;
            }
        }

        private bool CheckBlocked(SYSTEM_LOGIN_REQUEST request)
        {
            if (isLocal(request.IPADDRESS)) return false;
            var logMethod = "LoginController.CheckBlocked";
            CookieData cookie = null;
            SYSTEM_USER_DEVICE currentDevice = null;
            try
            {
                if (HasCookie(request.USERNAME))
                    cookie = GetCookieData(GetCookie(request.USERNAME));

                if (cookie == null)
                {
                    cookie = new CookieData()
                    {
                        Email = request.USERNAME,
                        TraceIdentifier = HttpContext.TraceIdentifier
                    };
                }
                cookie.LastLogDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                SetCookie(request.USERNAME, JsonConvert.SerializeObject(cookie), 14);

                var blocked = false;
                var deviceID = decimal.Zero;
                var devices = Logics.ManagementLogic.GetUserDevices(_dbContext, _logger, request.USERNAME);

                if (devices != null && devices.Any())
                {
                    // TRACEIDENTIFIER - ААР ОЛОХ ГЭЖ ОРОЛДОНО
                    currentDevice = devices.FirstOrDefault(d => d.TRACEIDENTIFIER == cookie.TraceIdentifier);
                    _logger.LogInformation($"{logMethod} : FOUND TRACE : {currentDevice != null}");
                    // ХЭРВЭЭ ОЛДОХГҮЙ БОЛ COOKIE УСТГАСАН ЭСВЭЛ ХУГАЦАА ДУУССАН ГЭЖ ҮЗЭЭД ОЛОХ ГЭЖ ОРОЛДОНО
                    if (currentDevice == null)
                        currentDevice = devices.FirstOrDefault(d => d.IPADDRESS == request.IPADDRESS && d.LOCALADDRESS == request.LOCALADDRESS
                            && d.OSNAME == request.OSNAME.ToLower() && d.BROWSERNAME == request.BROWSERNAME.ToLower());
                    _logger.LogInformation($"{logMethod} : AFTER TRACE : {currentDevice != null}");
                }

                if (currentDevice == null)
                {
                    var newDevice = new SYSTEM_USER_DEVICE()
                    {
                        ID = Logics.BaseLogic.GetNewId(_dbContext, "SYSTEM_USER_DEVICE"),
                        USERMAIL = request.USERNAME.ToLower(),
                        IPADDRESS = request.IPADDRESS,
                        LOCALADDRESS = request.LOCALADDRESS,
                        TRACEIDENTIFIER = cookie.TraceIdentifier,
                        OSNAME = request.OSNAME.ToLower(),
                        BROWSERNAME = request.BROWSERNAME.ToLower(),
                        LASTREQUESTID = request.ID,
                        LASTLOGDATE = DateTime.Now,
                        BLOCKED = 0,
                        MAILEXPIREDATE = DateTime.Now.AddDays(7),
                        WARN = 0
                    };

                    _logger.LogInformation($"{logMethod} : INSERT DEVICE");
                    Logics.BaseLogic.Insert(_dbContext, newDevice);
                    deviceID = devices.Any() ? newDevice.ID : 0;
                }
                else
                {
                    blocked = currentDevice.BLOCKED == 1;
                    if (!blocked)
                    {
                        if (currentDevice.WARN == 1)
                        {
                            deviceID = currentDevice.ID;
                            currentDevice.MAILEXPIREDATE = DateTime.Now.AddDays(7);
                        }
                        currentDevice.OSNAME = request.OSNAME.ToLower();
                        currentDevice.BROWSERNAME = request.BROWSERNAME.ToLower();
                        currentDevice.LOCALADDRESS = request.LOCALADDRESS;
                        currentDevice.TRACEIDENTIFIER = cookie.TraceIdentifier;
                        currentDevice.LASTREQUESTID = request.ID;
                        currentDevice.LASTLOGDATE = DateTime.Now;

                        _logger.LogInformation($"{logMethod} : INSERT DEVICE");
                        Logics.BaseLogic.Update(_dbContext, currentDevice);
                    }
                }

                if (deviceID != 0 && !isLocal(request.IPADDRESS))
                {
                    var url = ConfigData.GetCongifData(ConfigData.ConfigKey.DisableDeviceURL)
                        .Replace("#id", deviceID.ToString())
                        .Replace("#email", request.USERNAME);

                    var content = SystemResource.MailResource.NewDevice
                        .Replace("#Name", request.USERNAME)
                        .Replace("#LinkYes", url.Replace("#method", "POST"))
                        .Replace("#LinkNotSure", url.Replace("#method", "PUT"))
                        .Replace("#LinkNo", url.Replace("#method", "DELETE"))
                        .Replace("#When", request.REQUESTDATE.Value.ToString("yyyy-MM-dd HH:mm"))
                        .Replace("#Where", request.IPADDRESS)
                        .Replace("#What", $"{request.OSNAME} {request.OSVERSION} {request.BROWSERNAME}");

                    // $"{request.COUNTRY}, {request.CITY}, {request.REGIONNAME}"

                    var emailMessage = new EmailMessage()
                    {
                        Type = MessageType.OtherDevice,
                        ToAddress = request.USERNAME,
                        FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL,
                        Subject = "Таны имэйл хаяг, нууц үгээр өөр төхөөрөмжөөс нэвтэрч орсон байна.",
                        Content = content,
                        Priority = 10,
                        StoreId = 0,
                        UserEmail = EmailConfiguration.URTO_OPERATOR_MAIL
                    };

                    _logger.LogInformation($"{logMethod} : SEND EMAIL");
                    Logics.MailLogic.Add(_dbContext, emailMessage);
                }
                
                return blocked;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{logMethod} ERROR : {UsefulHelpers.GetExceptionMessage(ex)}");
                return false;
            }
        }

        private bool HasCookie(string key)
        {
            return HttpContext.Request.Cookies.ContainsKey(key);
        }

        private string GetCookie(string key)
        {
            return HttpContext.Request.Cookies.ContainsKey(key) ?
                HttpContext.Request.Cookies[key] : string.Empty;
        }

        private void SetCookie(string key, string value, int days)
        {
            var option = new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(days)
                //Expires = DateTime.Now.AddSeconds(20)
            };
            Response.Cookies.Append(key, value, option);
        }


        #region POST
        [HttpPost]
        [AllowAnonymous]
        [ApiExplorerSettings(GroupName = @"Нэвтрэх")]
        public async Task<IActionResult> Login([FromBody] LoginUser user)
        {
            var logMethod = "LoginController.Login";
            string values = ConfigData.GetCongifData(ConfigData.ConfigKey.FileServerURL);
            SYSTEM_USERS currentUser = null;
            SYSTEM_ORGANIZATION userCompany = null;
            try
            {
                //var currentdata = _dbContext.SYSTEM_USERS.ToList();
                currentUser = Logics.ManagementLogic.GetUser(_dbContext, user.Email);

                if (currentUser == null)
                    //return BadRequest("No such email is registered yet.");
                    return BadRequest("600");

                if (currentUser.ENABLED == ENABLED.Идэвхигүй)
                    //return BadRequest("Account is disabled.");
                    return BadRequest("601");

                string password = Cryptography.Sha256Hash(user.Password);
                if (currentUser.USERPASSWORD == null || !currentUser.USERPASSWORD.Equals(password))
                    //return BadRequest("Password not match.");
                    return BadRequest("602");

                userCompany = _dbContext.SYSTEM_ORGANIZATION.FirstOrDefault(c => c.ID.Equals(currentUser.ORGID));
                if (userCompany == null)
                    //return BadRequest("Organization not found.");
                    return BadRequest("603");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{logMethod} ERROR : {UsefulHelpers.GetExceptionMessage(ex)}");
                //return BadRequest("Баазтай холбогдох үед алдаа гарлаа!");
                return BadRequest("604");
            }
            
            string currentIP = HttpContext.GetClientIPAddress();            

            var identity = await GetClaimsIdentity(currentUser, userCompany);
            var request = await RequestApprove(currentIP, user.Email, identity == null);

            if (identity == null)
            {
                _logger.LogInformation($"Invalid username ({user.Email}) or password ({user.Email})");
                //return BadRequest("Invalid credentials");
                return BadRequest("605");
            }

            if (request != null && CheckBlocked(request))
                //return BadRequest("Your device can not access this account.");
                return BadRequest("606");

            // ХЭРВЭЭ БИЗНЕСИЙНХАН ТУХАЙН САРДАА АНХ ЛОГИН ХИЙЖ БАЙВАЛ ТӨЛБӨРӨӨ БОДНО
            if (userCompany.ORGTYPE == ORGTYPE.Бизнес)
            {
                // ХЭРВЭЭ АНХ УДАА ЛОГИН ХИЙЖ БАЙВАЛ ЛОГИН ХИЙСНИЙ ДАРАА САРЫН ХУРААМЖ АВАХ ТАЛААР АСУУНА.
                if (Logics.LicenseLogic.GetLastLicense(_dbContext, currentUser.ID) != null)
                    Logics.LicenseLogic.CheckMonthFees(_dbContext, _logger, currentUser.ORGID, currentUser.ID, DateTime.Today);
            }
            else if (userCompany.ORGTYPE == ORGTYPE.Дэлгүүр)
            {
                var contract = Logics.LicenseLogic.GetContract(_dbContext, userCompany.ID, DateTime.Today);
                if (contract == null)
                    return BadRequest("607");
            }

            _jwtOptions.IssuedAt = DateTime.UtcNow;
            _jwtOptions.NotBefore = DateTime.UtcNow;
            var claimcharacter = identity.Claims.ToList()[1].Type;
            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, identity.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                              ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                              ClaimValueTypes.Integer64),
                    identity.FindFirst(claimcharacter) };

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.Add(_jwtOptions.ValidFor),
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds,
                usertype = ((SystemUserTypes)userCompany.ORGTYPE).ToString(),
                currentUser.FIRSTNAME,
                currentUser.USERMAIL,
                currentUser.USERPIC,
                userCompany.COMPANYNAME,
                userCompany.REGNO,
                userCompany.LOGO,
                userCompany.ID,
                currentUser.ENABLED,
                currentUser.PHONE,
                currentUser.LASTNAME,
                currentUser.REGDATE,
                userid = currentUser.ID,
                roleid = currentUser.ROLEID,
                isagreement = currentUser.ISAGREEMENT,
                expiretime = _jwtOptions.Expiration.ToString("yyyy-MM-dd HH:m:s"),
                showordersnotifstatus = OrderMobileNotifStatus(userCompany)               
            };
            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            Response.Headers.Add("access_token", encodedJwt);
            Response.Headers.Add("expires_in", _jwtOptions.ValidFor.TotalSeconds.ToString());

            return Ok(response);
        }
        #endregion

        #region Methods
        int OrderMobileNotifStatus(SYSTEM_ORGANIZATION organization)
        {
            if (organization.ORGTYPE != ORGTYPE.Бизнес)
                return 0;

            var contracts = Logics.ContractLogic.GetContracts(_dbContext, _logger, organization.ID)
                .Select(x => x.CONTRACTNO).ToArray();
            var currentdata = _dbContext.MST_ORDER_MOBILECONFIG.Count(x => contracts.Contains(x.CONTRACTNO));

            return currentdata > 0 ? 0 : 1;            
        }
        


        private Task<ClaimsIdentity> GetClaimsIdentity(SYSTEM_USERS user, SYSTEM_ORGANIZATION organization)
        {
            try
            {
                bool isHeaderCompanyUser = organization.PARENTID != null;
                var usertype = (SystemUserTypes)organization.ORGTYPE;
                if (usertype == SystemUserTypes.Business)
                {
                    string encrypteddata = EncryptionUtils.Encrypt(
                        GenericString(user.USERMAIL, user.ID, user.ORGID, organization.REGNO, 1, isHeaderCompanyUser, organization.COMPANYNAME, user.ROLEID, user.ISAGREEMENT),
                        EncryptionUtils.ENCRYPTION_KEY);

                    return Task.FromResult(new ClaimsIdentity(
                        new GenericIdentity(encrypteddata, "Token"),
                        new[]
                        {
                               new Claim("BizApiCharacter", "IAmBizapi")
                        }));
                }
                else if (usertype == SystemUserTypes.Store)
                {
                    string encrypteddata = EncryptionUtils.Encrypt(
                        GenericString(user.USERMAIL, user.ID, user.ORGID, organization.REGNO, 2, isHeaderCompanyUser, organization.COMPANYNAME, user.ROLEID, null),
                        EncryptionUtils.ENCRYPTION_KEY);

                    return Task.FromResult(new ClaimsIdentity(
                        new GenericIdentity(encrypteddata, "Token"),
                        new[]
                        {
                               new Claim("StoreApiCharacter", "IAmStoreapi")
                        }));
                }
                else
                {
                    string encrypteddata = EncryptionUtils.Encrypt(
                        GenericString(user.USERMAIL, user.ID, user.ORGID, organization.REGNO, 3, isHeaderCompanyUser, organization.COMPANYNAME, user.ROLEID, null),
                        EncryptionUtils.ENCRYPTION_KEY);

                    return Task.FromResult(new ClaimsIdentity(
                        new GenericIdentity(encrypteddata, "Token"),
                        new[]
                        {
                              new Claim("EdiCharacter", "IAmMapi")
                        }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"LoginController.GetClaimsIdentity ERROR : {UsefulHelpers.GetExceptionMessage(ex)}");
                return Task.FromResult<ClaimsIdentity>(null);
            }
        }



        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }


        string GenericString(string usermail, decimal userid, decimal companyid, string companyregno, int orgtype, bool isHeaderCompanyUser, string companyName, int? roleid, int? agreement)
        {
            List<GenericData> data = new List<GenericData>
            {
                new GenericData  { UserMail = usermail,  UserId =Convert.ToInt32(userid), CompanyId = Convert.ToInt32(companyid), ComRegNo = companyregno, OrgType = Convert.ToInt32(orgtype), isHeaderCompanyUser = isHeaderCompanyUser, CompanyName  = companyName, Isagreement = agreement, Roleid = roleid}
            };
            return JsonConvert.SerializeObject(data);

        }


        private static long ToUnixEpochDate(DateTime date)
  => (long)Math.Round((date.ToUniversalTime() -
                       new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                      .TotalSeconds);
        #endregion


    }
}
