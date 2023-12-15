using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.SystemResource;

namespace WebAPI.Controllers.SystemControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        [Route("Send/{email}/{message}")]
        public async Task<ResponseClient> Post(string email, string message)
        {
            try

            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Зэндмэнэ систем", "noreply@datacare.mn"));
                emailMessage.To.Add(new MailboxAddress("Zendmene system", email));
                emailMessage.Subject = "Zendmene platform";
                string mmailData = MailResource.SampleTemplate.ToString();
                emailMessage.Body = new TextPart("html") { Text = $"{mmailData.Replace("#systemdata", $"{message}")}" };
                using (var client = new SmtpClient())
                {
                    client.Connect("mail.datacare.mn", 25, SecureSocketOptions.None);
                    client.Authenticate("noreply@datacare.mn", "12xmlv");

                    client.Send(emailMessage);
                }

                return ReturnResponce.SuccessMessageResponce("Мейлийг илгээлээ!");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

    }
}
