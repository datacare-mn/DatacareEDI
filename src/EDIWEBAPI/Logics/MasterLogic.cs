using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.DBModel.MasterSku;
using EDIWEBAPI.Entities.DBModel.Product;
using EDIWEBAPI.Entities.ResultModels;
using EDIWEBAPI.Entities.FilterViews;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.Feedback;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using EDIWEBAPI.Utils;
using EDIWEBAPI.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using EDIWEBAPI.Controllers.SendData;
using System.Text;
using static EDIWEBAPI.Enums.SystemEnums;
using EDIWEBAPI.SystemResource;

namespace EDIWEBAPI.Logics
{
    public class MasterLogic : BaseLogic
    {
        private static List<int> GetDepartmentIds(OracleDbContext _context, ILogger _log, ORGTYPE orgType, int userId, int departmentId)
        {
            var departments = new List<int>();
            if (departmentId != 0)
                departments.Add(departmentId);
            else
            {
                if (orgType == ORGTYPE.Дэлгүүр)
                    departments = (from d in _context.MST_DEPARTMENT
                                   join u in _context.SYSTEM_USER_DEPARTMENT on d.ID equals u.DEPARTMENTID
                                   where u.USERID == userId && d.ENABLED == 1
                                   group d by d.ID into g
                                   select g.Key).ToList();
                else
                    departments = _context.MST_DEPARTMENT
                        .Where(a => a.ENABLED == 1)
                        .Select(a => a.ID).ToList();
            }

            return departments;
        }

        public static List<MST_DEPARTMENT> GetContractDepartments(OracleDbContext _context, ILogger _logger, string contractNo)
        {
            return (from c in _context.MST_CONTRACT
                    join e in _context.MST_CONTRACT_DEPARTMENT on c.CONTRACTID equals e.CONTRACTID
                    join m in _context.MST_DEPARTMENT_MAPPING on e.DEPARTMENTCODE equals m.DEPARTMENTCODE
                    join d in _context.MST_DEPARTMENT on m.DEPARTMENTID equals d.ID
                    where c.CONTRACTNO == contractNo && m.ENABLED == 1 && d.ENABLED == 1
                    orderby m.VIEWORDER
                    select d).ToList();
        }

        public static List<MST_DEPARTMENT> GetDepartments(OracleDbContext _context, ILogger _logger, int orgType, int userId)
        {
            var departments = new List<MST_DEPARTMENT>();

            if (orgType == (int)ORGTYPE.Дэлгүүр)
                departments = (from d in _context.MST_DEPARTMENT
                               join u in _context.SYSTEM_USER_DEPARTMENT on d.ID equals u.DEPARTMENTID
                               where u.USERID == userId && d.ENABLED == 1
                               select d).ToList();
            else
                departments = _context.MST_DEPARTMENT
                    .Where(a => a.ENABLED == 1).Select(a => a).ToList();

            return departments;
        }

        public static ResponseClient GetProductPlan(OracleDbContext _context, ILogger _log,
            ORGTYPE orgType, int orgId, int userId, int departmentId)
        {
            var departments = GetDepartmentIds(_context, _log, orgType, userId, departmentId);

            var results = (from p in _context.REQ_PRODUCT
                           join r in _context.MST_PRODUCT_REQUEST on p.REQUESTID equals r.ID
                           join sd in _context.MST_PRODUCT_STATUS_DETAIL on p.STATUS equals sd.ID
                           join s in _context.MST_PRODUCT_STATUS on sd.STATUS equals s.ID
                           join d in departments on p.DEPARTMENTID equals d
                           where s.DECISION != 1
                                && orgType == ORGTYPE.Дэлгүүр ? p.STOREID == UsefulHelpers.STORE_ID : p.ORGID == orgId
                           group r by new { GROUPID = r.GROUPID, STATUSCODE = s.ID, STATUS = s.NAME } into g
                           select new
                           {
                               g.Key.GROUPID,
                               g.Key.STATUSCODE,
                               g.Key.STATUS,
                               QTY = g.Count()
                           }).ToList();

            if (orgType == ORGTYPE.Дэлгүүр)
            {
                var orgRequests = (from p in _context.REQ_PRODUCT_ORG
                                   join r in _context.MST_PRODUCT_REQUEST on p.REQUESTID equals r.ID
                                   join sd in _context.MST_PRODUCT_STATUS_DETAIL on p.STATUS equals sd.ID
                                   join s in _context.MST_PRODUCT_STATUS on sd.STATUS equals s.ID
                                   join d in departments on p.DEPARTMENTID equals d
                                   where s.DECISION != 1
                                   group r by new { GROUPID = r.GROUPID, STATUSCODE = s.ID, STATUS = s.NAME } into g
                                   select new
                                   {
                                       g.Key.GROUPID,
                                       g.Key.STATUSCODE,
                                       g.Key.STATUS,
                                       QTY = g.Count()
                                   }).ToList();

                if (orgRequests.Any())
                    results.AddRange(orgRequests);
            }

            var statuses = (from s in _context.MST_PRODUCT_STATUS
                            where s.DECISION != 1
                            orderby s.VIEWORDER
                            select new ProductStatusDto()
                            {
                                ID = s.ID,
                                NAME = orgType == ORGTYPE.Дэлгүүр ? s.STORENAME : s.NAME,
                                COLOR = s.COLOR
                            }).ToList();

            var groups = (from g in _context.MST_PRODUCT_REQUEST_GROUP
                          select g).ToList();

            foreach (var status in statuses)
            {
                foreach (var group in groups)
                {
                    var value = results.Where(a => a.GROUPID == group.ID && a.STATUSCODE == status.ID);
                    if (!value.Any()) continue;
                    status.SetValue(group.VIEWORDER, value.Sum(a => a.QTY));
                }
            }

            var dashboard = new
            {
                columns = groups,
                values = statuses
            };

            return ReturnResponce.ListReturnResponce(dashboard);
        }

        public static ResponseClient GetProductPerformance(OracleDbContext _context, ILogger _log,
            ORGTYPE orgType, int orgId, int userId, int departmentId, DateTime beginDate)
        {
            var departments = GetDepartmentIds(_context, _log, orgType, userId, departmentId);

            var requests = (from pl in _context.REQ_PRODUCT_LOG
                            join p in _context.REQ_PRODUCT on pl.HEADERID equals p.ID
                            join r in _context.MST_PRODUCT_REQUEST on p.REQUESTID equals r.ID
                            join sd in _context.MST_PRODUCT_STATUS_DETAIL on p.STATUS equals sd.ID
                            join s in _context.MST_PRODUCT_STATUS on sd.STATUS equals s.ID
                            join d in departments on p.DEPARTMENTID equals d
                            where pl.TYPE == RequestLogType.Created && beginDate <= pl.ACTIONDATE
                                && orgType == ORGTYPE.Дэлгүүр ? p.STOREID == UsefulHelpers.STORE_ID : p.ORGID == orgId
                            group p by new { REQUESTGROUPID = r.GROUPID, STATUSGROUPID = s.GROUPID } into g
                            select new
                            {
                                STATUSGROUPID = g.Key.STATUSGROUPID,
                                REQUESTGROUPID = g.Key.REQUESTGROUPID,
                                QTY = g.Count()
                            }).ToList();

            if (orgType == ORGTYPE.Дэлгүүр)
            {
                var orgRequests = (from pl in _context.REQ_PRODUCT_ORG_LOG
                                   join p in _context.REQ_PRODUCT_ORG on pl.HEADERID equals p.ID
                                   join r in _context.MST_PRODUCT_REQUEST on p.REQUESTID equals r.ID
                                   join sd in _context.MST_PRODUCT_STATUS_DETAIL on p.STATUS equals sd.ID
                                   join s in _context.MST_PRODUCT_STATUS on sd.STATUS equals s.ID
                                   join d in departments on p.DEPARTMENTID equals d
                                   where pl.TYPE == RequestLogType.Created && beginDate <= pl.ACTIONDATE
                                   group p by new { REQUESTGROUPID = r.GROUPID, STATUSGROUPID = s.GROUPID } into g
                                   select new
                                   {
                                       STATUSGROUPID = g.Key.STATUSGROUPID,
                                       REQUESTGROUPID = g.Key.REQUESTGROUPID,
                                       QTY = g.Count()
                                   }).ToList();

                if (orgRequests.Any())
                    requests.AddRange(orgRequests);
            }

            var statuses = (from s in _context.MST_PRODUCT_STATUS_GROUP
                            orderby s.VIEWORDER
                            select new ProductStatusDto()
                            {
                                ID = s.ID.ToString(),
                                NAME = s.NAME,
                                COLOR = s.COLOR
                            }).ToList();

            var groups = (from g in _context.MST_PRODUCT_REQUEST_GROUP
                          select g).ToList();

            foreach (var status in statuses)
            {
                foreach (var group in groups)
                {
                    var value = requests.Where(a => a.STATUSGROUPID.ToString() == status.ID && (a.REQUESTGROUPID == group.ID));
                    if (!value.Any()) continue;
                    status.SetValue(group.VIEWORDER, value.Sum(a => a.QTY));
                }
            }

            var dashboard = new
            {
                columns = groups,
                values = statuses
            };

            return ReturnResponce.ListReturnResponce(dashboard);
        }

