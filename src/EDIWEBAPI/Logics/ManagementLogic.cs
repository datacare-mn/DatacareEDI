using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Entities.DBModel.Order;
using EDIWEBAPI.Context;
using EDIWEBAPI.Utils;
using static EDIWEBAPI.Enums.SystemEnums;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using EDIWEBAPI.Entities.ResultModels;
using Oracle.ManagedDataAccess.Client;
using EDIWEBAPI.Entities.FilterViews;
using System.Data;
using EDIWEBAPI.Entities.CustomModels;

namespace EDIWEBAPI.Logics
{
    public class ManagementLogic : BaseLogic
    {
        public static HttpRequestModel GetHttpModelData(OracleDbContext _context, int comid, string appName = "STORE")
        {
            var app = GetAppUser(_context, appName, comid);
            return app == null ?
                null :
                new HttpRequestModel()
                {
                    AppName = app.APPNAME,
                    CompanyID = comid,
                    BaseApi = app.APIADDRESS,
                    UserName = app.APIUSER,
                    Password = app.APIPASS,
                    ExpiresIn = app.APIEXPRIREIN,
                    ExpireTime = app.APIEXPIRETIME,
                    Token = app.APITOKEN
                };
        }

        public static bool UpdateAppToken(OracleDbContext _context, int comid, TokenValue token, string appName = "STORE")
        {
            var app = GetAppUser(_context, appName, comid);
            if (app != null)
            {
                app.APITOKEN = token.Token;
                app.APIEXPRIREIN = token.ExpiresIn;
                app.APIEXPIRETIME = token.ExpireTime;
                Update(_context, app);
            }
            return app != null;
        }

        public static Entities.RequestModels.OrganizationUserRequest SearchOrganization(OracleDbContext _context, ILogger _log, string registryNo)
        {
            var organization = GetOrganization(_context, registryNo);
            if (organization != null)
                return new Entities.RequestModels.OrganizationUserRequest()
                {
                    ORGID = organization.ID,
                    REGNO = organization.REGNO,
                    COMPANYNAME = organization.COMPANYNAME,
                    CEONAME = organization.CEONAME,
                    ADDRESS = organization.ADDRESS,
                    WEBSITE = organization.WEBSITE
                };

            var info = UsefulHelpers.GetCompanyInfo(_log, registryNo);
            return info == null ? null : new Entities.RequestModels.OrganizationUserRequest()
            {
                ORGID = 0,
                REGNO = registryNo,
                COMPANYNAME = info.Name
            };
        }

        public static SYSTEM_ORGANIZATION AddOrganization(OracleDbContext _context, ILogger _log, ReciveOrder order)
        {
            var organization = new SYSTEM_ORGANIZATION()
            {
                REGNO = order.RegNo,
                COMPANYNAME = order.Name,
                ENABLED = 1,
                ORGTYPE = ORGTYPE.Бизнес
            };
            try
            {
                organization.ID = Convert.ToInt16(GetNewId(_context, "SYSTEM_ORGANIZATION"));
                Insert(_context, organization);
            }
            catch (Exception ex)
            {
                _log.LogError($"AddOrganization {order.RegNo} : {ex.Message}");
            }
            return organization;
        }

        public static SYSTEM_USER_DEVICE GetUserDevice(OracleDbContext _context, ILogger _log, int id)
        {
            return _context.SYSTEM_USER_DEVICE.FirstOrDefault(d => d.ID == id);
        }

        public static List<SYSTEM_USER_DEVICE> GetUserDevices(OracleDbContext _context, ILogger _log, string email)
        {
            return _context.SYSTEM_USER_DEVICE.Where(d => d.USERMAIL == email.ToLower()).ToList();
        }

