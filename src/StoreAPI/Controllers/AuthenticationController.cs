using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StoreAPI.Helpers;
using StoreAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace StoreAPI.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost]
        [AllowAnonymous]
        [ApiExplorerSettings(GroupName = @"Нэвтрэх")]
        public async Task<IActionResult> Get([FromBody] AuthenticationUser user)
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
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };
            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            Response.Headers.Add("access_token", encodedJwt);
            Response.Headers.Add("expires_in", _jwtOptions.ValidFor.TotalSeconds.ToString());
            return Ok(response);
        }


        private Task<ClaimsIdentity> GetClaimsIdentity(AuthenticationUser user)
        {

            int userid = 0;
            try
            {
                string usermail = user.Email.ToLower();
                string password = Cryptography.Sha256Hash(user.Password);
                BIZ_COM_USER comuser = _dbContext.BIZ_COM_USER.FirstOrDefault(U => U.EMAIL.ToLower().Equals(usermail) && U.PASSWORD.Equals(password));
                if (comuser == null)
                {
                    return Task.FromResult<ClaimsIdentity>(null);
                }
                else
                {
                    return Task.FromResult(new ClaimsIdentity(
                    new GenericIdentity(GenericString(comuser.EMAIL, comuser.USERID, comuser.COMID), "Token"),
                    new[]
                    {
                          new Claim("BICharacter", "IAmMapi")
                    }));

                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<ClaimsIdentity>(null);
            }


        }

        private static long ToUnixEpochDate(DateTime date)
  => (long)Math.Round((date.ToUniversalTime() -
                       new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                      .TotalSeconds);


        string GenericString(string usermail, decimal userid, decimal companyid)
        {
            List<GenericData> data = new List<GenericData>
            {
                new GenericData  { UserMail = usermail, UserId =Convert.ToInt32(userid), CompanyId = Convert.ToInt32(companyid)   }
            };
            return JsonConvert.SerializeObject(data);

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
    }
}
