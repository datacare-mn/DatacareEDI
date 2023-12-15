using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.CustomModels;
using WebAPI.Models.Entities;
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Controllers.SystemControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/login")]
    public class AuthenticationController : Controller
    {
        private readonly OracleDbContext _dbContext;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;


        public AuthenticationController(OracleDbContext context, IOptions<JwtIssuerOptions> jwtOptions, ILoggerFactory loggerFactory)
        {
            _dbContext = context;
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = loggerFactory.CreateLogger<AuthenticationController>();

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
        /// <summary>
        /// Нэвтрэх
        /// </summary>
        /// <remarks>
        /// Системд нэвтрэх токен авах
        ///  
        ///     GET /USERINFO
        ///     {
        ///        "email": "user@mail.com",
        ///        "password": "*************"
        ///     }
        ///     
        ///         
        /// 
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>Токен утга</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        /// <re>

        [HttpPost]
        [AllowAnonymous]
        [ApiExplorerSettings(GroupName = @"Нэвтрэх")]
        public async Task<IActionResult> Get([FromBody] LoginUser user)
        {
            var identity = await GetClaimsIdentity(user);
            if (identity == null)
            {
                _logger.LogInformation($"Invalid username ({user.Email}) or password ({user.Email})");
                return BadRequest("Invalid credentials");
            }

            string usermail = user.Email.ToLower();
            string password = Cryptography.Sha256Hash(user.Password);
            BIZ_COM_USER comuser = _dbContext.BIZ_COM_USER.FirstOrDefault(U => U.EMAIL.ToLower().Equals(usermail) && U.PASSWORD.Equals(password));
            var userCompany = _dbContext.BIZ_COMPANY.FirstOrDefault(c => c.COMID.Equals(comuser.COMID));
            SystemUserTypes usertype = (SystemUserTypes)userCompany.COMTYPE;

            var claimcharacter = identity.Claims.ToList()[1].Type;
            var claims = new[]
                        {
                    new Claim(JwtRegisteredClaimNames.Sub, identity.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                              ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                              ClaimValueTypes.Integer64),
                    identity.FindFirst(claimcharacter)
                  };
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds,
                usertype = usertype.ToString()
            };
            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            Response.Headers.Add("access_token", encodedJwt);
            Response.Headers.Add("expires_in", _jwtOptions.ValidFor.TotalSeconds.ToString());
            return Ok(response);
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

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private Task<ClaimsIdentity> GetClaimsIdentity(LoginUser user)
        {

            int userid = 0;
            try
            {
                string usermail = user.Email.ToLower();
                string password =Cryptography.Sha256Hash(user.Password);
                BIZ_COM_USER comuser = _dbContext.BIZ_COM_USER.FirstOrDefault(U => U.EMAIL.ToLower().Equals(usermail) && U.PASSWORD.Equals(password));
                SystemUserTypes usertype;

                if (comuser == null)
                {
                    return Task.FromResult<ClaimsIdentity>(null);
                }
                else
                {

                    var userCompany = _dbContext.BIZ_COMPANY.FirstOrDefault(c => c.COMID.Equals(comuser.COMID));
                    usertype = (SystemUserTypes)userCompany.COMTYPE;
                    if (usertype == SystemUserTypes.Business)
                    {
                        return Task.FromResult(new ClaimsIdentity(
                        new GenericIdentity(GenericString(comuser.EMAIL, comuser.USERID, comuser.COMID), "Token"),
                        new[]
                        {
                           new Claim("BizApiCharacter", "IAmBizapi")
                        }));
                    }
                    else if (usertype == SystemUserTypes.Store)
                    {
                        return Task.FromResult(new ClaimsIdentity(
                        new GenericIdentity(GenericString(comuser.EMAIL, comuser.USERID, comuser.COMID), "Token"),
                        new[]
                        {
                           new Claim("StoreApiCharacter", "IAmStoreapi")
                        }));
                    }
                    else
                    {
                        return Task.FromResult(new ClaimsIdentity(
                        new GenericIdentity(GenericString(comuser.EMAIL, comuser.USERID, comuser.COMID), "Token"),
                        new[]
                        {
                          new Claim("EdiCharacter", "IAmMapi")
                        }));


                    }

                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<ClaimsIdentity>(null);
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usermail"></param>
        /// <param name="userid"></param>
        /// <param name="companyid"></param>
        /// <returns></returns>

        string GenericString(string usermail, decimal userid, decimal companyid)
        {
            List<GenericData> data = new List<GenericData>
            {
                new GenericData  { UserMail = usermail, UserId =Convert.ToInt32(userid), CompanyId = Convert.ToInt32(companyid)   }
            };
            return JsonConvert.SerializeObject(data);

        }
    }
}
