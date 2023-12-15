using System;
using System.Collections.Generic;
using System.Linq;
using MimeKit;
using static EDIWEBAPI.Enums.SystemEnums;
using EDIWEBAPI.SystemResource;
using EDIWEBAPI.Utils;
using Microsoft.Extensions.Logging;
using EDIWEBAPI.Context;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace EDIWEBAPI.Controllers.SendData
{
    public class Emailer
    {
        public static string DEVELOPER_EMAIL = "batulzii@datacare.mn";

        public static void Send(EmailMessage message)
        {
            //throw new Exception("5.7.0 Your message could not be sent. The limit on the number of allowed outgoing messages was exceeded. Try again later.");

            var mail = new MimeMessage()
            {
                Subject = message.Subject,
                Body = new TextPart("html") { Text = message.Content }
            };

            var isStore = message.FromAddress.ToUpper() == EmailConfiguration.STORE_ORDER_MAIL.ToUpper();
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                if (!string.IsNullOrEmpty(message.CC))
                {
                    var cc = InternetAddress.Parse("tulgaa@datacare.mn");
                    if (InternetAddress.TryParse(message.CC, out cc))
                        mail.Cc.Add(cc);
                }

                var bcc = InternetAddress.Parse("admin@e-mart.mn");
                mail.Bcc.Add(bcc);

                mail.From.Add(new MailboxAddress(isStore ? 
                    EmailConfiguration.STORE_ADDRESS_NAME : 
                    EmailConfiguration.URTO_ADDRESS_NAME, 
                    message.FromAddress));

                //mail.From.Add(new MailboxAddress(EmailConfiguration.URTO_ADDRESS_NAME, EmailConfiguration.URTO_OPERATOR_MAIL));
                mail.To.Add(new MailboxAddress(message.ToAddress, message.ToAddress));

                //var smtp = message.FromAddress == EmailConfiguration.MARKETING_MAIL ? EmailConfiguration.STORE_SMTP_ADDRESS : EmailConfiguration.SMTP_ADDRESS;

                client.Connect(isStore ? 
                    EmailConfiguration.STORE_SMTP_ADDRESS : 
                    EmailConfiguration.URTO_SMTP_ADDRESS, 
                    EmailConfiguration.SMTP_PORT, SecureSocketOptions.None);
                //client.Authenticate(EmailConfiguration.URTO_OPERATOR_MAIL, EmailConfiguration.URTO_OPERATOR_PASSWORD);

                client.Authenticate(message.FromAddress, 
                    isStore ? 
                    EmailConfiguration.STORE_ORDER_PASSWORD : 
                    EmailConfiguration.URTO_OPERATOR_PASSWORD);

                client.Send(mail);
                client.Disconnect(true);
            }
        }

        public static MemoryStream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ResponseClient SendAttachment(ILogger _log, EmailMessage message)
        {
            try
            {
                var ms = new MemoryStream();
                ms = GenerateStreamFromString(message.Attachment);
                ms.Seek(0, SeekOrigin.Begin);

                using (var mail = new MailMessage()
                {
                    Subject = message.Subject,
                    Body = message.Content,
                    IsBodyHtml = true
                })
                {
                    //mail.From = new MailAddress(message.FromAddress);
                    mail.From = new MailAddress(EmailConfiguration.URTO_OPERATOR_MAIL, EmailConfiguration.URTO_ADDRESS_NAME);
                    mail.To.Add(message.ToAddress);

                    if (!string.IsNullOrEmpty(message.CC))
                        mail.Bcc.Add(new MailAddress(message.CC));
                    mail.Attachments.Add(new Attachment(ms, "invite.ics", "text/calendar"));

                    try
                    {
                        using (var smtp = new System.Net.Mail.SmtpClient(EmailConfiguration.URTO_SMTP_ADDRESS, EmailConfiguration.SMTP_PORT)
                        {
                            EnableSsl = true
                        })
                        {
                            //smtp.Credentials = new NetworkCredential(message.FromAddress, EmailConfiguration.OPERATOR_PASSWORD);
                            smtp.Credentials = new NetworkCredential(EmailConfiguration.URTO_OPERATOR_MAIL, EmailConfiguration.URTO_OPERATOR_PASSWORD);
                            ServicePointManager.ServerCertificateValidationCallback =
                                    delegate (object s,
                                             System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                             System.Security.Cryptography.X509Certificates.X509Chain chain,
                                             System.Net.Security.SslPolicyErrors sslPolicyErrors)
                                    {
                                        return true;
                                    };
                            smtp.Send(mail);
                            _log.LogInformation("Мейл амжилттай ", message.Subject);
                            return ReturnResponce.SaveSucessResponce();
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.LogError("Мейл илгээх үеийн алдаа ", ex.ToString());
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError("Мейл бэлдэх үеийн алдаа ", ex.ToString());
                throw ex;
            }
        }

        public static ResponseClient Send(
            OracleDbContext _dbContext, ILogger _log,
            string email, string message, MessageType type,
            string username, string fileurl = null, string mailbody = null,
            string contractno = null, string storename = null, string date = null,
            string userMail = null, int storeid = 0, string cc = null)
        {
            var emailMessage = new EmailMessage()
            {
                Type = type,
                ToAddress = email,
                FromAddress = type == MessageType.Order || type == MessageType.Return ?
                    EmailConfiguration.STORE_ORDER_MAIL : EmailConfiguration.URTO_OPERATOR_MAIL,
                Priority = (byte) type,
                UserEmail = userMail ?? EmailConfiguration.URTO_OPERATOR_MAIL,
                StoreId = storeid,
                CC = cc
            };
            try
            {
                if (type == MessageType.Order)
                {
                    emailMessage.Content = MailResource.OrderTemplate;
                    emailMessage.Subject = "Захиалга " + contractno;
                }
                else if (type == MessageType.Return)
                {
                    emailMessage.Content = MailResource.OrderTemplate;
                    emailMessage.Subject = "Буцаалт " + contractno;
                }
                else if (type == MessageType.NewUser)
                {
                    emailMessage.Content = MailResource.NewUserTemplate;
                    emailMessage.Subject = "Шинэ хэрэглэгч";
                }
                else if (type == MessageType.ForgotPassword)
                {
                    emailMessage.Content = MailResource.ForgetPass;
                    emailMessage.Subject = "Нууц үг сэргээх";
                }
                else if (type == MessageType.Payment)
                {
                    emailMessage.Content = MailResource.PaymentCancelTemplate;
                    emailMessage.Subject = "Нэхэмжлэл цуцлалт";
                }
                else
                {
                    emailMessage.Content = MailResource.NewUserTemplate;
                    emailMessage.Subject = "Шинэ хэрэглэгч";
                    emailMessage.Priority = 99;
                }

                // emailMessage.HtmlBody = $"{mailData.Replace("#systemdata", $"{message}").Replace("#systemmail", username).Replace("#fileurl", fileurl).Replace("#mailbody", mailbody)}";
                emailMessage.Content = emailMessage.Content.Replace("#systemdata", $"{message}")
                    .Replace("#systemmail", username).Replace("#fileurl", fileurl).Replace("#mailbody", mailbody)
                    .Replace("#StoreName", storename).Replace("#Date", date).Replace("#Contractno", contractno);

                if (type == MessageType.Order || type == MessageType.Return)
                {
                    emailMessage.Content = emailMessage.Content.Replace("#downloadcaption",
                        type == MessageType.Order ? "ЗАХИАЛГЫН МЭДЭЭЛЭЛ ХАРАХ" : "БУЦААЛТЫН МЭДЭЭЛЭЛ ХАРАХ");
                }

                Logics.MailLogic.Add(_dbContext, emailMessage);
                return ReturnResponce.SuccessMessageResponce("Мейлийг илгээлээ!");
            }
            catch (Exception ex)
            {
                UsefulHelpers.WriteErrorLog(_log, System.Reflection.MethodBase.GetCurrentMethod(), ex);
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static ResponseClient Send(
            OracleDbContext _dbContext, ILogger _log,
            string email, string message, MessageType type,
            string username, string userMail)
        {
            return Send(_dbContext, _log, email, message, type,
                username, null, null, null,
                null, null, userMail);
        }
    }
}
