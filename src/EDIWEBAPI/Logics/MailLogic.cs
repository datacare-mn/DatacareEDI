using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using EDIWEBAPI.Controllers.SendData;
using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using EDIWEBAPI.Entities.ResultModels;
using EDIWEBAPI.Entities.DBModel.SendData;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Logics
{
    public class MailLogic : BaseLogic
    {
        public static int AddRequest(OracleDbContext _context, string email)
        {
            var passcode = UsefulHelpers.GetRandomNumber(1000, 10000);
            var now = DateTime.Now;
            var count = _context.REQ_EMAIL.Where(a => a.EMAIL == email && a.EXPIREDATE > now && a.CONFIRMED != 1).Count();
            if (count > 3)
                throw new Exception("Та баталгаажуулахгүй 3с дээш хүсэлт явуулсан байна.");

            var newRequest = new REQ_EMAIL()
            {
                ID = Convert.ToInt16(GetNewId(_context, typeof(REQ_EMAIL).Name)),
                EMAIL = email,
                PASSCODE = Cryptography.Sha256Hash(passcode.ToString()),
                CONFIRMED = 0,
                REQUESTDATE = DateTime.Now,
                TYPE = 1,
                EXPIREDATE = DateTime.Now.AddMinutes(10)
            };

            Insert(_context, newRequest);

            var content = SystemResource.MailResource.MasterConfirm
                        .Replace("#email", email)
                        .Replace("#pass", passcode.ToString());

            var mail = new EmailMessage()
            {
                Type = MessageType.MasterConfirm,
                ToAddress = email,
                FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL,
                Subject = "Өртөө ситемийн баталгаажуулах код",
                Content = content,
                Priority = (int) MessageType.MasterConfirm,
                StoreId = 2,
                UserEmail = EmailConfiguration.URTO_OPERATOR_MAIL
            };
            Add(_context, mail);

            return passcode;
        }

        public static bool CheckRequest(OracleDbContext _context, string email, string passCode)
        {
            var found = false;
            var lastRequest = _context.REQ_EMAIL.Where(a => a.EMAIL == email).OrderByDescending(a => a.REQUESTDATE).FirstOrDefault();
            if (lastRequest != null)
            {
                found = lastRequest.PASSCODE == Cryptography.Sha256Hash(passCode);
                if (found && lastRequest.CONFIRMED != 1)
                {
                    lastRequest.CONFIRMED = 1;
                    lastRequest.CONFIRMEDDATE = DateTime.Now;
                    Update(_context, lastRequest);
                }
            }

            return found;
        }

        public static SYSTEM_MAIL_LOG Get(OracleDbContext _context, string id)
        {
            return _context.SYSTEM_MAIL_LOG.FirstOrDefault(x => x.ID == id);
        }

        public static List<SYSTEM_MAIL_LOG> GetMailLogs(OracleDbContext _context, DateTime beginDate, DateTime endDate, int storeId, int isSent)
        {
            return (from m in _context.SYSTEM_MAIL_LOG
                 where m.REQUESTDATE >= beginDate && m.REQUESTDATE <= endDate //(storeId == 0 || m.STOREID == storeId)
                    && (isSent == -1 || m.ISSEND == isSent)
                    orderby m.REQUESTDATE
                 select m).ToList();
        }

        public static List<SYSTEM_MESSAGE_ARCHIVE> GetSmsLogs(OracleDbContext _context, DateTime beginDate, DateTime endDate, 
            int storeId, int issent)
        {
            return (from m in _context.SYSTEM_MESSAGE_ARCHIVE
                where m.REQUESTDATE >= beginDate && m.REQUESTDATE <= endDate //(storeId == 0 || m.STOREID == storeId)
                    && (issent == -1 || m.ISSENT == issent)
                orderby m.REQUESTDATE
                select m).ToList();
        }

        public static IQueryable<MailModel> GetRequests(OracleDbContext _context, int limit)
        {
            return (from m in _context.SYSTEM_MAIL_LOG
                    where m.ISSEND == 0 && m.TRY <= 2
                    orderby m.PRIORITY, m.REQUESTDATE, m.TRY
                    select new MailModel()
                    {
                        ID = m.ID,
                        MAIL = m.MAIL,
                        SUBJECT = m.MAILSUBJECT
                    }).Take(limit);
        }

        public static ResponseClient Send(OracleDbContext _context, ILogger _log, string mailId)
        {
            var mail = Get(_context, mailId);
            if (mail == null)
                return ReturnResponce.NotFoundResponce();

            var emailMessage = new EmailMessage()
            {
                Subject = mail.MAILSUBJECT,
                Content = mail.MAILBODY,
                ToAddress = mail.MAIL,
                FromAddress = mail.MAILFROM,
                CC = mail.CC,
                Attachment = mail.ATTACHMENT
            };

            mail.SENDDATE = DateTime.Now;
            try
            {
                if (string.IsNullOrEmpty(mail.ATTACHMENT))
                    Emailer.Send(emailMessage);
                else
                    Emailer.SendAttachment(_log, emailMessage);

                mail.ISSEND = 1;
                Update(_context, mail);

                return ReturnResponce.SuccessMessageResponce("Мейлийг амжилттай илгээв!");
            }
            catch (Exception ex)
            {
                mail.ISSEND = 0;
                mail.TRY = mail.TRY + 1;
                mail.ERROR = ex.Message;

                Update(_context, mail);

                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static SYSTEM_MAIL_LOG GetLogObject(EmailMessage message)
        {
            return new SYSTEM_MAIL_LOG()
            {
                ID = Convert.ToString(Guid.NewGuid()),
                TYPE = message.Type,
                MAIL = message.ToAddress,
                MAILFROM = message.FromAddress,
                MAILSUBJECT = message.Subject,
                MAILBODY = message.Content,
                PRIORITY = message.Priority,
                REQUESTBY = message.UserEmail,
                REQUESTDATE = DateTime.Now,
                CC = message.CC,
                ATTACHMENT = message.Attachment,
                SENDDATE = null,
                TRY = 0,
                ISSEND = 0,
                STOREID = message.StoreId
            };
        }

        public static void Add(OracleDbContext _context, EmailMessage message)
        {
            Insert(_context, GetLogObject(message));
        }

        public static IQueryable<MailModel> GetSMSRequests(OracleDbContext _context, int limit)
        {
            return (from m in _context.SYSTEM_MESSAGE_ARCHIVE
                    where m.ISSENT == 0 && m.TRY <= 2
                    orderby m.PRIORITY, m.REQUESTDATE, m.TRY
                    select new MailModel()
                    {
                        ID = m.ID,
                        MAIL = m.PHONENUMBER,
                        SUBJECT = m.SMS
                    }).Take(limit);
        }

        public static void AddSMS(OracleDbContext _context, EDIWEBAPI.Enums.SystemEnums.MessageType type, string sms, string phoneNumber, int storeId = 0)
        {
            var newLog = new SYSTEM_MESSAGE_ARCHIVE()
            {
                ID = Convert.ToString(Guid.NewGuid()),
                TYPE = type,
                SMS = sms,
                PHONENUMBER = phoneNumber,
                REQUESTDATE = DateTime.Now,
                ISSENT = 0,
                TRY = 0,
                PRIORITY = type == Enums.SystemEnums.MessageType.None ? 99 : (byte) type,
                STOREID = storeId
            };

            Insert(_context, newLog);
        }

        public static SYSTEM_MESSAGE_ARCHIVE GetSMS(OracleDbContext _context, string id)
        {
            return _context.SYSTEM_MESSAGE_ARCHIVE.FirstOrDefault(x => x.ID == id);
        }

        public static async Task<ResponseClient> SendSMS(OracleDbContext _context, ILogger _log, string messageId)
        {
            var message = GetSMS(_context, messageId);
            if (message == null)
                return ReturnResponce.NotFoundResponce();

            message.SENDDATE = DateTime.Now;
            try
            {
                var response = await Messager.Send(message.SMS, message.PHONENUMBER);
                if (response.Success)
                {
                    message.ISSENT = 1;
                    message.SENDDATE = DateTime.Now;
                }
                else
                {
                    message.ERROR = response.Message;
                    message.ISSENT = 0;
                    message.TRY = message.TRY + 1;
                }

                Update(_context, message);
                return response.Success ? 
                    ReturnResponce.SuccessMessageResponce("Мейлийг амжилттай илгээв!") :
                    ReturnResponce.FailedMessageResponce(response.Message);
            }
            catch (Exception ex)
            {
                message.ISSENT = 0;
                message.TRY = message.TRY + 1;
                message.ERROR = ex.Message;

                Update(_context, message);
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
    }
}
