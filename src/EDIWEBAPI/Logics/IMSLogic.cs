using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;
namespace EDIWEBAPI.Logics
{
    public class IMSLogic : BaseLogic
    {
        public static HttpRequestModel GetHttpModelData(OracleDbContext _context, int comid, string appName = "IMS")
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
        public static bool UpdateAppToken(OracleDbContext _context, int comid, TokenValue token, string appName = "IMS")
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
        public static SYSTEM_APP_USERS GetAppUser(OracleDbContext _context, string appname, int storeId = 2)
        {
            return _context.SYSTEM_APP_USERS.FirstOrDefault(x => x.APPNAME == appname && x.STOREID == storeId);
        }
    }
}
