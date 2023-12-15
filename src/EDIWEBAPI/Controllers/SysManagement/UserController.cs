using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

using static EDIWEBAPI.Enums.SystemEnums;
using EDIWEBAPI.Entities.ResultModels;
using EDIWEBAPI.Entities.FilterViews;
using System.Threading.Tasks;
using EDIWEBAPI.Controllers.SendData;
using EDIWEBAPI.Controllers.Storeapi;
using EDIWEBAPI.Attributes;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using EDIWEBAPI.Entities.CustomModels;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<UserController> _log;

        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public UserController(OracleDbContext context, ILogger<UserController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        #region Get


        /// <summary>
        ///	#Хэрэглэгчийн хувийн мэдээлэл
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-22
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> Get()
        {
            int userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            var currentuser = Logics.ManagementLogic.GetUser(_dbContext, userid);
            if (currentuser == null)
                return ReturnResponce.NotFoundResponce();

            //var role = currentuser.ROLEID.HasValue ? Logics.ManagementLogic.GetRole(_dbContext, currentuser.ROLEID.Value) : null;
            var userDto = new
            {
                ID = currentuser.ID,
                USERMAIL = currentuser.USERMAIL,
                USERPASSWORD = currentuser.USERPASSWORD,
                USERPIC = currentuser.USERPIC,
                LASTNAME = currentuser.LASTNAME,
                FIRSTNAME = currentuser.FIRSTNAME,
                REGDATE = currentuser.REGDATE,
                PHONE = currentuser.PHONE,
                ENABLED = currentuser.ENABLED,
                ROLEID = currentuser.ROLEID,
                currentuser.COOPERATION,
                ORGID = currentuser.ORGID,
                ISADMIN = currentuser.ISADMIN,
                ROLECHANGEDATE = currentuser.ROLECHANGEDATE,
                ISAGREEMENT = currentuser.ISAGREEMENT,
                AGREEMENTDATE = currentuser.AGREEMENTDATE,
                OLDROLEID = currentuser.OLDROLEID,
                SYSTEM_ORGANIZATION = currentuser.SYSTEM_ORGANIZATION
                //ROLENAME = role == null ? string.Empty : role.ROLENAME
            };
            return ReturnResponce.ListReturnResponce(userDto);
        }

        [HttpGet]
        [Authorize]
        [Route("getuser/{id}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetUser(int id)
        {
            try
            {
                var currentuser = Logics.ManagementLogic.GetUser(_dbContext, id);
                if (currentuser == null)
                    return ReturnResponce.NotFoundResponce();

                //var role = currentuser.ROLEID.HasValue ? Logics.ManagementLogic.GetRole(_dbContext, currentuser.ROLEID.Value) : null;
                var userDto = new UserDto
                {
                    ID = currentuser.ID,
                    USERMAIL = currentuser.USERMAIL,
                    USERPIC = currentuser.USERPIC,
                    LASTNAME = currentuser.LASTNAME,
                    FIRSTNAME = currentuser.FIRSTNAME,
                    REGDATE = currentuser.REGDATE,
                    PHONE = currentuser.PHONE,
                    ROLEID = currentuser.ROLEID,
                    COOPERATION = currentuser.COOPERATION
                    //ROLENAME = role == null ? string.Empty : role.ROLENAME
                };

                var organization = Logics.ManagementLogic.GetOrganization(_dbContext, currentuser.ORGID);
                if (organization.ORGTYPE == ORGTYPE.Дэлгүүр)
                {
                    userDto.departments = (from d in _dbContext.SYSTEM_USER_DEPARTMENT
                                           where d.USERID == id
                                           select d.DEPARTMENTID).ToList();

                    userDto.roles = (from r in _dbContext.SYSTEM_USER_ROLES
                                     where r.USERID == id
                                     select r.ROLEID).ToList();
                }

                return ReturnResponce.ListReturnResponce(userDto);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        /// <summary>
        /// Идэвхитэй хэрэглэгчдийн жагсаалт шүүлтүүртэй хуудаслалтай 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : Л.Идэр
        /// Үүсгэсэн огноо : 2017.11.10 
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 


        // Байгууллагаар нь хэрэглэгчдийн жагсаалтын шүүж авчрах API  
        [HttpGet]
        [Route("allusers")]
        [Authorize]
        public async Task<ResponseClient> GetAllUsers()
        {
            try
            {
                var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
                var userList = Logics.ManagementLogic.GetUsers(_dbContext, userid);
                return userList != null ? 
                    ReturnResponce.ListReturnResponce(userList) :
                    ReturnResponce.NotFoundResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpPost]
        [Route("getUsers")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetUsers([FromBody]UserFilterView filter)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            try
            {
                var userList = _dbContext.GET_USERS(filter);
                return userList != null ? 
                    ReturnResponce.ListReturnResponce(userList) :
                    ReturnResponce.NotFoundResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("getuserinfo/{id}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetUsersInfo(int id)
        {
            int comtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            var user = Logics.ManagementLogic.GetUser(_dbContext, id);
            if (user == null)
                return ReturnResponce.NotFoundResponce();

            //var comrolename = Logics.ManagementLogic.GetOrganizationRole(_dbContext, user.ROLEID.Value);
            var result = new
            {
                id = user.ID,
                lastname = user.LASTNAME,
                firstname = user.FIRSTNAME,
                email = user.USERMAIL
                //rolename = comrolename?.ROLENAME
            };
            return ReturnResponce.ListReturnResponce(result);
        }







        /// <summary>
        /// Байгууллагын хэрэглэгчийн жагсаалт идэвхитэй 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        ///    
        [HttpGet]
        [Route("getorganizationusers/{id}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetOrganizationUsers(int id) {
            var userList = _dbContext.ORGANIZATION_USERS_SELECT(id).ToList();

            if (userList == null)
                return ReturnResponce.NotFoundResponce();

            return ReturnResponce.ListReturnResponce(userList);
        }


        [HttpGet]
        [Route("getallusers//")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetAllUser()
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var currentusers = Logics.ManagementLogic.GetUsers(_dbContext, comid, Enums.SystemEnums.ENABLED.Идэвхитэй);
            return currentusers != null ? 
                ReturnResponce.ListReturnResponce(currentusers) :
                ReturnResponce.NotFoundResponce();
        }


        /// <summary>
        /// Байгууллагын хэрэглэгчийн жагсаалт идэвхигүй 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpGet]
        [Route("getalldiabledusers")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetAllDiabledUsers()
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var currentusers = Logics.ManagementLogic.GetUsers(_dbContext, comid, Enums.SystemEnums.ENABLED.Идэвхигүй);
            return currentusers != null ?
                ReturnResponce.ListReturnResponce(currentusers) :
                ReturnResponce.NotFoundResponce();
        }

        [HttpGet]
        [Route("getchangedcount/{id}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetChangedCount(int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            if ((ORGTYPE)Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType)) != ORGTYPE.Бизнес)
                return ReturnResponce.SuccessMessageResponce("0");
            try
            {
                var month = DateTime.Today.ToString("MM");
                var count = _dbContext.SYSTEM_USER_STATUS_LOG
                    .Count(s => s.USERID == id && s.LOGMONTH == month && s.LOGYEAR == DateTime.Today.Year);
                //.Count(s => s.USERID == id && s.LOGYEAR == DateTime.Today.Year && s.LOGMONTH == DateTime.Today.ToString("MM"));

                if (count >= 1)
                    return ReturnResponce.FailedMessageResponce(count.ToString());
                else
                    return ReturnResponce.SuccessMessageResponce(count.ToString());
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("getuserstatuslog/{id}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetUserStatusLog(int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var logs = from l in _dbContext.SYSTEM_USER_STATUS_LOG
                           join u in _dbContext.SYSTEM_USERS on l.LOGBY equals u.ID
                           where l.USERID == id
                           orderby l.LOGDATE descending
                           select new
                           {
                               ENABLED = l.ENABLED,
                               DESCRIPTION = l.ENABLED == 0 ? "Устгасан" : "Сэргээсэн",
                               USERNAME = u.FIRSTNAME,
                               LOGDATE = l.LOGDATE
                           };

                return ReturnResponce.ListReturnResponce(logs.ToList());
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        #endregion

        #region Post


        /// <summary>
        /// Байгууллагын хэрэглэгч бүртгэх
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-02
        /// Зассан огноо : 
        /// </remarks>
        /// <param name="param"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> Post([FromBody] UserDto param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            if (_dbContext.USER_EXISTS(param.USERMAIL))
                return ReturnResponce.FailedMessageResponce($"{param.USERMAIL} дээр хэрэглэгч бүртгэгдсэн байна...");

            //var orgid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var logMethod = "USERCONTROLLER.POST";
            try
            {
                string password = Cryptography.CreatePassword();
                param.ID = Convert.ToInt32(Logics.ManagementLogic.GetNewId(_dbContext, "SYSTEM_USERS"));

                var uData = new SYSTEM_USERS()
                {
                    ID = param.ID,
                    USERMAIL = param.USERMAIL,
                    USERPIC = param.USERPIC,
                    LASTNAME = param.LASTNAME,
                    FIRSTNAME = param.FIRSTNAME,
                    REGDATE = DateTime.Now,
                    ENABLED = ENABLED.Идэвхитэй,
                    ORGID = param.ORGID,
                    PHONE = param.PHONE,
                    ISADMIN = 0,
                    ROLEID = param.ROLEID,
                    COOPERATION = param.COOPERATION,
                    USERPASSWORD = Cryptography.Sha256Hash(password)
                };

                var organziation = Logics.ManagementLogic.GetOrganization(_dbContext, param.ORGID);

                Logics.BaseLogic.Insert(_dbContext, uData, false);
                if (organziation.ORGTYPE == ORGTYPE.Дэлгүүр)
                {
                    if (param.departments != null)
                    {
                        foreach (var departmentId in param.departments)
                        {
                            Logics.BaseLogic.Insert(_dbContext, new SYSTEM_USER_DEPARTMENT()
                            {
                                ID = Convert.ToInt16(Logics.BaseLogic.GetNewId(_dbContext, "SYSTEM_USER_DEPARTMENT")),
                                DEPARTMENTID = departmentId,
                                ORGID = param.ORGID,
                                USERID = uData.ID
                            }, false);
                        }
                    }

                    if (param.roles != null)
                    {
                        foreach (var roleId in param.roles)
                        {
                            Logics.BaseLogic.Insert(_dbContext, new SYSTEM_USER_ROLES()
                            {
                                ID = Convert.ToInt16(Logics.BaseLogic.GetNewId(_dbContext, "SYSTEM_USER_ROLES")),
                                ROLEID = roleId,
                                USERID = uData.ID
                            }, false);
                        }
                    }
                }

                _dbContext.SaveChanges();

                var mailValue = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserMail));
                var createEmail = string.IsNullOrEmpty(mailValue) ? param.USERMAIL : mailValue;
                _log.LogInformation($"{logMethod} MAILTO = {param.USERMAIL}");

                Emailer.Send(_dbContext, _log, param.USERMAIL, password, MessageType.NewUser,
                    param.USERMAIL, createEmail);

                return ReturnResponce.SaveSucessWithIdResponce(uData.ID);
            }
            catch (Exception ex)
            {
                _log.LogError($"{logMethod} : {param.USERMAIL} => {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }



        [HttpPost]
        [Authorize]
        [Route("changepassword/{newpassword}/{oldpassword}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> ChangeUserPassword(string newpassword, string oldpassword)
        {
            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            try
            {
                var currentuser = Logics.ManagementLogic.GetUser(_dbContext, userid);
                if (currentuser == null)
                    return ReturnResponce.FailedMessageResponce("Хэрэглэгч олдсонгүй!");

                string newpasswordhash = Cryptography.Sha256Hash(newpassword);
                string oldpasswordhash = Cryptography.Sha256Hash(oldpassword);

                if (currentuser.USERPASSWORD != oldpasswordhash)
                    return ReturnResponce.FailedMessageResponce("Хуучин нууц үг буруу байна.");

                currentuser.USERPASSWORD = newpasswordhash;
                Logics.ManagementLogic.Update(_dbContext, currentuser);

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        #endregion

        #region Put

        [HttpPut]
        [Route("updateuserprofile")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> UpdateUserProfile(IFormFile uploadedFile, string json)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var rss = Attacher.File(_log, uploadedFile, "attachedfiles");

                if (!rss.Success)
                    return rss;

                var user = JsonConvert.DeserializeObject<MultipartUser>(json.Replace("[", "").Replace("]", ""));

                var currentdata = Logics.ManagementLogic.GetUser(_dbContext, user.USERMAIL);
                if (currentdata == null)
                    return ReturnResponce.NotFoundResponce();

                if (currentdata.USERMAIL != user.USERMAIL && _dbContext.USER_EXISTS(user.USERMAIL))
                    return ReturnResponce.FailedMessageResponce($"{user.USERMAIL} дээр хэрэглэгч бүртгэгдсэн байна...");

                currentdata.USERPIC = Convert.ToString(rss.Value);
                currentdata.LASTNAME = user.LASTNAME;
                currentdata.FIRSTNAME = user.FIRSTNAME;
                currentdata.PHONE = user.PHONE;

                Logics.ManagementLogic.Update(_dbContext, currentdata);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }




        /// <summary>
        /// Хэрэглэгч мэдээлэлээ засах
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpPut]
        [Route("userprofile")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> UpdateUserProfile([FromBody]SYSTEM_USERS param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var currentdata = _dbContext.GET_USER(param.ID);
                if (currentdata == null)
                    return ReturnResponce.NotFoundResponce();

                if (currentdata.USERMAIL != param.USERMAIL && _dbContext.USER_EXISTS(param.USERMAIL))
                    return ReturnResponce.FailedMessageResponce($"{param.USERMAIL} дээр хэрэглэгч бүртгэгдсэн байна...");

                currentdata.USERPIC = param.USERPIC;
                currentdata.LASTNAME = param.LASTNAME;
                currentdata.FIRSTNAME = param.FIRSTNAME;
                currentdata.PHONE = param.PHONE;
                currentdata.COOPERATION = param.COOPERATION;

                Logics.ManagementLogic.Update(_dbContext, currentdata);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        ///	#Хэрэглэгчийн мэдээлэл засах
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> Put(IFormFile uploadedFile, string json)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var param = JsonConvert.DeserializeObject<UserDto>(json);

                var currentdata = _dbContext.GET_USER(param.ID);
                if (currentdata == null)
                    return ReturnResponce.NotFoundResponce();

                if (currentdata.USERMAIL != param.USERMAIL && _dbContext.USER_EXISTS(param.USERMAIL))
                    return ReturnResponce.FailedMessageResponce($"{param.USERMAIL} дээр хэрэглэгч бүртгэгдсэн байна...");

                if (uploadedFile != null)
                {
                    var rss = Attacher.File(_log, uploadedFile, "attachedfiles");

                    if (!rss.Success)
                        return rss;

                    currentdata.USERPIC = Convert.ToString(rss.Value);
                }
                
                currentdata.LASTNAME = param.LASTNAME;
                currentdata.FIRSTNAME = param.FIRSTNAME;
                currentdata.REGDATE = param.REGDATE;
                currentdata.PHONE = param.PHONE;
                currentdata.COOPERATION = param.COOPERATION;

                var organization = Logics.ManagementLogic.GetOrganization(_dbContext, currentdata.ORGID);
                if (organization.ORGTYPE == ORGTYPE.Дэлгүүр)
                {
                    // DEPARTMENT
                    if (param.departments == null)
                        param.departments = new List<int>();

                    var existing = (from d in _dbContext.SYSTEM_USER_DEPARTMENT
                                    where d.USERID == param.ID
                                    select d).ToList();

                    var mapping = new SortedDictionary<int, int>();
                    foreach (var current in existing)
                    {
                        if (!param.departments.Contains(current.DEPARTMENTID))
                            Logics.BaseLogic.Delete(_dbContext, current, false);

                        mapping.Add(current.DEPARTMENTID, current.DEPARTMENTID);
                    }

                    foreach (var departmentId in param.departments)
                    {
                        if (mapping.ContainsKey(departmentId)) continue;

                        Logics.BaseLogic.Insert(_dbContext, new SYSTEM_USER_DEPARTMENT()
                        {
                            ID = Convert.ToInt16(Logics.BaseLogic.GetNewId(_dbContext, "SYSTEM_USER_DEPARTMENT")),
                            DEPARTMENTID = departmentId,
                            ORGID = currentdata.ORGID,
                            USERID = currentdata.ID
                        }, false);
                    }

                    // ROLE
                    if (param.roles == null)
                        param.roles = new List<int>();

                    var existingRoles = (from d in _dbContext.SYSTEM_USER_ROLES
                                         where d.USERID == param.ID
                                         select d).ToList();

                    mapping.Clear();
                    foreach (var current in existingRoles)
                    {
                        if (!param.roles.Contains(current.ROLEID))
                            Logics.BaseLogic.Delete(_dbContext, current, false);

                        mapping.Add(current.ROLEID, current.ROLEID);
                    }

                    foreach (var roleId in param.roles)
                    {
                        if (mapping.ContainsKey(roleId)) continue;

                        Logics.BaseLogic.Insert(_dbContext, new SYSTEM_USER_ROLES()
                        {
                            ID = Convert.ToInt16(Logics.BaseLogic.GetNewId(_dbContext, "SYSTEM_USER_ROLES")),
                            ROLEID = roleId,
                            USERID = currentdata.ID
                        }, false);
                    }
                }

                Logics.BaseLogic.Update(_dbContext, currentdata, false);
                _dbContext.SaveChanges();

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError($"USERCONTROLLER.PUT : {ex.Message}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        /// <summary>
        ///	#Хэрэглэгч идэвхигүй болгох
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-01
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [Authorize]
        [HttpPut]
        [Route("delete/{ID}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> Delete(int ID)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var modifiedBy = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            return Logics.ManagementLogic.ChangeUserStatus(_dbContext, ID, ENABLED.Идэвхигүй, modifiedBy);
        }

        [Authorize]
        [HttpPut]
        [Route("restore/{ID}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> Restore(int ID)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            int modifiedBy = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            return Logics.ManagementLogic.ChangeUserStatus(_dbContext, ID, ENABLED.Идэвхитэй, modifiedBy);
        }

        #endregion

        #region Хэрэглэгч бүртгэл


        /// <summary>
        ///	#Тухайн нэвтэрсэн компанийн хэрэглэгч бүртгэх апи   
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-05-15
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpPost]
        [Authorize]
        [Route("createuser")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> PostCreateUser([FromBody]SYSTEM_USERS param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            if (_dbContext.USER_EXISTS(param.USERMAIL))
                return ReturnResponce.FailedMessageResponce($"{param.USERMAIL} дээр хэрэглэгч бүртгэгдсэн байна...");

            try
            {
                string password = Cryptography.CreatePassword();
                param.ID = Convert.ToInt32(Logics.ManagementLogic.GetNewId(_dbContext, "SYSTEM_USERS"));
                var orgid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));

                var uData = new SYSTEM_USERS()
                {
                    ID = param.ID,
                    USERMAIL = param.USERMAIL,
                    USERPIC = param.USERPIC,
                    LASTNAME = param.LASTNAME,
                    FIRSTNAME = param.FIRSTNAME,
                    REGDATE = param.REGDATE,
                    ENABLED = param.ENABLED,
                    ORGID = orgid,
                    PHONE = param.PHONE,
                    ISADMIN = param.ISADMIN,
                    ROLEID = param.ROLEID,
                    COOPERATION = param.COOPERATION,
                    USERPASSWORD = Cryptography.Sha256Hash(password)
                };

                Logics.ManagementLogic.Insert(_dbContext, uData);

                var mailValue = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserMail));
                var createEmail = string.IsNullOrEmpty(mailValue) ? param.USERMAIL : mailValue;

                Emailer.Send(_dbContext, _log, param.USERMAIL, password, MessageType.NewUser, param.USERMAIL, createEmail);

                return ReturnResponce.SaveSucessWithIdResponce(uData.ID);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpGet]
        [Authorize]
        [Route("changerole/{userid}/{roleid}")]
        public async Task<ResponseClient> ChangeRole(int userid, int roleid)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            int useuserid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            var mainuser = Logics.ManagementLogic.GetUser(_dbContext, useuserid);

            if (mainuser.ORGID != comid)
                return ReturnResponce.FailedMessageResponce("Та уг мэдээллийн хэрэглэгчийн мэдээллийг засах эрхгүй байна.");

            var currentuser = Logics.ManagementLogic.GetUser(_dbContext, userid);
            if (currentuser == null)
                return ReturnResponce.NotFoundResponce();

            try
            {
                if (currentuser.OLDROLEID != null)
                {
                    if (currentuser.ROLECHANGEDATE == null)
                    {
                        currentuser.ROLEID = roleid;
                        currentuser.ROLECHANGEDATE = DateTime.Now;

                        Logics.ManagementLogic.Update(_dbContext, currentuser);
                    }
                    else
                    {
                        if (currentuser.ROLECHANGEDATE.Value.Year == DateTime.Today.Year && currentuser.ROLECHANGEDATE.Value.Month == DateTime.Today.Month
                            && currentuser.ISAGREEMENT == 1)
                            return ReturnResponce.FailedMessageResponce($"{currentuser.ROLECHANGEDATE.Value.ToString("yyyy-MM-dd")} огноонд эрхийн өөрчлөлт хийсэн байна.{Environment.NewLine} Та сард нэг удаа эрхийн өөрчлөлт хийх боломжтой! {Environment.NewLine} Та {currentuser.ROLECHANGEDATE.Value.AddMonths(1).ToString("yyyy-MM-01")} өдөрөөс хойш  эрхийн өөрчлөлт хийх боломжтой!");

                        if (currentuser.ISAGREEMENT == 1)
                        {
                            currentuser.ROLECHANGEDATE = DateTime.Now;
                            currentuser.OLDROLEID = currentuser.ROLEID;
                            currentuser.ROLEID = roleid;
                        }
                        else
                        {
                            currentuser.ROLECHANGEDATE = DateTime.Now;
                            currentuser.ROLEID = roleid;
                        }

                        Logics.ManagementLogic.Update(_dbContext, currentuser);
                    }
                }
                else
                {
                    currentuser.ROLECHANGEDATE = DateTime.Now;
                    if (currentuser.ISAGREEMENT == 1)
                    {
                        currentuser.OLDROLEID = currentuser.ROLEID;
                    }
                    currentuser.ROLEID = roleid;

                    Logics.ManagementLogic.Update(_dbContext, currentuser);
                }
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("setagreement")]
        public async Task<ResponseClient> SetAgreement()
        {
            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            var currentuser = Logics.ManagementLogic.GetUser(_dbContext, userid);
            if (currentuser == null)
                return ReturnResponce.NotFoundResponce();

            int orgtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));

            try
            {
                if (orgtype == 1)
                {
                    currentuser.ISAGREEMENT = 1;
                    currentuser.AGREEMENTDATE = DateTime.Now;
                    Logics.ManagementLogic.Update(_dbContext, currentuser);
                }
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }



        [HttpGet]
        [Route("usermenu")]
        [Authorize]
        public async Task<ResponseClient> GetUserMenu()
        {
            int orgtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            int roleid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.Roleid));
            var mainmenulist = _dbContext.GET_LOGIN_USER_MENU_SELECT(roleid, orgtype, 1);
            var childmenulist = _dbContext.GET_LOGIN_USER_MENU_SELECT(roleid, orgtype, 0);
            var returnmenu = new List<GET_LOGIN_USER_MENU_SELECT>();

            foreach (GET_LOGIN_USER_MENU_SELECT menu in mainmenulist)
            {
                returnmenu.Add(menu);
                var addedmenu = childmenulist.Where(x => x.PARENTID == menu.MENUID);
                if (addedmenu.Count() > 0)
                {
                    menu.items = new List<GET_LOGIN_USER_MENU_SELECT>();
                    menu.items.AddRange(addedmenu);
                }
                returnmenu.Add(menu);
            }
            return ReturnResponce.ListReturnResponce(returnmenu);
        }


        #endregion


        #region USERDEVICE

        [HttpGet]
        [Authorize]
        [Route("getuserdevices")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetUserDevices()
        {
            var mail = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserMail));
            var devices = from d in _dbContext.SYSTEM_USER_DEVICE
                          join e in _dbContext.SYSTEM_LOGIN_REQUEST on d.LASTREQUESTID equals e.ID
                          where d.USERMAIL == mail.ToLower()
                          select new
                          {
                              d.ID,
                              d.IPADDRESS,
                              d.OSNAME,
                              d.LASTLOGDATE,
                              d.BLOCKED,
                              d.BLOCKEDDATE,
                              e.OSVERSION,
                              e.BROWSERNAME,
                              e.BROWSERVERSION,
                              e.COUNTRY,
                              e.CITY,
                              e.REGIONNAME
                          };

            return devices.Any() ?
                ReturnResponce.ListReturnResponce(devices) :
                ReturnResponce.NotFoundResponce();
        }

        [HttpGet]
        [Authorize]
        [Route("getloginrequests/{ipaddress}/{osname}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetLoginRequests(string ipaddress, string osname)
        {
            var mail = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserMail));
            var devices = from d in _dbContext.SYSTEM_LOGIN_REQUEST
                          where d.USERNAME == mail.ToLower() && d.IPADDRESS == ipaddress && d.OSNAME == osname
                          select new
                          {
                              d.OSVERSION,
                              d.BROWSERNAME,
                              d.BROWSERVERSION,
                              d.COUNTRY,
                              d.CITY,
                              d.REGIONNAME,
                              d.REQUESTDATE
                          };

            return devices.Any() ?
                ReturnResponce.ListReturnResponce(devices) :
                ReturnResponce.NotFoundResponce();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("disabledevice")]
        public async Task<ResponseClient> DisableDevice([FromBody] Entities.CustomModels.DisableRequest request)
        {
            try
            {
                var device = Logics.ManagementLogic.GetUserDevice(_dbContext, _log, request.ID);
                if (device == null || device.USERMAIL != request.UserMail.ToLower())
                    return ReturnResponce.NotFoundResponce();

                //if (device.BLOCKED == 1)
                //    return ReturnResponce.FailedMessageResponce("Тухайн төхөөрөмжийг идэвхгүй болгочихсон байна.");

                //if (device.MAILEXPIREDATE.Value < DateTime.Now)
                //    return ReturnResponce.FailedMessageResponce("Тухайн линкийн хугацаа дууссан байна.");

                if (request.Method == "POST")
                {
                    device.BLOCKED = 0;
                    if (device.WARN == 1)
                    {
                        device.WARN = 0;
                        device.STOPWARNDATE = DateTime.Now;
                    }
                }
                else if (request.Method == "PUT")
                {
                    device.BLOCKED = 0;
                    device.WARN = 1;
                    device.STARTWARNDATE = DateTime.Now;
                }
                else if (request.Method == "DELETE")
                {
                    device.BLOCKED = 1;
                    device.BLOCKEDDATE = DateTime.Now;
                }

                Logics.BaseLogic.Update(_dbContext, device);
                return ReturnResponce.SuccessMessageResponce("Амжилттай.");
            }
            catch (Exception ex)
            {
                Logics.ManagementLogic.ExceptionLog(_dbContext, _log, HttpContext,
                    JsonConvert.SerializeObject(request), "user", "DisableDevice", ex);

                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        #endregion

        /// <summary>
        ///	#Байгууллага регистерийн дугаараар хайх
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Үүсгэсэн огноо : 2018-10-23
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [AllowAnonymous]
        [Route("searchorganization/{key}/{registryno}")]
        public async Task<ResponseClient> SearchOrganization(string key, string registryNo)
        {
            if (!UsefulHelpers.CheckSecurityKey(key))
                return ReturnResponce.AccessDeniedResponce();
            try
            {
                var found = Logics.ManagementLogic.SearchOrganization(_dbContext, _log, registryNo);
                return found == null ?
                    ReturnResponce.NotFoundResponce() :
                    ReturnResponce.ListReturnResponce(found);
            }
            catch (Exception ex)
            {
                _log.LogError($"UserController.SearchOrganization : {registryNo} => {ex.Message}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        ///	#Хэрэглэгч имэйлээр хайх
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Үүсгэсэн огноо : 2019-02-14
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Амжилттай</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [AllowAnonymous]
        [Route("searchuser/{key}/{email}")]
        public async Task<ResponseClient> SearchUser(string key, string email)
        {
            if (!UsefulHelpers.CheckSecurityKey(key))
                return ReturnResponce.AccessDeniedResponce();
            try
            {
                var found = Logics.ManagementLogic.GetUser(_dbContext, email);
                return found == null ?
                    ReturnResponce.NotFoundResponce() :
                    ReturnResponce.SuccessMessageResponce("");
            }
            catch (Exception ex)
            {
                _log.LogError($"UserController.SearchUser : {email} => {ex.Message}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("addorganizationuser/{key}")]
        public async Task<ResponseClient> AddOrganizationUser(string key, [FromBody] Entities.RequestModels.OrganizationUserRequest request)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            if (!UsefulHelpers.CheckSecurityKey(key))
                return ReturnResponce.AccessDeniedResponce();
            try
            {
                if (_dbContext.USER_EXISTS(request.USEREMAIL))
                    return ReturnResponce.FailedMessageResponce($"{request.USEREMAIL} дээр хэрэглэгч бүртгэгдсэн байна...");

                var organization = Logics.ManagementLogic.GetOrganization(_dbContext, request.REGNO);
                if (organization == null)
                {
                    organization = new SYSTEM_ORGANIZATION()
                    {
                        ID = Convert.ToInt16(Logics.BaseLogic.GetNewId(_dbContext, "SYSTEM_ORGANIZATION")),
                        REGNO = request.REGNO,
                        COMPANYNAME = request.COMPANYNAME,
                        ADDRESS = request.ADDRESS,
                        CEONAME = request.CEONAME,
                        EMAIL = request.USEREMAIL,
                        ENABLED = 1,
                        MOBILE = "",
                        ORGTYPE = ORGTYPE.Бизнес,
                        WEBSITE = request.WEBSITE,
                        LOGO = UsefulHelpers.NO_IMAGE_URL
                    };

                    Logics.BaseLogic.Insert(_dbContext, organization);
                }

                if (organization.ORGTYPE != ORGTYPE.Бизнес)
                    return ReturnResponce.FailedMessageResponce("Зөвхөн бизнес төрөлтэй байгууллага дээр хэрэглэгч бүртгэх боломжтой.");

                var password = Cryptography.CreatePassword();
                var user = new SYSTEM_USERS()
                {
                    ID = Convert.ToInt16(Logics.BaseLogic.GetNewId(_dbContext, "SYSTEM_USERS")),
                    USERMAIL = request.USEREMAIL,
                    FIRSTNAME = request.USERNAME,
                    ENABLED = ENABLED.Идэвхитэй,
                    ISADMIN = 1,
                    ORGID = organization.ID,
                    ROLEID = 1, // DEFAULT ИНГЭЭД ТАВИХ
                    COOPERATION = 0,
                    PHONE = request.USERMOBILE,
                    REGDATE = DateTime.Now,
                    USERPASSWORD = Cryptography.Sha256Hash(password)
                };

                Logics.BaseLogic.Insert(_dbContext, user);

                Emailer.Send(_dbContext, _log, request.USEREMAIL, password, MessageType.NewUser,
                    request.USEREMAIL, request.USEREMAIL);

                return ReturnResponce.SuccessMessageResponce("Амжилттай.");
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(System.Reflection.MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

    }

    public class MultipartUser
    {
        public string USERMAIL { get; set; }

        public string PHONE { get; set; }

        public string FIRSTNAME { get; set; }

        public string LASTNAME { get; set; }

    }

}