        public static void RequestLog(OracleDbContext _context, ILogger _log, HttpContext context,
            string controller, string route, string parameter, bool success = true)
        {
            try
            {
                var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(context, UserProperties.CompanyId));
                var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(context, UserProperties.UserId));
                var user_data = ((Microsoft.AspNetCore.Server.Kestrel.Internal.Http.FrameRequestHeaders)context.Request.Headers).HeaderUserAgent.ToString();
                var userAgent = new Utils.UserAgent.UserAgent(user_data);

                var actionLog = new SYSTEM_REQUEST_ACTION_LOG()
                {
                    ID = Guid.NewGuid().ToString(),
                    OSNAME = userAgent.OS.Name,
                    OSVERSION = userAgent.OS.Version,
                    BROWSER = userAgent.Browser.Name,
                    BROWSERVERSION = userAgent.Browser.Version,
                    COMID = comid,
                    USERID = userid,
                    CONTROLLER = controller,
                    ROUTE = route,
                    REQUESTDATA = (parameter ?? "").Length,
                    REQUESTDATE = DateTime.Now,
                    REQUESTYEARMONTH = DateTime.Now.ToString("yyyyMM"),
                    PARAMETER = parameter,
                    SUCCESS = (byte)(success ? 1 : 0),
                    MESSAGE = ""
                };