        public static void SendNewRequests(OracleDbContext _context, ILogger _log, DateTime today)
        {
            var todayInString = today.ToString("yyyyMMdd");
            var requests = (from l in _context.REQ_PRODUCT_LOG.ToList()
                            join h in _context.REQ_PRODUCT.ToList() on l.HEADERID equals h.ID
                            where l.TYPE == RequestLogType.Created && l.ORGTYPE == ORGTYPE.Бизнес
                                && UsefulHelpers.ConvertDatetimeToString(l.ACTIONDATE) == todayInString
                            //&& CheckDate(l, todayInString)
                            group h by new { h.REQUESTID, h.DEPARTMENTID } into g
                            select new
                            {
                                g.Key.REQUESTID,
                                g.Key.DEPARTMENTID,
                                QTY = g.Count()
                            }).ToList()
                            .Union
                           (from l in _context.REQ_PRODUCT_ORG_LOG.ToList()
                            join h in _context.REQ_PRODUCT_ORG.ToList() on l.HEADERID equals h.ID
                            where l.TYPE == RequestLogType.Created && l.ORGTYPE == ORGTYPE.Бизнес
                                 && UsefulHelpers.ConvertDatetimeToString(l.ACTIONDATE) == todayInString
                            //&& CheckDate(l, todayInString)
                            group h by new { h.REQUESTID, h.DEPARTMENTID } into g
                            select new
                            {
                                g.Key.REQUESTID,
                                g.Key.DEPARTMENTID,
                                QTY = g.Count()
                            }).ToList();

            var source = (from u in _context.SYSTEM_USER_DEPARTMENT.ToList()
                          join r in requests on u.DEPARTMENTID equals r.DEPARTMENTID
                          join p in _context.MST_PRODUCT_REQUEST.ToList() on r.REQUESTID equals p.ID
                          group r by new { u.USERID, p.NAME, r.REQUESTID } into g
                          select new
                          {
                              g.Key.USERID,
                              REQUEST = g.Key.NAME,
                              g.Key.REQUESTID,
                              QTY = g.Sum(a => a.QTY)
                          }).ToList();

            var users = from s in source
                        join u in _context.SYSTEM_USERS.ToList() on s.USERID equals u.ID
                        group u by new { u.ID, u.USERMAIL } into g
                        select new
                        {
                            g.Key.ID,
                            g.Key.USERMAIL
                        };

            var content = SystemResource.MailResource.MasterRequest;

            var li = "<li>#type " +
                        "<ul> " +
                          "<li>#status - #count</li>" +
                        "</ul>" +
                      "</li> ";
            var builder = new StringBuilder();
            var total = 0;

            foreach (var user in users)
            {
                var userRequests = source.Where(a => a.USERID == user.ID).Select(a => a);
                if (!userRequests.Any()) continue;

                builder.Clear();
                total = 0;

                foreach (var req in userRequests)
                {
                    total += req.QTY;
                    builder.AppendLine(li.Replace("#type", req.REQUEST).Replace("#status", "Илгээсэн").Replace("#count", req.QTY.ToString()));
                }

                var mail = new EmailMessage()
                {
                    Type = MessageType.MasterRequest,
                    ToAddress = user.USERMAIL,
                    FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL,
                    Subject = $"Танд {total} хүсэлт ирсэн байна.",
                    Content = content.Replace("#email", user.USERMAIL).Replace("#list", $"<ul>{builder.ToString()}</ul>"),
                    Priority = (int)MessageType.MasterRequest,
                    StoreId = UsefulHelpers.STORE_ID,
                    UserEmail = EmailConfiguration.URTO_OPERATOR_MAIL
                };

                MailLogic.Add(_context, mail);
            }
        }

