using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.SystemResource;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;
using System.Linq;
using EDIWEBAPI.Controllers.SysManagement;
using System.Collections.Generic;
using EDIWEBAPI.Attributes;

namespace EDIWEBAPI.Controllers.SendData
{
    [Route("api/mailsend")]
    public class MailSendController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MailSendController> _log;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MailSendController(OracleDbContext context, ILogger<MailSendController> log)
        {
            _dbContext = context;
            _log = log;
        }

        [HttpPost]
        [Authorize]
        [Route("multiple")]
        public async Task<ResponseClient> SendMails([FromBody] MultiMailRequest request)
        {
            try
            {
                var users = from u in _dbContext.SYSTEM_USERS.ToList()
                            join o in request.Organizations on u.ORGID equals o.id
                            where u.ENABLED == ENABLED.Идэвхитэй
                            select new { u.USERMAIL, o.strymd, o.stpymd };

                if (!users.Any())
                    return ReturnResponce.NotFoundResponce();

                var userMail = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserMail));
                var template = MailResource.PaymentMassTemplate;

                foreach (var user in users)
                {
                    var newMail = new EmailMessage()
                    {
                        Subject = "Төлбөрийн мэдээлэл",
                        Content = template
                            .Replace("#Date", $"{user.strymd} - {user.stpymd}")
                            .Replace("#systemdata", request.Note),
                        ToAddress = user.USERMAIL,
                        StoreId = UsefulHelpers.STORE_ID,
                        Type = MessageType.PaymentMass,
                        Priority = (int)MessageType.PaymentMass,
                        UserEmail = userMail,
                        FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL
                    };
                    Logics.MailLogic.Add(_dbContext, newMail);
                }
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("passcode")]
        public async Task<ResponseClient> AddRequest(string key, string email)
        {
            if (!UsefulHelpers.CheckSecurityKey(key))
                return ReturnResponce.AccessDeniedResponce();
            try
            {
                var request = Logics.MailLogic.AddRequest(_dbContext, email);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("passcode/{key}/{email}/{passcode}")]
        public async Task<ResponseClient> CheckRequest(string key, string email, string passcode)
        {
            if (!UsefulHelpers.CheckSecurityKey(key))
                return ReturnResponce.AccessDeniedResponce();
            try
            {
                var found = Logics.MailLogic.CheckRequest(_dbContext, email, passcode);
                return found ?
                    ReturnResponce.SuccessMessageResponce("") :
                    ReturnResponce.FailedMessageResponce("");
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("getrequests/{count}/{key}")]
        public async Task<ResponseClient> GetRequests(int count, string key)
        {
            try
            {
                if (!HasPrivilige(key))
                    return ReturnResponce.AccessDeniedResponce();

                var requests = Logics.MailLogic.GetRequests(_dbContext, count);
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
        [Route("sendrequest/{mailid}/{key}")]
        public async Task<ResponseClient> SendMailRequest(string mailid, string key)
        {
            if (!HasPrivilige(key))
                return ReturnResponce.AccessDeniedResponce();

            return Logics.MailLogic.Send(_dbContext, _log, mailid);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("sendnewuser/{mail}/{password}")]
        public async Task<ResponseClient> SendNewuserMail(string mail, string password)
        {
            try
            {
                Emailer.Send(_dbContext, _log, mail, password, MessageType.NewUser, mail, 
                    EmailConfiguration.URTO_OPERATOR_MAIL);

                return ReturnResponce.SuccessMessageResponce("Мейл илгээв");
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Route("massmail/{key}")]
        [AllowAnonymous]
        public ResponseClient SendMassMail(string key)
        {
            if (!HasPrivilige(key))
                return ReturnResponce.AccessDeniedResponce();
            try
            {
                var sdate = DateTime.Now;
                var users = (from o in _dbContext.SYSTEM_ORGANIZATION
                             join u in _dbContext.SYSTEM_USERS on o.ID equals u.ORGID
                             where o.ORGTYPE == ORGTYPE.Бизнес && o.ENABLED == 1 && u.ENABLED == ENABLED.Идэвхитэй
                             select new { u.USERMAIL }).ToList();
                int i = 0;
                foreach (var current in users)
                {
                    var emailMessage = new EmailMessage()
                    {
                        Type = MessageType.Brochure,
                        Subject = "Шинэ үйлчилгээ нэмэгдлээ.",
                        Content = MailResource.Production1,
                        ToAddress = current.USERMAIL,
                        FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL,
                        UserEmail = EmailConfiguration.URTO_OPERATOR_MAIL,
                        Priority = 99
                    };
                    System.Threading.Thread.Sleep(1000);
                    Logics.MailLogic.Add(_dbContext, emailMessage);
                    i++;
                }
                TimeSpan ts = DateTime.Now - sdate;
                return ReturnResponce.SuccessMessageResponce($"{ts.TotalSeconds} секундэд {i} ширхэг мейлийг илгээв!");
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Route("sendmassmail")]
        [AllowAnonymous]
        public ResponseClient SendMassMail([FromBody] List<MassMail> maillist)
        {
            DateTime sdate = DateTime.Now;
            int i = 0;
            int u = maillist.Count;
            foreach (MassMail emaildata in maillist)
            {
                if (i >= 600)
                {
                    var emailMessage = new EmailMessage()
                    {
                        Type = MessageType.Brochure,
                        Subject = "Сайн байна уу?",
                        Content = MailResource.BrochureTemplate,
                        ToAddress = emaildata.email,
                        FromAddress = "info@e-mart.mn",
                        UserEmail = EmailConfiguration.URTO_OPERATOR_MAIL,
                        Priority = 8
                    };
                    //Logics.MailLogic.Add(_dbContext, emailMessage);
                }
                i++;
            }
            DateTime edate = DateTime.Now;
            TimeSpan ts = edate - sdate;
           return ReturnResponce.SuccessMessageResponce($"{ts.TotalSeconds} секундэд {i} ширхэг мейлийг илгээв!");
        }

        public static bool HasPrivilige(object email)
        {
            return (email == null ? "" : email.ToString()) == "tulgaa@datacare.mn";
        }

        [HttpPost]
        [Authorize]
        [Route("sendmails")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient SendMails()
        {
            var logMethod = "MAILSENDCONTROLLER.SENDMAILS";
            _log.LogInformation($"{logMethod} START :{DateTime.Now}");

            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var email = UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserMail);
            if (!HasPrivilige(email))
                return ReturnResponce.AccessDeniedResponce();

            try
            {
                var mails = from users in _dbContext.SYSTEM_USERS
                            where users.ENABLED == ENABLED.Идэвхитэй
                            select new EmailMessage()
                            {
                                Type = MessageType.Brochure,
                                Subject = "Сайн байна уу?",
                                Content = MailResource.BrochureTemplate,
                                ToAddress = users.USERMAIL,
                                FromAddress = "info@e-mart.mn",
                                UserEmail = EmailConfiguration.URTO_OPERATOR_MAIL,
                                Priority = 9
                            };

                _log.LogInformation($"{logMethod} COUNT :{DateTime.Now} : {mails.Count()}");

                if (mails.Any())
                {
                    foreach (var message in mails)
                    {
                        var newMessage = Logics.MailLogic.GetLogObject(message);
                        _dbContext.SYSTEM_MAIL_LOG.Add(newMessage);
                        System.Threading.Thread.Sleep(500);
                    }
                    _dbContext.SaveChanges();
                }

                _log.LogInformation($"{logMethod} END :{DateTime.Now}");

                return ReturnResponce.SuccessMessageResponce(mails.Count().ToString());
            }
            catch (Exception ex)
            {
                _log.LogError($"{logMethod} :{ex.Message}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpGet]
        [Route("systemusersmail")]
        [AllowAnonymous]
        public ResponseClient GetSystemMails()
        {
            var query = from users in _dbContext.SYSTEM_USERS
                       select new
                       {
                           email = users.USERMAIL
                       };
            return ReturnResponce.ListReturnResponce(query);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("sendtestmail/{toemail}")]
        public ResponseClient SendTestMail(string toemail)
        {
            var emailMessage = new EmailMessage()
            {
                Type = MessageType.Marketing,
                Subject = "Борлуулалтын мэдээлэл хялбар авах боломж",
                Content = MailResource.Production1,
                ToAddress = toemail,
                FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL,
                UserEmail = EmailConfiguration.URTO_OPERATOR_MAIL,
                Priority = 9
            };

            Logics.MailLogic.Add(_dbContext, emailMessage);

            return ReturnResponce.SaveSucessResponce();
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("sendertest/{fromemail}/{toemail}/{msg}")]
        public ResponseClient SendBrochure(string fromemail, string toemail, string msg)
        {
            var emailMessage = new EmailMessage()
            {
                Type = MessageType.Brochure,
                Subject = "Сайн байна уу?",
                Content = msg,
                ToAddress = toemail,
                FromAddress = fromemail,
                UserEmail = EmailConfiguration.URTO_OPERATOR_MAIL,
                Priority = 9
            };

            Logics.MailLogic.Add(_dbContext, emailMessage);

            return ReturnResponce.SaveSucessResponce();
        }



        [HttpGet]
        [AllowAnonymous]
        [Route("sendbrochure/{email}/{templateid}")]
        public ResponseClient SendBrochure(string email, int templateid)
        {
            var emailMessage = new EmailMessage()
            {
                Type = MessageType.Brochure,
                Subject = "Сайн байна уу?",
                Content = templateid == 1 ? MailResource.BrochureTemplate : MailResource.BrochureTemplate2,
                FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL,
                ToAddress = email,
                Priority = 9,
                UserEmail = EmailConfiguration.URTO_OPERATOR_MAIL
            };

            Logics.MailLogic.Add(_dbContext, emailMessage);

            return ReturnResponce.SaveSucessResponce();
        }



        [HttpPost]
        [AllowAnonymous]
        [Route("Send/{email}/{message}")]
        public async Task<ResponseClient> Post(
            string email, string message, MessageType type, 
            string username, string fileurl = null, string mailbody = null, 
            string contractno = null, string storename = null, string date = null,
            string userMail = null)
        {
            return Emailer.Send(_dbContext, _log, email, message, type,
                username, fileurl, mailbody, contractno, storename, date,
                userMail);
        }
        

        [HttpPost]
        [AllowAnonymous]
        [Route("sendlicenserequest/{email}/{comid}/{licensekey}/{price}/{qrdata}")]
        public ResponseClient SendLicenseRequest(string email, int comid, string licensekey, decimal? price, string qrdata)
        {
            var currentcompany = Logics.ManagementLogic.GetOrganization(_dbContext, comid);
            if (currentcompany != null)
            {
                var emailMessage = new EmailMessage()
                {
                    Type = MessageType.Payment,
                    Subject = "Лицензийн төлбөр",
                    ToAddress = email,
                    FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL,
                    Priority = 4
                };
                string mailData = "";
                qrdata = "https://chart.googleapis.com/chart?chs=150x150&cht=qr&chl=" + qrdata; 

                var controler = new ServiceUtilsController(_dbContext, null);
                string invoiceno = Convert.ToString(controler.GetInvoiceNo().Value);

                mailData = MailResource.LicenseRequest.ToString();
                mailData = mailData.Replace("{MOBILE}", currentcompany.MOBILE).Replace("{EMAIL}", currentcompany.EMAIL).Replace("{COMPANYNAME}", currentcompany.COMPANYNAME)
                                   .Replace("{ADDRESS}", currentcompany.ADDRESS).Replace("{QRDATA}", qrdata).Replace("{INVOICENO}", invoiceno);
                mailData = mailData.Replace("{LICENSEKEY}", $"{licensekey}").Replace("{REQUESTDATE}", $"{DateTime.Now.ToShortDateString()}")
                                   .Replace("{CHARGEDATE}", $"{DateTime.Now.ToShortDateString()}").Replace("{COUNT}", "1").Replace("{PRICE}", $"{price}")
                                   .Replace("{ALLPRICE}", $"{price}");
                emailMessage.Content = mailData;

                Logics.MailLogic.Add(_dbContext, emailMessage);
                return ReturnResponce.SuccessMessageResponce("Мейл илгээв"); 
            }
            else
                return ReturnResponce.NotFoundResponce();
        }






        [HttpGet]
        [Route("sendical/{Mailaddress}")]
        public async Task<ResponseClient> SendMail(CalendarMail mailcal, HttpContext context)
        {
            string usermail = Convert.ToString(UsefulHelpers.GetIdendityValue(context, UserProperties.UserMail));
            try
            {
                string currentDate = $"{DateTime.Now.ToString("yyyyMMdd")}T{DateTime.Now.ToString("HHm")}";
                string edate = $"{DateTime.Now.AddMinutes(30).ToString("yyyyMMdd")}T{DateTime.Now.AddMinutes(30).ToString("HHm")}";
                string uid = Convert.ToString(Guid.NewGuid());

                var message = new EmailMessage()
                {
                    Type = MessageType.Payment,
                    Subject = mailcal.Subject,
                    Content = mailcal.Description,
                    FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL,
                    ToAddress = mailcal.MailAddress,
                    CC = usermail,
                    Attachment = MailResource.ICalendar.Replace("{messageData}", mailcal.MessageData).Replace("{replymail}", "noreply@datacare.mn")
                                                       .Replace("{Location}", mailcal.Location).Replace("{UID}", uid).Replace("{Dates}", currentDate)
                                                       .Replace("{Datee}", edate),
                    UserEmail = usermail,
                    Priority = 1
                };

                Logics.MailLogic.Add(_dbContext, message);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError("Мейл хадгалах үеийн алдаа ", ex.ToString());
                return ReturnResponce.SaveSucessResponce();
            }
        }




        #region MailArchive
        [HttpPost]
        [Route("getmailhistory")]
        [Authorize]

        public async Task<ResponseClient> GetMailHistory([FromBody] MailHistory body)
        {
            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            if (orgType == (int)ORGTYPE.Бизнес)
                return ReturnResponce.AccessDeniedResponce();
            try
            {
                var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
                var response = Logics.MailLogic.GetMailLogs(_dbContext, body.sdate, body.edate.AddDays(1), orgType == 2 ? comid : 0, body.issent, body.mail);
                return ReturnResponce.ListReturnResponce(response);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpGet]
        [Route("resendmailhistory/{mailid}")]
        [Authorize]
        public async Task<ResponseClient> ResendMailHistory(string mailid)
        {
            return Logics.MailLogic.Send(_dbContext, _log, mailid);
        }

        
        
    }

    #endregion

    public class MassMail
    {
        public string email { get; set; }
    }
}