                Insert(_context, actionLog);
            }
            catch (Exception ex)
            {
                _log.LogError($"ManagementLogic.RequestLog : {UsefulHelpers.GetExceptionMessage(ex)}");
            }
        }

        public static void RequestLog(OracleDbContext _context, ILogger _log, HttpContext context,
            string parameter, bool success = true)
        {
            try
            {
                var values = UsefulHelpers.GetControllerRoute(context);
                RequestLog(_context, _log, context, values[0], values[1], parameter, success);
            }
            catch (Exception ex)
            {
                _log.LogError($"ManagementLogic.RequestLog : {UsefulHelpers.GetExceptionMessage(ex)}");
            }
        }

        public static void ExceptionLog(OracleDbContext _context, ILogger _log, HttpContext context,
            string[] parameters, string controller, string route,
            Exception exception)
        {
            ExceptionLog(_context, _log, context,
                Newtonsoft.Json.JsonConvert.SerializeObject(parameters),
                controller, route, exception);
        }

        public static void ExceptionLog(OracleDbContext _context, ILogger _log, HttpContext context,
            string parameters, string controller, string route,
            Exception exception)
        {
            try
            {
                string user_data = ((Microsoft.AspNetCore.Server.Kestrel.Internal.Http.FrameRequestHeaders)context.Request.Headers)
                    .HeaderUserAgent.ToString();
                var ua = new Utils.UserAgent.UserAgent(user_data);

                var actionLog = new SYSTEM_REQUEST_ACTION_LOG()
                {
                    ID = Guid.NewGuid().ToString(),
                    OSNAME = ua.OS.Name,
                    OSVERSION = ua.OS.Version,
                    BROWSER = ua.Browser.Name,
                    BROWSERVERSION = ua.Browser.Version,
                    COMID = 0,
                    USERID = 0,
                    ROUTE = route,
                    CONTROLLER = controller,
                    REQUESTDATA = 0,
                    REQUESTDATE = DateTime.Now,
                    REQUESTYEARMONTH = DateTime.Now.ToString("yyyyMM"),
                    PARAMETER = Newtonsoft.Json.JsonConvert.SerializeObject(parameters),
                    SUCCESS = 0,
                    MESSAGE = exception.Message
                };

                Insert(_context, actionLog);
            }
            catch (Exception ex)
            {
                _log.LogError($"{controller} {route} ExceptionLog : {ex.Message}");
            }
        }

        public static SYSTEM_APP_USERS GetAppUser(OracleDbContext _context, string appname, int storeId = 2)
        {
            return _context.SYSTEM_APP_USERS.FirstOrDefault(x => x.APPNAME == appname && x.STOREID == storeId);
        }

        public static ResponseClient ChangeUserStatus(OracleDbContext _context, int id, ENABLED newValue, int modifiedBy)
        {
            try
            {
                var currentdata = _context.GET_USER(id);
                if (currentdata == null)
                    return ReturnResponce.NotFoundResponce();

                if (currentdata.ENABLED == newValue)
                    return ReturnResponce.FailedMessageResponce($"Хэрэглэгч {currentdata.ENABLED} төлөвтэй байна.");

                var newlog = new SYSTEM_USER_STATUS_LOG()
                {
                    ID = Convert.ToInt16(GetNewId(_context, "SYSTEM_USER_STATUS_LOG")),
                    USERID = currentdata.ID,
                    ORGID = currentdata.ORGID,
                    ENABLED = newValue == ENABLED.Идэвхитэй ? 1 : 0,
                    LOGYEAR = DateTime.Now.Year,
                    LOGMONTH = DateTime.Now.ToString("MM"),
                    LOGBY = modifiedBy,
                    LOGDATE = DateTime.Now
                };

                Insert(_context, newlog, false);

                currentdata.ENABLED = newValue;
                Update(_context, currentdata);

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static SYSTEM_ORGANIZATION GetOrganization(OracleDbContext _context, int id)
        {
            return _context.SYSTEM_ORGANIZATION.FirstOrDefault(x => x.ID == id);
        }

        public static SYSTEM_ORGANIZATION GetOrganization(OracleDbContext _context, string regno)
        {
            return _context.SYSTEM_ORGANIZATION.FirstOrDefault(x => x.REGNO == regno);
        }

        public static SYSTEM_USERS GetUser(OracleDbContext _context, int id)
        {
            return _context.SYSTEM_USERS.FirstOrDefault(x => x.ID == id);
        }

        public static SYSTEM_USERS GetUser(OracleDbContext _context, string email)
        {
            return _context.SYSTEM_USERS.FirstOrDefault(x => x.USERMAIL.ToLower() == email.ToLower());
        }

        public static SYSTEM_ROLES GetRole(OracleDbContext _context, int id)
        {
            return _context.SYSTEM_ROLES.FirstOrDefault(x => x.ID == id);
        }

        public static SYSTEM_ORGANIZATION_ROLES GetOrganizationRole(OracleDbContext _context, int id)
        {
            return _context.SYSTEM_ORGANIZATION_ROLES.FirstOrDefault(x => x.ID == id);
        }

        public static IQueryable<SYSTEM_USERS> GetUsers(OracleDbContext _context, int organizationId)
        {
            return _context.SYSTEM_USERS.Where(x => x.ORGID == organizationId);
        }

        public static IQueryable<SYSTEM_USERS> GetUsers(OracleDbContext _context, int organizationId, Enums.SystemEnums.ENABLED enabled)
        {
            return _context.SYSTEM_USERS.Where(x => x.ORGID == organizationId && x.ENABLED == enabled);
        }

        public static SYSTEM_STORECYCLE_CONFIG GetCycleConfig(OracleDbContext _context, int id)
        {
            return _context.SYSTEM_STORECYCLE_CONFIG.FirstOrDefault(x => x.ID == id);
        }

        public static IQueryable<SYSTEM_STORECYCLE_CONFIG> GetCycleConfigs(OracleDbContext _context, int storeid)
        {
            return _context.SYSTEM_STORECYCLE_CONFIG.Where(x => x.STOREID == storeid).OrderBy(o => o.CYCLEINDEX);
        }

        public static List<SYSTEM_ANNOUNCEMENT> GetUserAnnouncements(OracleDbContext _context, decimal orgId)
        {
            var announcements = (from a in _context.SYSTEM_ANNOUNCEMENT
                                 where a.ENABLED == 1 && a.BEGINDATE <= DateTime.Today && DateTime.Today <= a.ENDDATE
                                 select a).ToList();

            var userAnnouncements = (from a in announcements
                                     join u in _context.SYSTEM_ANNOUNCEMENT_CUSTOMER on a.ID equals u.HEADERID
                                     where a.TYPE != 1 && u.ORGID == orgId
                                     select a.ID).Distinct();

            return announcements.Where(a => a.TYPE == 1 || userAnnouncements.Contains(a.ID)).OrderBy(a => a.ENDDATE).ToList();
        }

        public static SYSTEM_ANNOUNCEMENT GetAnnouncement(OracleDbContext _context, decimal id)
        {
            return _context.SYSTEM_ANNOUNCEMENT.FirstOrDefault(a => a.ID == id);
        }

        public static List<SYSTEM_ANNOUNCEMENT_CUSTOMER> GetAnnouncementUsers(OracleDbContext _context, decimal id)
        {
            return _context.SYSTEM_ANNOUNCEMENT_CUSTOMER.Where(a => a.HEADERID == id).ToList();
        }

        public static ResponseClient DeleteAnnouncement(OracleDbContext _context, ILogger _logger, decimal id)
        {
            try
            {
                var request = GetAnnouncement(_context, id);
                if (request == null)
                    return ReturnResponce.NotFoundResponce();

                request.ENABLED = 0;
                Update(_context, request);

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static ResponseClient GetAnnouncement(OracleDbContext _context, ILogger _logger, decimal id)
        {
            try
            {
                var announcement = GetAnnouncement(_context, id);
                if (announcement == null)
                    return ReturnResponce.NotFoundResponce();

                var response = new AnnouncementRequestDto(announcement);
                if (announcement.TYPE != 1)
                {
                    var users = GetAnnouncementUsers(_context, id);
                    if (users.Any())
                        users.ForEach(a => response.USERS.Add(a.ORGID));
                }

                return ReturnResponce.ListReturnResponce(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"MANAGEMENTLOGIC.GETANNOUNCEMENT : {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static ResponseClient SetAnnouncement(OracleDbContext _context, ILogger _logger, IFormFile picture, 
            AnnouncementRequestDto request, decimal userId)
        {
            try
            {
                if (request.TYPE == 1 && (request.USERS == null || !request.USERS.Any()))
                    return ReturnResponce.FailedMessageResponce("Хэрэглэгчид тодорхойгүй байна.");

                if (picture != null)
                {
                    var type = UsefulHelpers.GetImageType(picture.FileName);
                    var fileType = UsefulHelpers.GetFileType(picture);
                    if (fileType == FileType.None || fileType == FileType.Forbidden)
                        return ReturnResponce.FailedMessageResponce($"{picture.FileName} файлыг оруулах боломжгүй.");

                    var restUtils = new HttpRestUtils(UsefulHelpers.STORE_ID, _context);
                    if (!restUtils.StoreServerConnected)
                        return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

                    var attachResponse = restUtils.Post($"/api/attachment/{(fileType == FileType.Image ? "image" : "file")}", picture).Result;
                    if (!attachResponse.Success)
                        return attachResponse;

                    request.IMAGEURL = attachResponse.Value.ToString();
                }

                SYSTEM_ANNOUNCEMENT announcement = null;
                var existingUsers = new List<SYSTEM_ANNOUNCEMENT_CUSTOMER>();
                if (request.ID == 0)
                {
                    announcement = new SYSTEM_ANNOUNCEMENT(request);
                    announcement.ID = GetNewId(_context, typeof(SYSTEM_ANNOUNCEMENT).Name);
                    announcement.CREATEDBY = userId;
                    announcement.CREATEDDATE = DateTime.Now;
                    Insert(_context, announcement, false);
                }
                else
                {
                    announcement = GetAnnouncement(_context, request.ID);
                    if (announcement == null)
                        return ReturnResponce.NotFoundResponce();

                    announcement.BEGINDATE = request.BEGINDATE;
                    announcement.ENDDATE = request.ENDDATE;
                    announcement.TYPE = request.TYPE;
                    announcement.TITLE = request.TITLE;
                    announcement.MESSAGE = request.MESSAGE;
                    announcement.IMAGEURL = request.IMAGEURL;
                    announcement.RECEIVER = request.RECEIVER;

                    existingUsers = GetAnnouncementUsers(_context, announcement.ID);
                    Update(_context, announcement, false);
                }

                announcement.UPDATEDBY = userId;
                announcement.UPDATEDDATE = DateTime.Now;

                if (announcement.TYPE == 1 && existingUsers.Any())
                    existingUsers.ForEach(a => Delete(_context, a, false));

                if (announcement.TYPE != 1 && request.USERS.Any())
                {
                    foreach (var user in existingUsers)
                    {
                        if (request.USERS.Contains(user.ORGID)) continue;
                        Delete(_context, user, false);
                    }

                    foreach (var currentOrgId in request.USERS)
                    {
                        if (existingUsers.FirstOrDefault(a => a.ORGID == currentOrgId) != null) continue;
                        Insert(_context, new SYSTEM_ANNOUNCEMENT_CUSTOMER()
                        {
                            ID = GetNewId(_context, "SYSTEM_ANN_CUSTOMER"),
                            HEADERID = announcement.ID,
                            ORGID = currentOrgId
                        }, false);
                    }
                }

                _context.SaveChanges();
                return ReturnResponce.ListReturnResponce(announcement);
            }
            catch (Exception ex)
            {
                _logger.LogError($"MANAGEMENTLOGIC.SETANNOUNCEMENT : {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static ResponseClient Turgargaw(OracleDbContext _context, ILogger _logger, List<string> request, decimal annid)
        {
            try
            
            {

                SYSTEM_ANNOUNCEMENT announcement = null;
                announcement = GetAnnouncement(_context, annid);
                if (announcement == null)
                    return ReturnResponce.NotFoundResponce();

                var users = (from d in request
                             join s in _context.SYSTEM_ORGANIZATION on d equals s.REGNO
                             select s.ID).ToList();
                foreach (var currentOrgId in users)
                {
                    Insert(_context, new SYSTEM_ANNOUNCEMENT_CUSTOMER()
                    {
                        ID = GetNewId(_context, "SYSTEM_ANN_CUSTOMER"),
                        HEADERID = announcement.ID,
                        ORGID = currentOrgId
                    }, false);
                }

                _context.SaveChanges();
                return ReturnResponce.ListReturnResponce(announcement);
            }
            catch (Exception ex)
            {
                _logger.LogError($"MANAGEMENTLOGIC.SETANNOUNCEMENT : {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        public static AnnouncementListModel GetAnnouncements(OracleDbContext _context, ILogger _log, AnnouncementFilterView filter)
        {
            var P_BEGIN_DATE = new OracleParameter("P_BEGIN_DATE", OracleDbType.Date, filter.BEGINDATE, ParameterDirection.Input);
            var P_END_DATE = new OracleParameter("P_END_DATE", OracleDbType.Date, filter.ENDDATE, ParameterDirection.Input);

            var P_ORDER_COLUMN = new OracleParameter("P_ORDER_COLUMN", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var P_START_ROW = new OracleParameter("P_START_ROW", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var P_ROW_COUNT = new OracleParameter("P_ROW_COUNT", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);

            var RETURN_VALUE = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            int totalCount = _context.Database.SqlQuery<int>("BEGIN GET_ANNOUNCEMENTS_TOTAL("
             + ":P_BEGIN_DATE, "
             + ":P_END_DATE, "
             + ":RETURN_VALUE); END; ",
             P_BEGIN_DATE, P_END_DATE, RETURN_VALUE).Single();

            var values = _context.Database.SqlQuery<AnnouncementRequestDto>("BEGIN GET_ANNOUNCEMENTS("
             + ":P_BEGIN_DATE, "
             + ":P_END_DATE, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":RETURN_VALUE); END; ",
             P_BEGIN_DATE, P_END_DATE, P_ORDER_COLUMN, P_START_ROW, P_ROW_COUNT,
             RETURN_VALUE).ToList();

            return new AnnouncementListModel(totalCount, values);
        }
    }
}
