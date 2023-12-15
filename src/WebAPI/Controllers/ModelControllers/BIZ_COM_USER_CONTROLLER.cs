using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Entities;

namespace WebAPI.Controllers.ModelControllers
{

    [Route("edi/users")]
    public class BIZ_COM_USER_CONTROLLER : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<BIZ_COM_USER_CONTROLLER> _log;

        public BIZ_COM_USER_CONTROLLER(OracleDbContext context, ILogger<BIZ_COM_USER_CONTROLLER> log)
        {
            _dbContext = context;
            _log = log;
        }

        [Route("Userinfo")]
        [HttpPut]
      //  [Authorize(Policy = "EdiApiUser")]
      [AllowAnonymous]
        public IActionResult GetUserInfo([FromBody] int userid)
        {
            var currentUser = _dbContext.BIZ_COM_USER.Where(x => x.USERID == userid).SingleOrDefault();
            if (currentUser != null)
            {
                return Ok(currentUser);
            }
            else
            {
                return BadRequest();
            }
        }


        [Route("UpdateUserInfo")]
        [HttpPost]
        [AllowAnonymous]

        public IActionResult UpdateUserinfo([FromBody] object user)
        {

            dynamic request = JObject.Parse(user.ToString());
            int userid = request.userid;
            string fullname = request.fullname;
            string pic = request.pic;
            


            var originaluser = _dbContext.BIZ_COM_USER.Where(u => u.USERID == userid).SingleOrDefault();
            originaluser.FULLNAME = fullname;
            originaluser.PIC = pic;
            //originaluser.USERSTATUS = user.USERSTATUS;
            //originaluser.USERTYPE = user.USERTYPE;
            _dbContext.Entry(originaluser).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return Ok();

        }















    }
}
