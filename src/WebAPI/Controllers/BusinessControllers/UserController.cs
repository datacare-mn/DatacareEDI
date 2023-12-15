using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAPI.Controllers.AttachmentControllers;
using WebAPI.Controllers.SystemControllers;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.CustomModels;
using WebAPI.Models.Entities;
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Controllers.BusinessControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class UserController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<UserController> _log;

        public UserController(OracleDbContext context, ILogger<UserController> log)
        {
            _dbContext = context;
            _log = log;
        }

        /// <summary>
        /// Дамжуулсан хэрэглэгчийн мэдээлэийг буцаана
        /// </summary>
        /// <param name="id">USERID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public ResponseClient GetUser(int id)
        {
            try
            {
                ResponseClient response = new ResponseClient();
                var currentUser = _dbContext.BIZ_COM_USER.Where(x => x.USERID == id).SingleOrDefault();
                if (currentUser != null)
                {
                    return ReturnResponce.ListReturnResponce(currentUser);
                }
                else
                {
                    return ReturnResponce.NotFoundResponce();
                }
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        /// Компанийн бүх хэрэглэгчид
        /// </summary>
        /// <param name="comid">Компанийн ID </param>
        /// <returns></returns>
        [HttpGet]
        [Route("AllUsers")]
        //[Authorize(Policy = "StoreApiUser")]
        //[Authorize(Policy = "BizApiUser")]
        [Authorize]
        public ResponseClient GetUserAllUsers()
        {
            try
            {
                // var currentUsers = _dbContext.BIZ_COM_USER.Where(x => x.COMID == comid).ToList();
                var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));


                var join = from u in _dbContext.BIZ_COM_USER
                           join c in _dbContext.BIZ_COMPANY on u.COMID equals c.COMID
                           select new
                           {
                               u.USERID,
                               u.USERSTATUS,
                               u.USERTYPE,
                               u.PIC,
                               u.FULLNAME,
                               u.EMAIL,
                               u.BIZ_COMPANY
                               
                           };
                var currentUsers = join.ToList();
                if (currentUsers.Count > 0)
                {
                    return ReturnResponce.ListReturnResponce(currentUsers);
                }
                else
                {
                    return ReturnResponce.NotFoundResponce();
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        /// Хэрэглэгчийн мэдээлэл засах
        /// </summary>
        /// <param name="user"></param>
        /// <returns>responce</returns>
        [HttpPut]
        [Authorize]
        public async Task  <ResponseClient> UpdateUser([FromBody]BIZ_COM_USER user)
        {

            try
            {
                var currentUser = _dbContext.BIZ_COM_USER.SingleOrDefault(x => x.USERID == user.USERID);
                if (currentUser != null)
                {
                    currentUser.USERSTATUS = user.USERSTATUS;
                    currentUser.USERTYPE = user.USERTYPE;
                    currentUser.EMAIL = user.EMAIL;
                    currentUser.FULLNAME = user.FULLNAME;
                    currentUser.PASSWORD = user.PASSWORD;
                    currentUser.PIC = user.PIC;
                    _dbContext.SaveChanges(HttpContext, 1);
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                {
                    return ReturnResponce.NotFoundResponce();
                }

            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        /// Шинэ хэрэглэгч бүртгэх
        /// </summary>
        /// <param name="user">хэрэглэгчийн мэдээлэл</param>
        /// <returns></returns>

        [HttpPost]
        [Authorize]
        public async Task<ResponseClient> NewUser([FromBody] BIZ_COM_USER user)
        {
            try
            {
                user.USERID =Convert.ToInt32(_dbContext.GetTableID("BIZ_COM_USER"));
                var email = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserMail));
                if (ModelState.IsValid)
                {
                    var otheruser = _dbContext.BIZ_COM_USER.FirstOrDefault(x => x.EMAIL == user.EMAIL);
                    if (otheruser == null)
                    {
                        string pass = Cryptography.CreatePassword();
                        user.PASSWORD = Cryptography.Sha256Hash(pass);
                        _dbContext.BIZ_COM_USER.Add(user);
                        _dbContext.SaveChanges(HttpContext, 1);
                        MailSendController mc = new MailSendController(_dbContext, null);
                        await mc.Post(user.EMAIL, $"Нэвтрэх нэр : <b>{user.EMAIL}</b><p></p>Нууц үг : <b>{pass}</b><p></p>");
                        return ReturnResponce.SaveSucessResponce();
                    }
                    else
                    {
                        return ReturnResponce.FailedMessageResponce(ReturnMessageHelper.MAILALREADYREGISTERED);
                    }
                }
                else
                {
                    return ReturnResponce.SaveFailureResponce();
                }

            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [Route("CheckMailAddress/{email}")]
        [HttpPost]
        [AllowAnonymous]

        public async Task<ResponseClient> CheckMailAddress(string email)
        {
            var currentUser = _dbContext.BIZ_COM_USER.FirstOrDefault(x => x.EMAIL.ToLower() == email.ToLower());
            if (currentUser == null)
            {
                return ReturnResponce.SuccessMessageResponce("И-мэйл хаяг боломжтой!");
            }
            else
            {
                return ReturnResponce.FailedMessageResponce("И-мэйл хаяг бүртгэгдсэн байна.");
            }
        }


        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        public async Task<ResponseClient> ChangePassword([FromBody] ChangePasswordUser user)
        {
            if (ModelState.IsValid)
            {
                string pass = Cryptography.Sha256Hash(user.OldPassword);
                string newpassword = Cryptography.Sha256Hash(user.NewPassword);
                var currUser = _dbContext.BIZ_COM_USER.FirstOrDefault(x => x.EMAIL.ToLower() == user.Email.ToLower());
                if (currUser != null)
                {
                    var currentUser = _dbContext.BIZ_COM_USER.FirstOrDefault(x => x.EMAIL.ToLower() == user.Email.ToLower() && x.PASSWORD == pass);
                    if (currentUser != null)
                    {
                        currentUser.PASSWORD = newpassword;
                        _dbContext.SaveChanges(HttpContext, 1);
                        return ReturnResponce.SaveSucessResponce();
                    }
                    else
                    {
                        return ReturnResponce.FailedMessageResponce("Таны хуучин нууц үг буруу байна...");
                    }
                }
                else
                {
                    return ReturnResponce.FailedMessageResponce(ReturnMessageHelper.MAILNOTREGISTERED);
                }


            }
            else
            {
                return ReturnResponce.ModelIsNotValudResponce();
            }
        }

        [Route("ResetPassword/{email}")]
        [HttpPost]
        [AllowAnonymous]

        public async Task<ResponseClient> ResetPassword(string email)
        {
            ResponseClient response = new ResponseClient();
            if (ModelState.IsValid)
            {

                var currentUser = _dbContext.BIZ_COM_USER.FirstOrDefault(x => x.EMAIL == email);
                if (currentUser != null)
                {
                    string pass = Cryptography.CreatePassword();
                    currentUser.PASSWORD = Cryptography.Sha256Hash(pass);
                    _dbContext.SaveChanges(HttpContext, 1);
                    MailSendController mc = new MailSendController(_dbContext, null);
                    await mc.Post(currentUser.EMAIL, $"Нэвтрэх нэр : <b>{currentUser.EMAIL}</b><p></p>Нууц үг : <b>{pass}</b><p></p>");
                    return ReturnResponce.SuccessMessageResponce("Нууц үгийг илгээлээ!");
                }
                else
                {
                    return ReturnResponce.FailedMessageResponce(ReturnMessageHelper.MAILNOTREGISTERED);
                }

            }
            else
            {
                return ReturnResponce.ModelIsNotValudResponce();
            }



        }

    }
}