        public static ResponseClient GetNotes<T, G>(OracleDbContext _context, ILogger _log, int requestId, int orgType)
            where T : IProductRequest
            where G : IProductLog
        {
            try
            {
                var requestType = GetRequestType<T>();
                var product = GetRequest(_context, _log, requestType, requestId);

                if (product == null)
                    return ReturnResponce.NotFoundResponce();

                var details = requestType == MasterRequestType.Product ?
                    GetProductLogs(_context, _log, product.ID, orgType) :
                    requestType == MasterRequestType.ProductOrg ?
                    GetOrgLogs(_context, _log, product.ID, orgType) :
                    GetFeedbackLogs(_context, _log, product.ID, orgType);

                return ReturnResponce.ListReturnResponce(details);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        private static void NotificationSeen(OracleDbContext _context, ILogger _log,
            int id, int orgType, MasterRequestType requestType)
        {
            try
            {
                var notificationType = requestType == MasterRequestType.Product ? NotifcationType.Бараа :
                    requestType == MasterRequestType.ProductOrg ? NotifcationType.ШинэХарилцагч :
                    NotifcationType.Санал;

                var comId = orgType == (int)ORGTYPE.Бизнес ? (int)ORGTYPE.Дэлгүүр : (int)ORGTYPE.Бизнес;
                var notifications = _context.SYSTEM_NOTIFCATION_DATA
                                    .Where(a => a.RECORDID == id && a.NOTIFMODULETYPE == (int)notificationType && a.ISSEEN == 0 && a.COMID == comId);

                if (!notifications.Any()) return;

                foreach (var notif in notifications)
                {
                    notif.ISSEEN = 1;
                    Update(_context, notif, false);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError($"MasterLogic.NotificationSeen : {UsefulHelpers.GetExceptionMessage(ex)}");
            }
        }

        public static ResponseClient AddNote<T, G>(OracleDbContext _context, ILogger _log,
                                                    RequestNoteDto requestNote, int orgType,
                                                    NotifcationType notificationType, int userId)
            where T : IProductRequest
            where G : IProductLog
        {
            try
            {
                var requestType = GetRequestType<T>();
                var product = GetRequest(_context, _log, requestType, requestNote.Id);

                if (product == null)
                    return ReturnResponce.NotFoundResponce();
                
                var notification = new SYSTEM_NOTIFCATION_DATA()
                {
                    ID = Convert.ToString(Guid.NewGuid()),
                    STOREID = UsefulHelpers.STORE_ID,
                    CREATEDDATE = DateTime.Now,
                    ISSEEN = 0,
                    RECORDID = product.ID,
                    COMID = orgType,
                    NOTIFMODULETYPE = (int)notificationType
                };

                var newLog = GetNewLog<G>(_context, _log);
                newLog.HEADERID = requestNote.Id;
                newLog.USERID = userId;
                newLog.ORGTYPE = orgType == (int)ORGTYPE.Дэлгүүр ? ORGTYPE.Дэлгүүр : ORGTYPE.Бизнес;
                newLog.TYPE = RequestLogType.NoteAdded;
                newLog.STATUS = product.STATUS;
                newLog.ACTIONDATE = DateTime.Now;
                newLog.NOTE = requestNote.Note;

                //Update(_context, product, false);
                Insert(_context, notification, false);
                Insert(_context, newLog, false);
                _context.SaveChanges();

                // ХЭРВЭЭ ДЭЛГҮҮРЭЭС ХАРИЛЦАГЧ РҮҮ ТАЙЛБАР НЭМЖ БАЙВАЛ И-МЭЙЛ ИЛГЭЭНЭ
                if (newLog.ORGTYPE == ORGTYPE.Дэлгүүр)
                {
                    var email = GetEmail(_context, _log, product, requestType);

                    var url = ConfigData.GetCongifData(ConfigData.ConfigKey.EditCompanyURL)
                            .Replace("#id", newLog.ID.ToString())
                            .Replace("#email", email);

                    var user = ManagementLogic.GetUser(_context, userId);

                    var content = MailResource.MasterNote
                        .Replace("#email", email)
                        .Replace("#store", UsefulHelpers.STORE_NAME)
                        .Replace("#date", product.REQUESTDATE.ToString("yyyy/MM/dd"))
                        .Replace("#username", user == null ? "" : user.FIRSTNAME)
                        .Replace("#date", DateTime.Now.ToString("yyyy/MM/dd HH:mm"))
                        .Replace("#url", requestType == MasterRequestType.ProductOrg ? url : string.Empty)
                        .Replace("#comment", requestNote.Note);

                    var emailMessage = new EmailMessage()
                    {
                        Type = MessageType.MasterNote,
                        ToAddress = email,
                        FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL,
                        Subject = $"Таны хүсэлтэд {UsefulHelpers.STORE_NAME}аас хариу өгсөн байна.",
                        Content = content,
                        Priority = (int)MessageType.MasterNote,
                        StoreId = UsefulHelpers.STORE_ID,
                        UserEmail = EmailConfiguration.URTO_OPERATOR_MAIL
                    };

                    MailLogic.Add(_context, emailMessage);
                }
                else if (newLog.ORGTYPE == ORGTYPE.Бизнес)
                {
                    SendEmailsToStoreUsers(_context, _log, product, false);
                }

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static void CheckStatus(OracleDbContext _context, ILogger _log, int currentStatus)
        {
            var status = GetStatus(_context, _log, currentStatus);
            if (status.DECISION == 1)
                throw new Exception($"{status.NAME} төлөвтэй үед уг үйлдлийг хийх боломжгүй.");
        }

        private static IProductRequest GetRequest(OracleDbContext _context, ILogger _log, MasterRequestType requestType, int id)
        {
            return requestType == MasterRequestType.Product ?
                GetProductRequest(_context, _log, id) :
                requestType == MasterRequestType.ProductOrg ?
                GetOrgRequest(_context, _log, id) :
                GetFeedback(_context, _log, id);
        }

        public static ResponseClient AddImages<T, L, G>(OracleDbContext _context, ILogger _log,
                                                        RequestNoteDto requestNote, IList<IFormFile> files,
                                                        int orgType, NotifcationType notificationType, int userId)
            where T : IProductRequest
            where L : IProductImage
            where G : IProductLog
        {
            try
            {
                var requestType = GetRequestType<T>();
                var product = GetRequest(_context, _log, requestType, requestNote.Id);

                if (product == null)
                    return ReturnResponce.NotFoundResponce();

                CheckStatus(_context, _log, product.STATUS);

                var restUtils = new HttpRestUtils(product.STOREID, _context);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server.");

                if (product.ATTACHMENT != 1)
                {
                    product.ATTACHMENT = 1;
                    Update(_context, product, false);
                }

                var notification = new SYSTEM_NOTIFCATION_DATA()
                {
                    ID = Convert.ToString(Guid.NewGuid()),
                    STOREID = UsefulHelpers.STORE_ID,
                    CREATEDDATE = DateTime.Now,
                    ISSEEN = 0,
                    RECORDID = product.ID,
                    COMID = orgType,
                    NOTIFMODULETYPE = (int)notificationType
                };

                Insert(_context, notification, false);

                var newLog = GetNewLog<G>(_context, _log);
                newLog.HEADERID = product.ID;
                newLog.USERID = userId;
                newLog.NOTE = requestNote.Note;
                newLog.ORGTYPE = ORGTYPE.Бизнес;
                newLog.TYPE = RequestLogType.NoteAdded;
                newLog.STATUS = product.STATUS;
                newLog.ACTIONDATE = DateTime.Now;

                Insert(_context, newLog, false);

                foreach (var file in files)
                {
                    var type = UsefulHelpers.GetImageType(file.FileName);
                    var fileType = UsefulHelpers.GetFileType(file);
                    if (fileType == FileType.None || fileType == FileType.Forbidden)
                        return ReturnResponce.FailedMessageResponce("Ийм өргөтгөлтэй файл оруулах боломжгүй.");

                    var attachResponse = restUtils.Post($"/api/attachment/{(fileType == FileType.Image ? "image" : "file")}", file).Result;
                    if (!attachResponse.Success)
                        return attachResponse;

                    var newImage = GetNewImage<L>(_context, _log);
                    newImage.HEADERID = requestNote.Id;
                    newImage.LOGID = newLog.ID;
                    newImage.IMAGETYPE = (int)type;
                    newImage.SEEN = 0;
                    newImage.URL = attachResponse.Value.ToString();
                    newImage.VIEWORDER = 0;
                    newImage.ENABLED = 1;
                    newImage.CREATEDDATE = DateTime.Now;

                    Insert(_context, newImage, false);
                }

                _context.SaveChanges();

                if (newLog.ORGTYPE == ORGTYPE.Бизнес)
                    SendEmailsToStoreUsers(_context, _log, product, false);

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static ResponseClient DeleteImage<T, L>(OracleDbContext _context, ILogger _log, int id, int imageId, int orgId, int userId)
            where T : IProductRequest
            where L : IProductImage
        {
            try
            {
                var productRequest = typeof(T) == typeof(REQ_PRODUCT);
                var product = productRequest ?
                    GetProductRequest(_context, _log, id) :
                    GetOrgRequest(_context, _log, id);

                if (product == null)
                    return ReturnResponce.NotFoundResponce();

                CheckStatus(_context, _log, product.STATUS);

                //if (product.ORGID != orgId)
                //    return ReturnResponce.AccessDeniedResponce();

                var image = productRequest ?
                    GetProductRequestImage(_context, _log, imageId) :
                    GetOrgRequestImage(_context, _log, imageId);

                if (image == null)
                    return ReturnResponce.NotFoundResponce();

                if (image.HEADERID != id)
                    return ReturnResponce.AccessDeniedResponce();

                image.ENABLED = 0;
                image.SEENBY = userId;
                image.SEENDATE = DateTime.Now;

                Update(_context, image);

                return ReturnResponce.SuccessMessageResponce("");
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static MasterRequestType GetRequestType<T>()
            where T : IProductRequest
        {
            var type = MasterRequestType.Product;

            if (typeof(T) == typeof(REQ_PRODUCT))
                type = MasterRequestType.Product;
            else if (typeof(T) == typeof(REQ_PRODUCT_ORG))
                type = MasterRequestType.ProductOrg;
            else if (typeof(T) == typeof(REQ_FEEDBACK))
                type = MasterRequestType.Feedback;

            return type;
        }

        private static void Received<T, G>(OracleDbContext _context, ILogger _log, IProductRequest product,
                MST_PRODUCT_STATUS_DETAIL firstStatus, int userId)
            where T : IProductRequest
            where G : IProductLog
        {
            try
            {
                var nextStatus = GetNextStatus(_context, product.REQUESTID, firstStatus.VIEWORDER);
                ChangeStatus<T, G>(_context, _log, product, string.Empty, userId, nextStatus);
            }
            catch (Exception ex)
            {
                _log.LogError($"{UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod())} => {UsefulHelpers.GetExceptionMessage(ex)}");
            }
        }

        public static ResponseClient GetRequest<T, G>(OracleDbContext _context, ILogger _log, int id, int orgType, int userId)
            where T : IProductRequest
            where G : IProductLog
        {
            try
            {
                var requestType = GetRequestType<T>();
                var product = GetRequest(_context, _log, requestType, id);

                if (product == null)
                    return ReturnResponce.NotFoundResponce();

                var details = requestType == MasterRequestType.Product ?
                    GetProductImages(_context, _log, product.ID, orgType == (int)ORGTYPE.Дэлгүүр ? 1 : 2) :
                    requestType == MasterRequestType.ProductOrg ?
                    GetOrgImages(_context, _log, product.ID, orgType == (int)ORGTYPE.Дэлгүүр ? 1 : 2) :
                    new List<ImageDto>();

                var firstStatus = GetFirstStatus(_context, product.REQUESTID);
                if (orgType == (int)ORGTYPE.Дэлгүүр && product.STATUS == firstStatus.ID)
                    // ХҮЛЭЭН АВСАН БОЛГОХ
                    Received<T, G>(_context, _log, product, firstStatus, userId);
                else
                    // ХАРСАН БОЛГОХ
                    NotificationSeen(_context, _log, product.ID, orgType, requestType);

                var logs = requestType == MasterRequestType.Product ?
                    GetProductLogs(_context, _log, product.ID, orgType) :
                    requestType == MasterRequestType.ProductOrg ?
                    GetOrgLogs(_context, _log, product.ID, orgType) :
                    GetFeedbackLogs(_context, _log, product.ID, orgType);

                var container = new
                {
                    Product = product,
                    Details = details,
                    Logs = logs
                };

                return ReturnResponce.ListReturnResponce(container);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        private static MST_PRODUCT_STATUS_DETAIL GetFirstStatus(OracleDbContext _context, int typeId)
        {
            var first = _context.MST_PRODUCT_STATUS_DETAIL
                .Where(a => a.TYPEID == typeId)
                .OrderBy(a => a.VIEWORDER)
                .FirstOrDefault();

            if (first == null)
                throw new Exception("Хүлээн авсан төлөв тодорхойгүй байна.");

            return first;
        }

        private static MST_PRODUCT_STATUS_DETAIL GetNextStatus(OracleDbContext _context, int typeId, int viewOrder)
        {
            var first = _context.MST_PRODUCT_STATUS_DETAIL
                .Where(a => a.TYPEID == typeId && a.VIEWORDER > viewOrder)
                .OrderBy(a => a.VIEWORDER)
                .FirstOrDefault();

            if (first == null)
                throw new Exception("Дараагийн төлөв тодорхойгүй байна.");

            return first;
        }

        public static ResponseClient AddRequest<T, L, G>(OracleDbContext _context, ILogger _log,
            T newObject, IList<IFormFile> files, int userId, string note = null)
            where T : IProductRequest
            where L : IProductImage
            where G : IProductLog
        {
            // ШИНЭ ХАРИЛЦАГЧИЙН ШАЛГУУР
            if (newObject.GetType() == typeof(REQ_PRODUCT_ORG))
            {
                var regNo = (newObject as REQ_PRODUCT_ORG).REGNO;

                var existing = from r in _context.REQ_PRODUCT_ORG
                               join s in _context.MST_PRODUCT_STATUS_DETAIL on r.STATUS equals s.ID
                               where r.REGNO == regNo && s.DECISION != 1
                               group r by r.REGNO into g
                               select new { QTY = g.Count() };

                if (existing.Any() && existing.First().QTY > 3)
                    return ReturnResponce.FailedMessageResponce("Таны шинэ харилцагчийн хүсэлт явуулах тоо хэтэрсэн байна.");
            }

            var restUtils = new HttpRestUtils(newObject.STOREID, _context);
            if (!restUtils.StoreServerConnected)
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

            newObject.ID = Convert.ToInt32(GetNewId(_context, typeof(T).Name));
            newObject.SEEN = 0;
            newObject.STATUS = GetFirstStatus(_context, newObject.REQUESTID).ID;
            newObject.ENABLED = 1;
            newObject.REQUESTDATE = DateTime.Now;

            // ХЭРВЭЭ ТАЙЛБАРТАЙ ХАДГАЛЖ БАЙВАЛ
            IProductLog logNote = null;
            if (!string.IsNullOrEmpty(note))
            {
                logNote = GetNewLog<G>(_context, _log);
                logNote.HEADERID = newObject.ID;
                logNote.USERID = userId;
                logNote.ORGTYPE = ORGTYPE.Бизнес;
                logNote.TYPE = RequestLogType.NoteAdded;
                logNote.STATUS = newObject.STATUS;
                logNote.NOTE = note;
                logNote.ACTIONDATE = DateTime.Now;

                Insert(_context, logNote, false);
            }

            var images = new List<IProductImage>();
            ResponseClient attachResponse = null;
            var index = 0;

            foreach (var file in files)
            {
                var type = UsefulHelpers.GetImageType(file.FileName);
                var fileType = UsefulHelpers.GetFileType(file);
                if (fileType == FileType.None || fileType == FileType.Forbidden)
                    return ReturnResponce.FailedMessageResponce($"{file.FileName} файлыг оруулах боломжгүй.");

                attachResponse = restUtils.Post($"/api/attachment/{(fileType == FileType.Image ? "image" : "file")}", file).Result;
                if (!attachResponse.Success)
                    return attachResponse;

                var newImage = GetNewImage<L>(_context, _log);
                newImage.HEADERID = newObject.ID;
                newImage.IMAGETYPE = (int)type;
                newImage.SEEN = 0;
                newImage.URL = attachResponse.Value.ToString();
                newImage.VIEWORDER = index;
                newImage.ENABLED = 1;
                newImage.CREATEDDATE = DateTime.Now;
                if (logNote != null)
                    newImage.LOGID = logNote.ID;

                images.Add(newImage);
                index++;
            }

            var newLog = GetNewLog<G>(_context, _log);
            newLog.HEADERID = newObject.ID;
            newLog.USERID = userId;
            newLog.ORGTYPE = ORGTYPE.Бизнес;
            newLog.TYPE = RequestLogType.Created;
            newLog.STATUS = newObject.STATUS;
            newLog.ACTIONDATE = DateTime.Now;

            Insert(_context, newObject, false);
            Insert(_context, newLog, false);

            foreach (var image in images)
                Insert(_context, image, false);

            _context.SaveChanges();

            // ТУХАЙН АНГИЛЛЫН ХАРИУЦСАН ХҮМҮҮС РҮҮ ИМЭЙЛ ИЛГЭЭХ
            SendEmailsToStoreUsers(_context, _log, newObject);

            return ReturnResponce.SaveSucessResponce();
        }

        private static void SendEmailsToStoreUsers(OracleDbContext _context, ILogger _logger, IProductRequest productRequest, bool newRequest = true)
        {
            try
            {
                if (productRequest == null || productRequest.DEPARTMENTID == 0) return;

                var organizationName = string.Empty;
                if (productRequest.GetType() == typeof(REQ_PRODUCT_ORG))
                {
                    organizationName = (productRequest as REQ_PRODUCT_ORG).ORGNAME;
                }
                else
                {
                    var orgId = productRequest is REQ_PRODUCT ?
                        (productRequest as REQ_PRODUCT).ORGID :
                        (productRequest as REQ_FEEDBACK).ORGID;

                    var currentOrg = ManagementLogic.GetOrganization(_context, orgId);
                    organizationName = currentOrg == null ? string.Empty : currentOrg.COMPANYNAME;
                }

                var request = _context.MST_PRODUCT_REQUEST.FirstOrDefault(a => a.ID == productRequest.REQUESTID);
                var template = MailResource.MasterRequestSingle
                    .Replace("#organization_name", organizationName)
                    .Replace("#type", request == null ? "" : request.NAME);

                var storeUsers = (from d in _context.SYSTEM_USER_DEPARTMENT
                                  join u in _context.SYSTEM_USERS on d.USERID equals u.ID
                                  where d.DEPARTMENTID == productRequest.DEPARTMENTID && u.COOPERATION == 1
                                  select new
                                  {
                                      u.USERMAIL,
                                      u.FIRSTNAME
                                  }).Distinct().ToList();

                storeUsers.ForEach(a => MailLogic.Add(_context, new EmailMessage
                {
                    Subject = newRequest ? "Танд шинэ хүсэлт ирсэн байна" : "Хүсэлтэнд тайлбар ирсэн байна",
                    Content = template.Replace("#name", a.FIRSTNAME),
                    ToAddress = a.USERMAIL,
                    StoreId = UsefulHelpers.STORE_ID,
                    Type = MessageType.MasterRequest,
                    Priority = (int)MessageType.MasterRequest,
                    UserEmail = a.USERMAIL,
                    FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError($"MASTERLOGIC.SENDEMAILSTOSTOREUSERS : {productRequest.ID} => {UsefulHelpers.GetExceptionMessage(ex)}");
            }
        }

        private static IProductLog GetNewLog<G>(OracleDbContext _context, ILogger _log)
        {
            var newLog = (IProductLog)Activator.CreateInstance(typeof(G));
            newLog.ID = Convert.ToInt32(GetNewId(_context, typeof(G).Name));
            return newLog;
        }

        private static IProductImage GetNewImage<L>(OracleDbContext _context, ILogger _log)
        {
            var newImage = (IProductImage)Activator.CreateInstance(typeof(L));
            newImage.ID = Convert.ToInt32(GetNewId(_context, typeof(L).Name));
            return newImage;
        }

        public static ResponseClient GetStatuses<T>(OracleDbContext _context, ILogger _log, int id)
            where T : IProductRequest
        {
            try
            {
                var requestType = GetRequestType<T>();
                var product = GetRequest(_context, _log, requestType, id);

                if (product == null)
                    return ReturnResponce.NotFoundResponce();

                var currentStatus = GetStatus(_context, _log, product.STATUS);
                // 2019-04-03 ХОСОО УБАГИЙН ТУШААЛ
                //if (currentStatus.DECISION == 1)
                //    return ReturnResponce.FailedMessageResponce($"{currentStatus.NAME} үед төлөв солих боломжгүй.");

                var statuses = (from s in _context.MST_PRODUCT_STATUS_DETAIL
                                join p in _context.MST_PRODUCT_STATUS on s.STATUS equals p.ID
                                where s.TYPEID == product.REQUESTID && s.ID != currentStatus.ID && p.CHOOSABLE == 1
                                // && (s.DECISION == 1 || s.VIEWORDER >= currentStatus.VIEWORDER)
                                orderby s.VIEWORDER
                                select new
                                {
                                    s.ID,
                                    s.NAME
                                }).ToList();

                if (!statuses.Any())
                    return ReturnResponce.NotFoundResponce();

                return ReturnResponce.ListReturnResponce(statuses);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static ResponseClient GetStatuses(OracleDbContext _context, ILogger _log, List<RequestModel> records)
        {
            try
            {
                var orgRecords = new List<int>();
                var otherRecords = new List<int>();
                foreach (var current in records)
                {
                    if (UsefulHelpers.IsNewOrgType(current.TYPE))
                        orgRecords.Add(current.ID);
                    else
                        otherRecords.Add(current.ID);
                }

                var omg = ((from r in _context.REQ_PRODUCT
                            join d in _context.MST_PRODUCT_STATUS_DETAIL on r.STATUS equals d.ID
                            join o in otherRecords on r.ID equals o
                            group d by new { d.STATUS } into g
                            select new { g.Key.STATUS })
                          .Union
                          (from r in _context.REQ_PRODUCT_ORG
                           join d in _context.MST_PRODUCT_STATUS_DETAIL on r.STATUS equals d.ID
                           join o in orgRecords on r.ID equals o
                           group d by new { d.STATUS } into g
                           select new { g.Key.STATUS })).Distinct().ToList();

                var statuses = (from s in _context.MST_PRODUCT_STATUS.ToList()
                                join p in omg on s.ID equals p.STATUS into lj
                                from l in lj.DefaultIfEmpty()
                                where s.CHOOSABLE == 1 && l == null
                                select new
                                {
                                    s.ID,
                                    s.NAME
                                }).ToList();

                if (!statuses.Any())
                    return ReturnResponce.NotFoundResponce();

                return ReturnResponce.ListReturnResponce(statuses);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static MST_PRODUCT_STATUS_DETAIL GetStatusId(OracleDbContext _context, ILogger _logger, string status, string type)
        {
            return (from r in _context.MST_PRODUCT_REQUEST
                    join s in _context.MST_PRODUCT_STATUS_DETAIL on r.ID equals s.TYPEID
                    where r.CODE == type && s.STATUS == status
                    select s).FirstOrDefault();
        }

        public static void ChangeStatus<T, G>(OracleDbContext _context, ILogger _log, 
            int id, string note, int userId, MST_PRODUCT_STATUS_DETAIL status)
            where T : IProductRequest
            where G : IProductLog
        {
            var requestType = GetRequestType<T>();
            var found = GetRequest(_context, _log, requestType, id);

            if (found == null)
                throw new Exception("Өгөгдөл олдсонгүй.");

            ChangeStatus<T, G>(_context, _log, found, note, userId, status);
        }

        private static string GetEmail(OracleDbContext _context, ILogger _log, IProductRequest request, MasterRequestType type)
        {
            return type == MasterRequestType.Product ?
                        ManagementLogic.GetUser(_context, ((REQ_PRODUCT)request).REQUESTBY).USERMAIL :
                        type == MasterRequestType.ProductOrg ?
                        ((REQ_PRODUCT_ORG)request).REQUESTBY :
                        ManagementLogic.GetUser(_context, ((REQ_FEEDBACK)request).REQUESTBY).USERMAIL;
        }

        private static MST_PRODUCT_STATUS GetProductStatus(OracleDbContext _context, ILogger _logger, string status)
        {
            return _context.MST_PRODUCT_STATUS.FirstOrDefault(a => a.ID == status);
        }

        public static void ChangeStatus<T, G>(OracleDbContext _context, ILogger _log,
            IProductRequest request, string note, int userId, 
            MST_PRODUCT_STATUS_DETAIL status)
            where T : IProductRequest
            where G : IProductLog
        {
            var type = GetRequestType<T>();
            if (request.STATUS == status.ID)
                throw new Exception("Төлвөө шалгаад дахин оролдоно уу.");

            var productStatus = GetProductStatus(_context, _log, status.STATUS);
            if (productStatus == null)
                throw new Exception("Төлвийн мэдээлэл тодорхойгүй байна.");

            request.STATUS = status.ID;
            request.NOTE = note;
            if (!request.RECEIVEDBY.HasValue)
            {
                request.RECEIVEDBY = userId;
                request.RECEIVEDDATE = DateTime.Now;
            }

            if (status.DECISION == 1)
            {
                request.CONFIRMEDBY = userId;
                request.CONFIRMEDDATE = DateTime.Now;
            }
            else
            {
                request.CONFIRMEDBY = null;
                request.CONFIRMEDDATE = null;
            }

            if (status.DURATION > 0)
                request.EXPIREDATE = DateTime.Now.AddDays(status.DURATION);
            else
                request.EXPIREDATE = null;

            var newNotification = new SYSTEM_NOTIFCATION_DATA()
            {
                ID = Convert.ToString(Guid.NewGuid()),
                STOREID = UsefulHelpers.STORE_ID,
                CREATEDDATE = DateTime.Now,
                ISSEEN = 0,
                RECORDID = request.ID,
                COMID = (int) ORGTYPE.Дэлгүүр,
                NOTIFMODULETYPE = (int) (type == MasterRequestType.Product ? NotifcationType.Бараа :
                    type == MasterRequestType.ProductOrg ? NotifcationType.ШинэХарилцагч :
                    NotifcationType.Санал)
            };

            var newLog = GetNewLog<G>(_context, _log);
            newLog.HEADERID = request.ID;
            newLog.USERID = userId;
            newLog.ORGTYPE = ORGTYPE.Дэлгүүр;
            newLog.TYPE = RequestLogType.StatusChanged;
            newLog.STATUS = request.STATUS;
            newLog.ACTIONDATE = DateTime.Now;
            newLog.NOTE = note;

            Update(_context, request, false);
            Insert(_context, newLog, false);
            Insert(_context, newNotification, false);

            _context.SaveChanges();

            var email = GetEmail(_context, _log, request, type);

            var requestType = _context.MST_PRODUCT_REQUEST.FirstOrDefault(a => a.ID == request.REQUESTID);
            var content = MailResource.MasterStatus
                .Replace("#email", email)
                .Replace("#store", UsefulHelpers.STORE_NAME)
                .Replace("#date", request.REQUESTDATE.ToString("yyyy/MM/dd"))
                .Replace("#type", requestType.NAME)
                .Replace("#status", status.NAME)
                .Replace("#note", note);

            var emailMessage = new EmailMessage()
            {
                Type = MessageType.MasterStatus,
                ToAddress = email,
                FromAddress = EmailConfiguration.URTO_OPERATOR_MAIL,
                Subject = $"Таны {requestType.NAME}-н хүсэлтийг {productStatus.ACTIONNAME}.",
                Content = content,
                Priority = (int)MessageType.MasterStatus,
                StoreId = UsefulHelpers.STORE_ID,
                UserEmail = EmailConfiguration.URTO_OPERATOR_MAIL
            };

            MailLogic.Add(_context, emailMessage);
        }

        public static MST_PRODUCT_STATUS_DETAIL GetStatus(OracleDbContext _context, ILogger _log, int id)
        {
            return _context.MST_PRODUCT_STATUS_DETAIL.FirstOrDefault(a => a.ID == id);
        }

        public static IProductRequest GetProductRequest(OracleDbContext _context, ILogger _log, int id)
        {
            return _context.REQ_PRODUCT.FirstOrDefault(p => p.ID == id);
        }

        public static IProductRequest GetOrgRequest(OracleDbContext _context, ILogger _log, int id)
        {
            return _context.REQ_PRODUCT_ORG.FirstOrDefault(p => p.ID == id);
        }

        public static IProductRequest GetFeedback(OracleDbContext _context, ILogger _log, int id)
        {
            return _context.REQ_FEEDBACK.FirstOrDefault(p => p.ID == id);
        }

        public static IProductImage GetProductRequestImage(OracleDbContext _context, ILogger _log, int id)
        {
            return _context.REQ_PRODUCT_IMAGE.FirstOrDefault(p => p.ID == id);
        }
        public static IProductImage GetOrgRequestImage(OracleDbContext _context, ILogger _log, int id)
        {
            return _context.REQ_PRODUCT_ORG_IMAGE.FirstOrDefault(p => p.ID == id);
        }

        public static List<ImageDto> GetProductImages(OracleDbContext _context, ILogger _log, int id, int state)
        {
            return (from i in _context.REQ_PRODUCT_IMAGE
                    where i.HEADERID == id && i.ENABLED == 1
                    orderby i.VIEWORDER
                    select new ImageDto()
                    {
                        ID = i.ID,
                        IMAGETYPE = i.IMAGETYPE,
                        URL = i.URL,
                        STATE = state
                    }).ToList();
        }

        public static List<ImageDto> GetOrgImages(OracleDbContext _context, ILogger _log, int id, int state)
        {
            return (from i in _context.REQ_PRODUCT_ORG_IMAGE
                    where i.HEADERID == id && i.ENABLED == 1
                    orderby i.VIEWORDER
                    select new ImageDto()
                    {
                        ID = i.ID,
                        IMAGETYPE = i.IMAGETYPE,
                        URL = i.URL,
                        STATE = state
                    }).ToList();
        }

        public static List<ProductRequestLog> GetProductLogs(OracleDbContext _context, ILogger _log, int id, int orgType)
        {
            return (from i in _context.REQ_PRODUCT_LOG.ToList()
                    join u in _context.SYSTEM_USERS.ToList() on i.USERID equals u.ID
                    join s in _context.MST_PRODUCT_STATUS_DETAIL.ToList() on i.STATUS equals s.ID into lj
                    from l in lj.DefaultIfEmpty()
                    where i.HEADERID == id && (i.TYPE == RequestLogType.NoteAdded || i.TYPE == RequestLogType.StatusChanged)
                    let images = _context.REQ_PRODUCT_IMAGE.Where(a => a.LOGID == i.ID && a.ENABLED == 1).Select(a => new ImageDto() { ID = a.ID, URL = a.URL }).ToList()
                    orderby i.ACTIONDATE
                    select new ProductRequestLog()
                    {
                        USERID = i.USERID,
                        USERNAME = u.FIRSTNAME,
                        IMAGEURL = u.USERPIC,
                        ACTIONDATE = i.ACTIONDATE,
                        Note = i.TYPE == RequestLogType.Created ? "Үүсгэсэн" : 
                            i.TYPE == RequestLogType.Edited ? "Засвар хийсэн" :
                            i.TYPE == RequestLogType.StatusChanged ? (l == null ? "Төлөв сольсон" : l.NAME) + (orgType == (int) ORGTYPE.Бизнес || string.IsNullOrEmpty(i.NOTE) ? string.Empty : (" : " + i.NOTE)) :
                            i.TYPE == RequestLogType.NoteAdded ? i.NOTE : string.Empty,
                        Type = i.TYPE,
                        Images = images
                    }).ToList();
        }

        public static List<ProductRequestLog> GetFeedbackLogs(OracleDbContext _context, ILogger _log, int id, int orgType)
        {
            return (from i in _context.REQ_FEEDBACK_LOG.ToList()
                    join u in _context.SYSTEM_USERS.ToList() on i.USERID equals u.ID
                    join s in _context.MST_PRODUCT_STATUS_DETAIL.ToList() on i.STATUS equals s.ID into lj
                    from l in lj.DefaultIfEmpty()
                    where i.HEADERID == id && (i.TYPE == RequestLogType.NoteAdded || i.TYPE == RequestLogType.StatusChanged)
                    //let images = _context.REQ_PRODUCT_IMAGE.Where(a => a.LOGID == i.ID && a.ENABLED == 1).Select(a => new ImageDto() { ID = a.ID, URL = a.URL }).ToList()
                    orderby i.ACTIONDATE
                    select new ProductRequestLog()
                    {
                        USERID = i.USERID,
                        USERNAME = u.FIRSTNAME,
                        IMAGEURL = u.USERPIC,
                        ACTIONDATE = i.ACTIONDATE,
                        Note = i.TYPE == RequestLogType.Created ? "Үүсгэсэн" :
                            i.TYPE == RequestLogType.Edited ? "Засвар хийсэн" :
                            i.TYPE == RequestLogType.StatusChanged ? (l == null ? "Төлөв сольсон" : l.NAME) + (orgType == (int)ORGTYPE.Бизнес || string.IsNullOrEmpty(i.NOTE) ? string.Empty : (" : " + i.NOTE)) :
                            i.TYPE == RequestLogType.NoteAdded ? i.NOTE : string.Empty,
                        Type = i.TYPE,
                        Images = new List<ImageDto>()
                    }).ToList();
        }

        public static List<ProductRequestLog> GetOrgLogs(OracleDbContext _context, ILogger _log, int id, int orgType)
        {
            var request = (REQ_PRODUCT_ORG) GetRequest(_context, _log, MasterRequestType.ProductOrg, id);
            return (from i in _context.REQ_PRODUCT_ORG_LOG.ToList()

                    join u in _context.SYSTEM_USERS.ToList() on i.USERID equals u.ID into lj
                    from l in lj.DefaultIfEmpty()

                    join s in _context.MST_PRODUCT_STATUS_DETAIL.ToList() on i.STATUS equals s.ID into llj
                    from ll in llj.DefaultIfEmpty()

                    where i.HEADERID == id && (i.TYPE == RequestLogType.NoteAdded || i.TYPE == RequestLogType.StatusChanged)
                    let images = _context.REQ_PRODUCT_ORG_IMAGE.Where(a => a.LOGID == i.ID && a.ENABLED == 1).Select(a => new ImageDto() { ID = a.ID, URL = a.URL }).ToList()
                    orderby i.ACTIONDATE
                    select new ProductRequestLog()
                    {
                        USERID = i.USERID,
                        USERNAME = l == null ? request.ORGNAME : l.FIRSTNAME,
                        IMAGEURL = l == null ? UsefulHelpers.NO_IMAGE_URL : l.USERPIC,
                        ACTIONDATE = i.ACTIONDATE,
                        Note = i.TYPE == RequestLogType.Created ? "Үүсгэсэн" :
                            i.TYPE == RequestLogType.Edited ? "Засвар хийсэн" :
                            i.TYPE == RequestLogType.StatusChanged ? (ll == null ? "Төлөв сольсон" : ll.NAME) + (orgType == (int)ORGTYPE.Бизнес || string.IsNullOrEmpty(i.NOTE) ? string.Empty : (" : " + i.NOTE)) :
                            i.TYPE == RequestLogType.NoteAdded ? i.NOTE : string.Empty,
                        Type = i.TYPE,
                        Images = images
                    }).ToList();
        }

        public static MST_PRODUCT GetProduct(OracleDbContext _context, ILogger _Log, int id)
        {
            return _context.MST_PRODUCT.FirstOrDefault(p => p.ID == id);
        }

        public static List<ProductStore> GetProductStores(OracleDbContext _context, ILogger _log, int productId)
        {
            var details = from s in _context.MST_PRODUCT_STORE
                          join o in _context.SYSTEM_ORGANIZATION on s.STOREID equals o.ID
                          where s.PRODUCTID == productId
                          select new ProductStore()
                          {
                              STOREID = s.STOREID,
                              STORENAME = o.COMPANYNAME,
                              CONTRACTNO = s.CONTRACTNO,
                              PRICE = s.PRICE,
                              ENABLED = s.ENABLED,
                              STOCK = s.STOCK
                          };

            return details.ToList();
        }

        public static FeedbackListModel GetStoreFeedbacks(OracleDbContext _context, ILogger _log, ProductRequestFilterView filter)
        {
            var P_ORGID = new OracleParameter("P_ORGID", OracleDbType.Int32, filter.ORGID, ParameterDirection.Input);
            var P_STOREID = new OracleParameter("P_STOREID", OracleDbType.Int32, filter.STOREID, ParameterDirection.Input);
            var P_TYPEID = new OracleParameter("P_TYPEID", OracleDbType.Int32, filter.DEPARTMENTID, ParameterDirection.Input);
            var P_STATUS = new OracleParameter("P_STATUS", OracleDbType.Varchar2, filter.STATUS, ParameterDirection.Input);

            var P_ORDER_COLUMN = new OracleParameter("P_ORDER_COLUMN", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var P_START_ROW = new OracleParameter("P_START_ROW", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var P_ROW_COUNT = new OracleParameter("P_ROW_COUNT", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);

            var RETURN_VALUE = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            int totalCount = _context.Database.SqlQuery<int>("BEGIN GET_STORE_FEEDBACKS_TOTAL("
             + ":P_ORGID, "
             + ":P_STOREID, "
             + ":P_TYPEID, "
             + ":P_STATUS, "
             + ":RETURN_VALUE); END; ",
             P_ORGID, P_STOREID, P_TYPEID, P_STATUS,
             RETURN_VALUE).Single();

            var values = _context.Database.SqlQuery<FeedbackListDto>("BEGIN GET_STORE_FEEDBACKS("
             + ":P_ORGID, "
             + ":P_STOREID, "
             + ":P_TYPEID, "
             + ":P_STATUS, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":RETURN_VALUE); END; ",
             P_ORGID, P_STOREID, P_TYPEID, P_STATUS,
             P_ORDER_COLUMN, P_START_ROW, P_ROW_COUNT,
             RETURN_VALUE).ToList();

            return new FeedbackListModel(totalCount, values);
        }

        public static FeedbackListModel GetFeedbacks(OracleDbContext _context, ILogger _log, ProductRequestFilterView filter)
        {
            var P_ORGID = new OracleParameter("P_ORGID", OracleDbType.Int32, filter.ORGID, ParameterDirection.Input);
            var P_STOREID = new OracleParameter("P_STOREID", OracleDbType.Int32, filter.STOREID, ParameterDirection.Input);
            var P_TYPEID = new OracleParameter("P_TYPEID", OracleDbType.Int32, filter.DEPARTMENTID, ParameterDirection.Input);
            var P_STATUS = new OracleParameter("P_STATUS", OracleDbType.Varchar2, filter.STATUS, ParameterDirection.Input);

            var P_ORDER_COLUMN = new OracleParameter("P_ORDER_COLUMN", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var P_START_ROW = new OracleParameter("P_START_ROW", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var P_ROW_COUNT = new OracleParameter("P_ROW_COUNT", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);

            var RETURN_VALUE = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            int totalCount = _context.Database.SqlQuery<int>("BEGIN GET_BUSINESS_FEEDBACKS_TOTAL("
             + ":P_ORGID, "
             + ":P_STOREID, "
             + ":P_TYPEID, "
             + ":P_STATUS, "
             + ":RETURN_VALUE); END; ",
             P_ORGID, P_STOREID, P_TYPEID, P_STATUS,
             RETURN_VALUE).Single();

            var values = _context.Database.SqlQuery<FeedbackListDto>("BEGIN GET_BUSINESS_FEEDBACKS("
             + ":P_ORGID, "
             + ":P_STOREID, "
             + ":P_TYPEID, "
             + ":P_STATUS, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":RETURN_VALUE); END; ",
             P_ORGID, P_STOREID, P_TYPEID, P_STATUS,
             P_ORDER_COLUMN, P_START_ROW, P_ROW_COUNT,
             RETURN_VALUE).ToList();

            return new FeedbackListModel(totalCount, values);
        }

        public static ProductRequestListModel GetStoreProductRequests(OracleDbContext _context, ILogger _log, ProductRequestFilterView filter)
        {
            var P_ORGID = new OracleParameter("P_ORGID", OracleDbType.Int32, filter.ORGID, ParameterDirection.Input);
            var P_REGNO = new OracleParameter("P_REGNO", OracleDbType.Varchar2, filter.REGNO, ParameterDirection.Input);
            var P_STOREID = new OracleParameter("P_STOREID", OracleDbType.Int32, filter.STOREID, ParameterDirection.Input);
            var P_USERID = new OracleParameter("P_USERID", OracleDbType.Int32, filter.USERID, ParameterDirection.Input);
            var P_REQUESTID = new OracleParameter("P_REQUESTID", OracleDbType.Int32, filter.REQUESTID, ParameterDirection.Input);
            var P_CONTRACTNO = new OracleParameter("P_CONTRACTNO", OracleDbType.Varchar2, filter.CONTRACTNO, ParameterDirection.Input);
            var P_DEPARTMENTID = new OracleParameter("P_DEPARTMENTID", OracleDbType.Int32, filter.DEPARTMENTID, ParameterDirection.Input);
            var P_STATUS = new OracleParameter("P_STATUS", OracleDbType.Varchar2, filter.STATUS, ParameterDirection.Input);
            var P_BEGIN_DATE = new OracleParameter("P_BEGIN_DATE", OracleDbType.Date, filter.BEGINDATE, ParameterDirection.Input);
            var P_END_DATE = new OracleParameter("P_END_DATE", OracleDbType.Date, filter.ENDDATE, ParameterDirection.Input);

            var P_ORDER_COLUMN = new OracleParameter("P_ORDER_COLUMN", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var P_START_ROW = new OracleParameter("P_START_ROW", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var P_ROW_COUNT = new OracleParameter("P_ROW_COUNT", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);

            var RETURN_VALUE = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            int totalCount = _context.Database.SqlQuery<int>("BEGIN GET_STORE_REQUESTS_TOTAL("
             + ":P_ORGID, "
             + ":P_REGNO, "
             + ":P_STOREID, "
             + ":P_USERID, "
             + ":P_REQUESTID, "
             + ":P_CONTRACTNO, "
             + ":P_DEPARTMENTID, "
             + ":P_STATUS, "
             + ":P_BEGIN_DATE, "
             + ":P_END_DATE, "
             + ":RETURN_VALUE); END; ",
             P_ORGID, P_REGNO, P_STOREID, P_USERID, P_REQUESTID, P_CONTRACTNO, P_DEPARTMENTID, P_STATUS, P_BEGIN_DATE, P_END_DATE,
             RETURN_VALUE).Single();

            var values = _context.Database.SqlQuery<ProductRequestDto>("BEGIN GET_STORE_REQUESTS("
             + ":P_ORGID, "
             + ":P_REGNO, "
             + ":P_STOREID, "
             + ":P_USERID, "
             + ":P_REQUESTID, "
             + ":P_CONTRACTNO, "
             + ":P_DEPARTMENTID, "
             + ":P_STATUS, "
             + ":P_BEGIN_DATE, "
             + ":P_END_DATE, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":RETURN_VALUE); END; ",
             P_ORGID, P_REGNO, P_STOREID, P_USERID, P_REQUESTID, P_CONTRACTNO, P_DEPARTMENTID, P_STATUS, P_BEGIN_DATE, P_END_DATE,
             P_ORDER_COLUMN, P_START_ROW, P_ROW_COUNT,
             RETURN_VALUE).ToList();

            return new ProductRequestListModel(totalCount, values);
        }

        public static ProductRequestListModel GetProductRequests(OracleDbContext _context, ILogger _log, ProductRequestFilterView filter)
        {
            var P_ORGID = new OracleParameter("P_ORGID", OracleDbType.Int32, filter.ORGID, ParameterDirection.Input);
            var P_STOREID = new OracleParameter("P_STOREID", OracleDbType.Int32, filter.STOREID, ParameterDirection.Input);
            var P_REQUESTID = new OracleParameter("P_REQUESTID", OracleDbType.Int32, filter.REQUESTID, ParameterDirection.Input);
            var P_CONTRACTNO = new OracleParameter("P_CONTRACTNO", OracleDbType.Varchar2, filter.CONTRACTNO, ParameterDirection.Input);
            var P_DEPARTMENTID = new OracleParameter("P_DEPARTMENTID", OracleDbType.Int32, filter.DEPARTMENTID, ParameterDirection.Input);
            var P_STATUS = new OracleParameter("P_STATUS", OracleDbType.Varchar2, filter.STATUS, ParameterDirection.Input);
            var P_BEGIN_DATE = new OracleParameter("P_BEGIN_DATE", OracleDbType.Date, filter.BEGINDATE, ParameterDirection.Input);
            var P_END_DATE = new OracleParameter("P_END_DATE", OracleDbType.Date, filter.ENDDATE, ParameterDirection.Input);

            var P_ORDER_COLUMN = new OracleParameter("P_ORDER_COLUMN", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var P_START_ROW = new OracleParameter("P_START_ROW", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var P_ROW_COUNT = new OracleParameter("P_ROW_COUNT", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);

            var RETURN_VALUE = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            int totalCount = _context.Database.SqlQuery<int>("BEGIN GET_BUSINESS_REQUESTS_TOTAL("
             + ":P_ORGID, "
             + ":P_STOREID, "
             + ":P_REQUESTID, "
             + ":P_CONTRACTNO, "
             + ":P_DEPARTMENTID, "
             + ":P_STATUS, "
             + ":P_BEGIN_DATE, "
             + ":P_END_DATE, "
             + ":RETURN_VALUE); END; ",
             P_ORGID, P_STOREID, P_REQUESTID, P_CONTRACTNO, P_DEPARTMENTID, P_STATUS, P_BEGIN_DATE, P_END_DATE,
             RETURN_VALUE).Single();

            var values = _context.Database.SqlQuery<ProductRequestDto>("BEGIN GET_BUSINESS_REQUESTS("
             + ":P_ORGID, "
             + ":P_STOREID, "
             + ":P_REQUESTID, "
             + ":P_CONTRACTNO, "
             + ":P_DEPARTMENTID, "
             + ":P_STATUS, "
             + ":P_BEGIN_DATE, "
             + ":P_END_DATE, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":RETURN_VALUE); END; ",
             P_ORGID, P_STOREID, P_REQUESTID, P_CONTRACTNO, P_DEPARTMENTID, P_STATUS, P_BEGIN_DATE, P_END_DATE,
             P_ORDER_COLUMN, P_START_ROW, P_ROW_COUNT,
             RETURN_VALUE).ToList();

            return new ProductRequestListModel(totalCount, values);
        }

        public static ProductListModel GetProducts(OracleDbContext _context, ILogger _log, ProductFilterView filter)
        {
            var P_ORGID = new OracleParameter("P_ORGID", OracleDbType.Int32, filter.ORGID, ParameterDirection.Input);
            var P_BARCODE = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, filter.BARCODE, ParameterDirection.Input);
            var P_NAME = new OracleParameter("P_NAME", OracleDbType.Varchar2, filter.NAME, ParameterDirection.Input);
            var P_STORENAME = new OracleParameter("P_STORENAME", OracleDbType.Varchar2, filter.STORENAME, ParameterDirection.Input);
            var P_BRANDNAME = new OracleParameter("P_BRANDNAME", OracleDbType.Varchar2, filter.BRANDNAME, ParameterDirection.Input);
            var P_MEASUREID = new OracleParameter("P_MEASUREID", OracleDbType.Int32, filter.MEASUREID, ParameterDirection.Input);
            var P_DEPARTMENTID = new OracleParameter("P_DEPARTMENTID", OracleDbType.Int32, filter.DEPARTMENTID, ParameterDirection.Input);

            var P_ORDER_COLUMN = new OracleParameter("P_ORDER_COLUMN", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var P_START_ROW = new OracleParameter("P_START_ROW", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var P_ROW_COUNT = new OracleParameter("P_ROW_COUNT", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);
            
            var RETURN_VALUE = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            int totalCount = _context.Database.SqlQuery<int>("BEGIN GET_PRODUCTS_TOTAL("
             + ":P_ORGID, "
             + ":P_BARCODE, "
             + ":P_NAME, "
             + ":P_STORENAME, "
             + ":P_BRANDNAME, "
             + ":P_MEASUREID, "
             + ":P_DEPARTMENTID, "
             + ":RETURN_VALUE); END; ",
             P_ORGID, P_BARCODE, P_NAME, P_STORENAME, P_BRANDNAME, P_MEASUREID, P_DEPARTMENTID,
             RETURN_VALUE).Single();

            var values = _context.Database.SqlQuery<ProductDto>("BEGIN GET_PRODUCTS("
             + ":P_ORGID, "
             + ":P_BARCODE, "
             + ":P_NAME, "
             + ":P_STORENAME, "
             + ":P_BRANDNAME, "
             + ":P_MEASUREID, "
             + ":P_DEPARTMENTID, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":RETURN_VALUE); END; ",
             P_ORGID, P_BARCODE, P_NAME, P_STORENAME, P_BRANDNAME, P_MEASUREID, P_DEPARTMENTID,
             P_ORDER_COLUMN, P_START_ROW, P_ROW_COUNT,
             RETURN_VALUE).ToList();

            return new ProductListModel(totalCount, values);
        }
    }
}
