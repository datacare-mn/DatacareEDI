using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.LicenseConfig;
using EDIWEBAPI.Utils;
using Newtonsoft.Json;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Entities.CustomModels;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using EDIWEBAPI.Entities.RequestModels;

namespace EDIWEBAPI.Logics
{
    public class LicenseLogic : BaseLogic
    {
        public static SYSTEM_LICENSE_USER GetLastLicense(OracleDbContext _context, int userId)
        {
            var baseFee = _context.SYSTEM_LICENSE_PRICE.Where(a => a.ENABLED == 1 && a.TYPE == 1)
                .OrderBy(a => a.VIEWORDER)
                .FirstOrDefault();

            return _context.SYSTEM_LICENSE_USER.Where(u => u.USERID == userId && u.LICENSEID == baseFee.ID && u.ENABLED == 1)
                .OrderByDescending(u => u.YEARANDMONTH).FirstOrDefault();
        }

        public static SYSTEM_CONTRACT GetContract(OracleDbContext _context, int storeId, DateTime currentDate)
        {
            return _context.SYSTEM_CONTRACT.FirstOrDefault(c => c.STOREID == storeId && c.BEGINDATE <= currentDate && currentDate <= c.ENDDATE && c.ENABLED == 1);
        }

        public static SYSTEM_CONTRACT_DETAIL SetContractDetail(OracleDbContext _context, ILogger _log, int storeId, DateTime currentDate)
        {
            try
            {
                var contract = GetContract(_context, storeId, currentDate);
                if (contract == null) return null;

                var newDetail = new SYSTEM_CONTRACT_DETAIL()
                {
                    ID = Convert.ToInt16(GetNewId(_context, "SYSTEM_CONTRACT_DETAIL")),
                    LICENSEID = contract.ID,
                    YEAR = currentDate.Year,
                    MONTH = currentDate.ToString("MM"),
                    PRICE = contract.PRICE
                };
                var baseFee = _context.SYSTEM_LICENSE_PRICE.Where(p => p.TYPE == 1).OrderBy(a => a.VIEWORDER).FirstOrDefault();
                newDetail.ACTUALQTY = _context.SYSTEM_LICENSE_USER.Count(u => u.MONTH == newDetail.MONTH && u.YEAR == newDetail.YEAR && u.LICENSEID == baseFee.ID);
                newDetail.CHARGEQTY = newDetail.ACTUALQTY < contract.USERQTY ? contract.USERQTY : newDetail.ACTUALQTY;

                Insert(_context, newDetail, true);
                return newDetail;
            }
            catch (Exception ex)
            {
                _log.LogError($"LicenseLogic.SetContractDetails : {UsefulHelpers.GetExceptionMessage(ex)}");
                return null;
            }
        }

        public static void SetUserLicenses(OracleDbContext _context, List<Entities.RequestModels.UserLicenseRequest> licenses,
            int comId, int userId, DateTime currentDate)
        {
            var licenseUsers = LicenseLogic.GetUserLicenses(_context, userId, currentDate);
            foreach (var license in licenses)
            {
                if (license.ENABLED == license.VALUE && license.ENABLEDANNUAL == license.VALUEANNUAL) continue;

                var existing = licenseUsers.FirstOrDefault(u => u.LICENSEID == license.LICENSEID);
                var annual = license.ENABLEDANNUAL != license.VALUEANNUAL;
                if (existing == null)
                {
                    // ХЭРВЭЭ САРААР ЖИЛЭЭР 2УЛАА ЧЕКТЭЙ ИРВЭЛ / БААЗАД БАЙХГҮЙ УЧРААС ӨӨРЧЛӨГДӨХ БОЛОМЖТОЙ ХУВИЛБАР НЬ 2УУЛАА ЧЕКТЭЙ /
                    // ЖИЛЭЭР ГЭЖ ОЙЛГООД ХИЙНЭ. => ANNUAL
                    var newLicense = new SYSTEM_LICENSE_USER()
                    {
                        ID = GetNewId(_context, "SYSTEM_LICENSE_USER"),
                        YEAR = currentDate.Year,
                        MONTH = currentDate.ToString("MM"),
                        YEARANDMONTH = Convert.ToDecimal(currentDate.ToString("yyyyMM")),
                        BUSINESSID = comId,
                        USERID = userId,
                        LICENSEID = license.LICENSEID
                    };

                    newLicense.SetAnnual(annual, license);
                    Insert(_context, newLicense, false);
                }
                else
                {
                    // ХЭРВЭЭ САРААР ТОХИРГООГ ЖИЛЭЭР БОЛГОСОН ЭСВЭЛ ЖИЛЭЭР ТОХИРГООГ САРААР БОЛГОСОН БОЛ 
                    // 2 УУЛАА ӨӨРЧЛӨГДСӨН ИРНЭ. ЭНЭ ТОХИОЛДОЛД БААЗАД ЯМАР ХАДГАЛАГДСАН БАЙГААГААС ХАМААРУУЛНА
                    if (license.ENABLED != license.VALUE && license.ENABLEDANNUAL != license.VALUEANNUAL)
                        annual = existing.ANNUAL != 1;

                    existing.SetAnnual(annual, license);
                    Update(_context, existing, false);
                }
            }
            _context.SaveChanges();
        }

        public static List<UserLicenseDto> GetUserLicense(OracleDbContext _context, ILogger _logger, int userId, DateTime currentDate)
        {
            var licenseUsers = LicenseLogic.GetUserLicenses(_context, userId, currentDate);
            var defaultQty = (12 - currentDate.Month) + 1;

            var prices = GetPrices(_context);
            var reports = GetReportQty(_context, prices, currentDate, 0, userId);

            return (from p in prices
                    join u in licenseUsers on p.ID equals u.LICENSEID into lj
                    from l in lj.DefaultIfEmpty()
                    join r in reports on p.ID equals r.LICENSEID into llj
                    from ll in llj.DefaultIfEmpty()
                    orderby p.VIEWORDER
                    select new UserLicenseDto()
                    {
                        LICENSEID = p.ID,
                        TYPE = p.TYPE,
                        NAME = p.NAME,
                        NOTE = p.NOTE,
                        IMAGEURL = p.IMAGEURL,
                        SAVED = l != null,
                        PRICE = p.PRICE,
                        ANNUALPRICE = p.ANNUALPRICE,
                        ACTUALPRICE = l == null ? p.PRICE : l.PRICE,
                        QTY = l == null ? (p.TYPE == 1 ? 1 : 0) : l.QTY,
                        DEFAULTQTY = defaultQty,
                        AMOUNT = l == null ? (p.TYPE == 1 ? p.PRICE : 0) : l.AMOUNT,
                        ENABLED = l == null ? p.TYPE : (l.ANNUAL == 1 ? 2 : l.ENABLED),
                        ENABLEDANNUAL = l == null ? 2 : (l.ANNUAL == 1 ? l.ENABLED : 2),
                        VALUE = l == null ? p.TYPE : (l.ANNUAL == 1 ? 2 : l.ENABLED),
                        VALUEANNUAL = l == null ? 2 : (l.ANNUAL == 1 ? l.ENABLED : 2),
                        USAGEQTY = ll == null ? 0 : ll.COUNT,
                        PARENTID = l == null ? null : l.PARENTID
                        //DISABLEDDATE = l == null ? null : l.DISABLEDDATE,
                        //ENABLEDDATE = l == null ? null : l.ENABLEDDATE
                    }).ToList();
        }

        public static ResponseClient Calculate(OracleDbContext _context, ILogger _logger, int storeId, DateTime currentDate)
        {
            var logMethod = "LicenseLogic.Calculate";
            try
            {
                var values = GetStoreLicenses(_context, _logger, storeId, currentDate);
                _logger.LogInformation($"{logMethod} : AFTER GET");
                if (values == null || !values.Any())
                    return ReturnResponce.NotFoundResponce();

                var yearAndMonth = Convert.ToDecimal(currentDate.ToString("yyyyMM"));
                // ХУУЧИН БИЧЛЭГҮҮДИЙГ УСТГАХ
                var oldRecords = _context.SYSTEM_LICENSE_BUSINESS.Where(a => a.YEARANDMONTH == yearAndMonth);
                if (oldRecords.Any())
                {
                    _logger.LogInformation($"{logMethod} RECORDS TO DELETE : {oldRecords.Count()}");
                    _context.SYSTEM_LICENSE_BUSINESS.RemoveRange(oldRecords);
                }

                var total = decimal.Zero;
                values.ForEach(a =>
                {
                    total += a.TOTALFEE.Value;
                    Insert(_context, new SYSTEM_LICENSE_BUSINESS()
                    {
                        ID = Convert.ToDecimal(GetNewId(_context, "SYSTEM_LICENSE_BUSINESS")),
                        BUSINESSID = a.ID,
                        YEAR = currentDate.Year,
                        MONTH = currentDate.ToString("MM"),
                        YEARANDMONTH = yearAndMonth,
                        USERQTY = a.USERQTY,
                        USERFEE = a.BASEFEE,
                        REPORTQTY = a.REPORTQTY,
                        REPORTFEE = a.REPORTFEE,
                        TOTAL = a.TOTALFEE,
                        AMOUNT = a.TOTALFEE
                    }, false);
                });

                _logger.LogInformation($"{logMethod} : TOTAL ({values.Count} => {total})");
                _context.SaveChanges();
                _logger.LogInformation($"{logMethod} : AFTER COMMIT");

                MailLogic.AddSMS(_context, Enums.SystemEnums.MessageType.None,
                    $"{total} duntei {values.Count} shirheg license bodogdow. www.urto.mn",
                    Controllers.SendData.Messager.DEVELOPER_PHONE_NO);

                return ReturnResponce.SuccessMessageResponce("");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{logMethod} : {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        
        public static List<OrganizationLicenseDto> GetCompanyUserLicenses(OracleDbContext _context, ILogger _log, int comid, DateTime currentDate)
        {
            var prices = GetPrices(_context);
            var licenses = GetLicenses(_context, comid, currentDate);
            var reports = GetReportQty(_context, prices, currentDate, comid, 0);
            var licenseUsers = GetCompanyUserLicenses(prices, licenses);
            
            var results = (from u in licenseUsers 
                           group u by new { u.USERID, u.USERMAIL, u.USERNAME } into g
                           orderby g.Key.USERMAIL
                           select new OrganizationLicenseDto()
                           {
                               ID = g.Key.USERID,
                               REGNO = g.Key.USERMAIL,
                               NAME = g.Key.USERNAME,
                               USERQTY = g.Sum(k => k.TYPE == 1 ? k.COUNT : 0),
                               BASEFEE = g.Sum(u => u.TYPE == 1 ? u.AMOUNT : 0),
                               REPORTQTY = g.Sum(k => k.TYPE == 2 ? k.COUNT : 0),
                               REPORTFEE = g.Sum(u => u.TYPE == 2 ? u.AMOUNT : 0),
                               TOTALFEE = g.Sum(u => u.AMOUNT)
                           }).ToList();

            foreach (var current in results)
            {
                var details = from u in licenses
                              join p in prices on u.LICENSEID equals p.ID
                              join r in reports on new { u.USERID, u.LICENSEID } equals new { r.USERID, r.LICENSEID } into lj
                              from l in lj.DefaultIfEmpty()
                              where u.USERID == current.ID
                              orderby u.USERID, p.VIEWORDER
                              select new UserReportDto()
                              {
                                  ID = u.USERID,
                                  NAME = u.FIRSTNAME,
                                  EMAIL = u.USERMAIL,
                                  LICENSENAME = p.NAME,
                                  PRICE = u.AMOUNT,
                                  REPORTQTY = l == null ? 0 : l.COUNT
                              };

                if (details.Any())
                    current.DETAILS = details.ToList();
            }

            return results;
        }

        public static ResponseClient ChangeLicense(OracleDbContext _context, ILogger _log, LicenseChangeRequest request, decimal userId)
        {
            try
            {
                var license = GetLicenseBusiness(_context, request.BUSINESSID, request.LICENSEDATE);
                if (license == null)
                    return ReturnResponce.NotFoundResponce();

                Insert(_context, new SYSTEM_LICENSE_LOG()
                {
                    ID = GetNewId(_context, typeof(SYSTEM_LICENSE_LOG).Name),
                    HEADERID = license.ID,
                    NEWVALUE = request.NEWVALUE,
                    OLDVALUE = license.AMOUNT,
                    NOTE = request.NOTE,
                    CREATEDBY = userId,
                    CREATEDDATE = DateTime.Now
                }, false);

                license.AMOUNT = request.NEWVALUE;
                Update(_context, license, false);

                _context.SaveChanges();
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static SYSTEM_LICENSE_BUSINESS GetLicenseBusiness(OracleDbContext _context, decimal businessId, DateTime value)
        {
            var yearAndMonth = Convert.ToDecimal(value.ToString("yyyyMM"));
            return _context.SYSTEM_LICENSE_BUSINESS.FirstOrDefault(a => a.BUSINESSID == businessId && a.YEARANDMONTH == yearAndMonth);
        }

        public static ResponseClient GetLicenseLogs(OracleDbContext _context, ILogger _log, decimal businessId, DateTime value)
        {
            try
            {
                var license = GetLicenseBusiness(_context, businessId, value);
                if (license == null)
                    return ReturnResponce.NotFoundResponce();

                var logs = (from l in _context.SYSTEM_LICENSE_LOG
                            join u in _context.SYSTEM_USERS on l.CREATEDBY equals u.ID
                            where l.HEADERID == license.ID
                            select new
                            {
                                l.OLDVALUE,
                                l.NEWVALUE,
                                l.NOTE,
                                USERNAME = u.FIRSTNAME,
                                l.CREATEDDATE
                            }).ToList();

                return logs.Any() ?
                    ReturnResponce.ListReturnResponce(logs) :
                    ReturnResponce.NotFoundResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static List<OrganizationLicenseDto> GetStoreLicenses(OracleDbContext _context, ILogger _log, int storeId, DateTime value, bool extraInformation = false)
        {
            var prices = GetPrices(_context);
            var licenses = GetLicenses(_context, 0, value);
            var reports = GetReportQty(_context, prices, value);
            var licenseUsers = GetCompanyUserLicenses(prices, licenses);

            var yearAndMonth = Convert.ToDecimal(value.ToString("yyyyMM"));
            var calculated = new List<SYSTEM_LICENSE_BUSINESS>();
            if (extraInformation)
            {
                calculated = (from b in _context.SYSTEM_LICENSE_BUSINESS
                              where b.YEARANDMONTH == yearAndMonth
                              select b).ToList();
            }

            _log.LogInformation($"LicenseLogic.GetStoreLicenses : 6");

            var response = (from o in _context.SYSTEM_ORGANIZATION.ToList()
                            join u in licenseUsers on o.ID equals u.BUSINESSID
                            group u by new { u.BUSINESSID, o.REGNO, o.COMPANYNAME } into g
                            orderby g.Key.COMPANYNAME
                            select new OrganizationLicenseDto()
                            {
                                ID = g.Key.BUSINESSID,
                                REGNO = g.Key.REGNO,
                                NAME = g.Key.COMPANYNAME,
                                USERQTY = g.Sum(k => k.TYPE == 1 ? k.COUNT : 0),
                                BASEFEE = g.Sum(u => u.TYPE == 1 ? u.AMOUNT : 0),
                                REPORTQTY = g.Sum(k => k.TYPE == 2 ? k.COUNT : 0),
                                REPORTFEE = g.Sum(u => u.TYPE == 2 ? u.AMOUNT : 0),
                                TOTALFEE = g.Sum(u => u.AMOUNT)
                            }).ToList();

            _log.LogInformation($"LicenseLogic.GetStoreLicenses : 7");

            foreach (var current in response)
            {
                if (extraInformation)
                {
                    var calc = calculated.FirstOrDefault(a => a.BUSINESSID == current.ID);
                    if (calc == null)
                    {
                        current.CALCULATED = false;
                        current.TOTALAMOUNT = current.TOTALFEE;
                    }
                    else
                    {
                        current.CALCULATED = true;
                        current.TOTALAMOUNT = calc.AMOUNT;

                        var history = (from l in _context.SYSTEM_LICENSE_LOG
                                       join u in _context.SYSTEM_USERS on l.CREATEDBY equals u.ID
                                       where l.HEADERID == calc.ID
                                       orderby l.CREATEDDATE descending
                                       select new
                                       {
                                           u.FIRSTNAME,
                                           l.CREATEDDATE
                                       }).Take(1);

                        if (history.Any())
                        {
                            current.UPDATEDBY = history.First().FIRSTNAME;
                            current.UPDATEDDATE = history.First().CREATEDDATE;
                        }
                    }
                }

                var details = from u in licenses
                              join p in prices on u.LICENSEID equals p.ID
                              join r in reports on new { u.USERID, u.LICENSEID } equals new { r.USERID, r.LICENSEID } into lj
                              from l in lj.DefaultIfEmpty()
                              where u.BUSINESSID == current.ID
                              orderby u.USERID, p.VIEWORDER
                              select new UserReportDto()
                              {
                                  ID = u.USERID,
                                  NAME = UsefulHelpers.GetUserName(u.USERMAIL, u.FIRSTNAME),
                                  EMAIL = u.USERMAIL,
                                  LICENSENAME = p.NAME,
                                  PRICE = u.AMOUNT,
                                  REPORTQTY = l == null ? 0 : l.COUNT
                              };

                if (details.Any())
                    current.DETAILS = details.ToList();
            }

            _log.LogInformation($"LicenseLogic.GetStoreLicenses : 8");

            return response;
        }

        private static List<SYSTEM_LICENSE_PRICE> GetPrices(OracleDbContext _context)
        {
            return (from p in _context.SYSTEM_LICENSE_PRICE
                    where p.ENABLED == 1
                    orderby p.VIEWORDER
                    select p).ToList();
        }

        private static List<LicenseUserDto> GetLicenses(OracleDbContext _context, int comId, DateTime value)
        {
            var month = value.ToString("MM");
            return (from u in _context.SYSTEM_LICENSE_USER
                    join s in _context.SYSTEM_USERS on u.USERID equals s.ID
                    where u.MONTH == month && u.YEAR == value.Year && (comId == 0 || u.BUSINESSID == comId)
                    select new LicenseUserDto()
                    {
                        BUSINESSID = u.BUSINESSID,
                        USERID = u.USERID,
                        LICENSEID = u.LICENSEID,
                        USERMAIL = s.USERMAIL,
                        FIRSTNAME = s.FIRSTNAME,
                        AMOUNT = u.AMOUNT
                    }).ToList();
        }

        private static List<UserLicenseQty> GetReportQty(OracleDbContext _context, List<SYSTEM_LICENSE_PRICE> prices, DateTime value, 
            int comId = 0, int userId = 0)
        {
            var controllers = (from p in prices
                               where p.CONTROLLER != null
                               group p by p.CONTROLLER into g
                               select g.Key).ToList();

            var yearMonth = value.ToString("yyyyMM");
            var logs = (from l in _context.SYSTEM_REQUEST_ACTION_LOG
                        join c in controllers on l.CONTROLLER equals c
                        where l.REQUESTYEARMONTH == yearMonth && (comId == 0 || l.COMID == comId) && (userId == 0 || l.USERID == userId)
                        group l by new { l.USERID, l.CONTROLLER, l.ROUTE } into g
                        select new { USERID = g.Key.USERID, CONTROLLER = g.Key.CONTROLLER, ROUTE = g.Key.ROUTE, COUNT = g.Count() }).ToList();

            return (from p in prices
                    join r in logs on
                        new { CONTROLLER = (p.CONTROLLER ?? "").ToUpper(), ROUTE = (p.ROUTE ?? "").ToUpper() } equals
                        new { CONTROLLER = r.CONTROLLER.ToUpper(), ROUTE = r.ROUTE.ToUpper() }
                    where p.CONTROLLER != null
                    group r by new { p.ID, r.USERID } into g
                    select new UserLicenseQty()
                    {
                        USERID = g.Key.USERID,
                        LICENSEID = g.Key.ID,
                        COUNT = g.Sum(a => a.COUNT)
                    }).ToList();
        }

        private static List<OrgUserLicenseDto> GetCompanyUserLicenses(List<SYSTEM_LICENSE_PRICE> prices, List<LicenseUserDto> licenses)
        {
            return (from u in licenses
                    join l in prices on u.LICENSEID equals l.ID
                    group u by new { u.BUSINESSID, u.USERID, u.USERMAIL, u.FIRSTNAME, l.TYPE } into g
                    select new OrgUserLicenseDto()
                    {
                        BUSINESSID = g.Key.BUSINESSID,
                        USERID = g.Key.USERID,
                        USERMAIL = g.Key.USERMAIL,
                        USERNAME = g.Key.FIRSTNAME,
                        TYPE = g.Key.TYPE,
                        COUNT = g.Count(),
                        AMOUNT = g.Sum(p => p.AMOUNT)
                    }).ToList();
        }

        public static CompanyLicenseDto GetCompanyLicenses(OracleDbContext _context, int comid, DateTime currentDate)
        {
            var organization = ManagementLogic.GetOrganization(_context, comid);
            var orgusers = _context.SYSTEM_USERS.Where(u => u.ORGID == comid);
            var response = new CompanyLicenseDto()
            {
                ID = organization.ID,
                REGNO = organization.REGNO,
                NAME = organization.COMPANYNAME,
                USERQTY = orgusers.Count(),
                REGISTRYDATE = orgusers.Min(p => p.REGDATE)
            };

            var prices = GetPrices(_context);

            var month = currentDate.ToString("MM");
            var users = from u in _context.SYSTEM_LICENSE_USER
                        where u.BUSINESSID == comid && u.MONTH == month && u.YEAR == currentDate.Year
                        group u by u.LICENSEID into g
                        select new { LICENSEID = g.Key, COUNT = g.Count(), AMOUNT = g.Sum(p => p.AMOUNT) };

            response.DETAILS = (from l in prices
                                join u in users on l.ID equals u.LICENSEID into j
                                from lj in j.DefaultIfEmpty()
                                select new UserLicenseDto()
                                {
                                    LICENSEID = l.ID,
                                    NAME = l.NAME,
                                    PRICE = l.PRICE,
                                    TYPE = l.TYPE,
                                    VALUE = lj == null ? 0 : lj.COUNT,
                                    AMOUNT = lj == null ? 0 : lj.AMOUNT
                                }).ToList();

            response.AMOUNT = response.DETAILS.Sum(p => p.AMOUNT).Value;
            return response;
        }

        public static void CheckMonthFees(OracleDbContext _context, ILogger _log, int orgId, int userId, DateTime currentDate)
        {
            try
            {
                var prices = GetPrices(_context);
                var currentFees = GetUserLicenses(_context, userId, currentDate, false);
                // ХЭРВЭЭ САРЫН ХУРААМЖ ТӨРӨЛТЭЙ ЛИЦЕНЗ ХАДГАЛАГДСАН БАЙВАЛ
                var enabled = from p in prices
                              join f in currentFees on p.ID equals f.LICENSEID
                              where f.ENABLED == 1 && p.TYPE == 1
                              select f;

                _log.LogInformation($"LicenseLogic.CheckMonthFees : {userId} : {currentDate} => {enabled.Count()}");
                if (enabled.Any()) return;

                var disabled = currentFees.Where(f => f.ENABLED != 1);
                var lastFees = GetLastUserLicenses(_context, userId, currentDate);
                var annualFee = from l in _context.SYSTEM_LICENSE_USER
                                where l.USERID == userId && l.YEAR == currentDate.Year && l.ENABLED == 1 && l.ANNUAL == 1 && l.PARENTID == null
                                select new { l.ID, l.LICENSEID };

                foreach (var price in prices)
                {
                    decimal? parentId = null;
                    var annual = false;
                    // ХЭРВЭЭ САРЫН СУУРЬ ХУРААМЖ ЛУГАА БИШ БАЙВАЛ
                    if (price.TYPE != 1)
                    {
                        // ХЭРВЭЭ УРЬД САР НЬ ДАРАА САРДАА АШИГЛАХГҮЙ ГЭЖ ХАДГАЛСАН БОЛ
                        if (disabled.FirstOrDefault(d => d.LICENSEID == price.ID) != null) continue;
                        // ӨМНӨХ САРЫН БИЧЛЭГГҮЙ ЭСВЭЛ ӨМНӨХ САР НЬ АШИГЛААГҮЙ БӨГӨӨД ЖИЛЭЭР АШИГЛАХ ТОХИРГООГҮЙ БОЛ 
                        var lastFee = lastFees.FirstOrDefault(l => l.LICENSEID == price.ID);
                        var parent = annualFee.Where(a => a.LICENSEID == price.ID);

                        if (!parent.Any() && (lastFee == null || lastFee.ENABLED != 1)) continue;
                        // УДАМШСАН БИЧЛЭГИЙН ID
                        annual = parent.Any();
                        if (annual)
                            parentId = parent.FirstOrDefault().ID;
                        else
                            parentId = lastFee.ID;
                    }

                    _log.LogInformation($"LicenseLogic.CheckMonthFees : {userId} : {price.ID} : {price.PRICE}");
                    var newObject = new SYSTEM_LICENSE_USER()
                    {
                        ID = GetNewId(_context, "SYSTEM_LICENSE_USER"),
                        PARENTID = parentId,
                        YEAR = currentDate.Year,
                        MONTH = currentDate.ToString("MM"),
                        YEARANDMONTH = Convert.ToDecimal(currentDate.ToString("yyyyMM")),
                        ANNUAL = annual ? 1 : 2,
                        BUSINESSID = orgId,
                        USERID = userId,
                        LICENSEID = price.ID,
                        PRICE = annual ? price.ANNUALPRICE : price.PRICE,
                        QTY = annual ? 0 : 1,
                        AMOUNT = annual ? 0 : price.PRICE,
                        ENABLED = 1,
                        ENABLEDDATE = DateTime.Now
                    };

                    Insert(_context, newObject, false);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError($"LicenseLogic.CheckMonthFees : {UsefulHelpers.GetExceptionMessage(ex)}");
            }
        }

        public static List<SYSTEM_LICENSE_USER> GetLastUserLicenses(OracleDbContext _context, int userId, DateTime date)
        {
            var yearAndMonth = Convert.ToDecimal(date.ToString("yyyyMM"));
            var lastFee = from u in _context.SYSTEM_LICENSE_USER
                          where u.USERID == userId && u.YEARANDMONTH < yearAndMonth && u.ENABLED == 1 && u.ANNUAL != 1
                          orderby u.YEARANDMONTH descending
                          select u.YEARANDMONTH;

            if (!lastFee.Any()) return new List<SYSTEM_LICENSE_USER>();
            
            yearAndMonth = lastFee.First();
            return (from u in _context.SYSTEM_LICENSE_USER
                    where u.USERID == userId && u.YEARANDMONTH == yearAndMonth && u.ENABLED == 1 && u.ANNUAL != 1
                    select u).ToList();
        }

        public static List<SYSTEM_LICENSE_USER> GetUserLicenses(OracleDbContext _context, int userId, DateTime date, bool enabled = false)
        {
            var month = date.ToString("MM");

            return (from u in _context.SYSTEM_LICENSE_USER
                    where u.USERID == userId && u.MONTH == month && u.YEAR == date.Year && (!enabled || u.ENABLED == 1) //&& u.ENABLED == 1
                    select u).ToList();
        }

        public static Tuple<decimal, string> GetLicenseData(OracleDbContext _context, int storeid, int businessid, DateTime archdate)
        {
            string archmonth = archdate.ToString("MM");

            var licensedata = _context.SYSTEM_BIZ_LICENSE.FirstOrDefault(x => x.BUSINESSID == businessid
                && x.STOREID == storeid && x.YEAR == archdate.Year && x.MONTH == archmonth);

            return licensedata != null ? new Tuple<decimal, string>(Convert.ToDecimal(licensedata.AMOUNT), licensedata.PAYCTRCD)
                : new Tuple<decimal, string>(0, "");
        }

        public static SYSTEM_LICENSE_BUSINESS GetLicenseData(OracleDbContext _context, int businessId, DateTime curentDate)
        {
            var archmonth = curentDate.ToString("MM");

            return _context.SYSTEM_LICENSE_BUSINESS.FirstOrDefault(x => x.BUSINESSID == businessId
                && x.MONTH == archmonth && x.YEAR == curentDate.Year);
        }

        public static void DeleteExisting(OracleDbContext _dbContext, ILogger _log, int storeid, DateTime currentDate)
        {
            var found = false;
            try
            {
                var month = currentDate.ToString("MM");
                var licenses = _dbContext.SYSTEM_BIZ_LICENSE.Where(l => l.MONTH == month && l.YEAR == currentDate.Year && l.STOREID == storeid);
                if (licenses != null && licenses.Any())
                {
                    _dbContext.SYSTEM_BIZ_LICENSE.RemoveRange(licenses);
                    found = true;
                }

                var details = _dbContext.SYSTEM_BIZ_LIC_DETAIL.Where(d => d.MONTH == month && d.YEAR == currentDate.Year && d.STOREID == storeid);
                if (details != null && details.Any())
                {
                    _dbContext.SYSTEM_BIZ_LIC_DETAIL.RemoveRange(details);
                    found = true;
                }

                if (found)
                {
                    _dbContext.SaveChanges();
                    _log.LogInformation($"LicenseLogic.DeleteExisting SUCCEED.");
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"LicenseLogic.DeleteExisting {found} : {ex.Message}");
            }
        }

        public static void DeleteExisting(OracleDbContext _dbContext, ILogger _log, int storeid, int businessid, DateTime currentDate)
        {
            var found = false;
            try
            {
                var month = currentDate.ToString("MM");
                var licenses = _dbContext.SYSTEM_BIZ_LICENSE.Where(l => l.BUSINESSID == businessid && l.MONTH == month && l.YEAR == currentDate.Year && l.STOREID == storeid);
                if (licenses != null && licenses.Any())
                {
                    _dbContext.SYSTEM_BIZ_LICENSE.RemoveRange(licenses);
                    found = true;
                }

                var details = _dbContext.SYSTEM_BIZ_LIC_DETAIL.Where(d => d.BIZID == businessid && d.MONTH == month && d.YEAR == currentDate.Year && d.STOREID == storeid);
                if (details != null && details.Any())
                {
                    _dbContext.SYSTEM_BIZ_LIC_DETAIL.RemoveRange(details);
                    found = true;
                }

                if (found)
                {
                    _dbContext.SaveChanges();
                    _log.LogInformation($"LicenseLogic.DeleteExisting SUCCEED.");
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"LicenseLogic.DeleteExisting {found} : {ex.Message}");
            }
        }

        private static string GetBluredAmt(decimal? value)
        {
            var saleamt = string.Empty;
            char[] amtstring = string.Format("{0:#,0.#}", float.Parse(Convert.ToString(value))).ToCharArray();

            for (int u = 0; u < amtstring.Length; u++)
            {
                if (u == RandomData(amtstring.Length) || u == 2 || u == amtstring.Length - 1)
                    saleamt += amtstring[u];
                else if (amtstring[u] == ',')
                    saleamt += ',';
                else
                    saleamt += "*";
            }

            return saleamt;
        }

        public static IEnumerable<LicenseUser> GetTestLicenseUser(OracleDbContext _dbContext)
        {
            return from u in _dbContext.SYSTEM_TEST_LIC_USERS
                   where u.ROLEID != null && u.AGREEMENTDATE != null
                   select new LicenseUser
                   {
                       ID = u.ID,
                       ORGID = u.ORGID,
                       ROLEID = u.ROLEID,
                       OLDROLEID = u.OLDROLEID,
                       AGREEMENTDATE = u.AGREEMENTDATE,
                       ROLECHANGEDATE = u.ROLECHANGEDATE,
                       LOGCOUNT = u.LOGCOUNT,
                       REQUESTCOUNT = u.REQUESTCOUNT,
                       DELETED = u.DELETED,
                       DELETEDDATE = u.DELETEDDATE,
                       RESTORED = u.RESTORED,
                       RESTOREDDATE = u.RESTOREDDATE
                   };
        }

        public static IEnumerable<LicenseUser> GetTestLicenseUser(OracleDbContext _dbContext, int orgid)
        {
            return from u in _dbContext.SYSTEM_TEST_LIC_USERS
                   where u.ORGID == orgid && u.ROLEID != null && u.AGREEMENTDATE != null
                   select new LicenseUser
                   {
                       ID = u.ID,
                       ORGID = u.ORGID,
                       ROLEID = u.ROLEID,
                       OLDROLEID = u.OLDROLEID,
                       AGREEMENTDATE = u.AGREEMENTDATE,
                       ROLECHANGEDATE = u.ROLECHANGEDATE,
                       LOGCOUNT = u.LOGCOUNT,
                       REQUESTCOUNT = u.REQUESTCOUNT,
                       DELETED = u.DELETED,
                       DELETEDDATE = u.DELETEDDATE,
                       RESTORED = u.RESTORED,
                       RESTOREDDATE = u.RESTOREDDATE
                   };
        }

        public static IEnumerable<LicenseUser> GetLicenseUsers(OracleDbContext _dbContext, ILogger _log, int storeid, DateTime sdate, DateTime edate)
        {
            var pstoreid = new OracleParameter("P_STOREID", OracleDbType.Int16, storeid, ParameterDirection.Input);
            var porgid = new OracleParameter("P_ORGID", OracleDbType.Int16, 0, ParameterDirection.Input);
            var psdate = new OracleParameter("P_SDATE", OracleDbType.Varchar2, sdate.ToString("yyyy-MM-dd"), ParameterDirection.Input);
            var pedate = new OracleParameter("P_EDATE", OracleDbType.Varchar2, edate.ToString("yyyy-MM-dd"), ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            return _dbContext.Database.SqlQuery<LicenseUser>("BEGIN SYSTEM_LICENSE_DATA(:P_STOREID, :P_ORGID, :P_SDATE, :P_EDATE, :RETURN_VALUE); END;",
                pstoreid, porgid, psdate, pedate, retvalue);
        }

        public static IEnumerable<LicenseUser> GetLicenseUsers(OracleDbContext _dbContext, ILogger _log, int storeid, int orgid, DateTime sdate, DateTime edate)
        {
            var pstoreid = new OracleParameter("P_STOREID", OracleDbType.Int16, storeid, ParameterDirection.Input);
            var porgid = new OracleParameter("P_ORGID", OracleDbType.Int16, orgid, ParameterDirection.Input);
            var psdate = new OracleParameter("P_SDATE", OracleDbType.Varchar2, sdate.ToString("yyyy-MM-dd"), ParameterDirection.Input);
            var pedate = new OracleParameter("P_EDATE", OracleDbType.Varchar2, edate.ToString("yyyy-MM-dd"), ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            return _dbContext.Database.SqlQuery<LicenseUser>("BEGIN SYSTEM_LICENSE_DATA(:P_STOREID, :P_ORGID, :P_SDATE, :P_EDATE, :RETURN_VALUE); END;",
                pstoreid, porgid, psdate, pedate, retvalue);
        }

        public static List<LicenseUser> GetLicenseUsers(OracleDbContext _dbContext, ILogger _log, DateTime sdate, DateTime edate)
        {
            var accessedusers = from l in _dbContext.SYSTEM_USER_ACTION_LOG
                                where sdate <= l.LOGDATE && l.LOGDATE <= edate
                                group l by l.USERID into g
                                select new
                                {
                                    USERID = g.Key,
                                    LOGCOUNT = g.Count()
                                };

            _log.LogInformation($"GetLicenseUsers.accessedusers : {(accessedusers == null ? 0 : accessedusers.Count())}");
            if (accessedusers == null || !accessedusers.Any())
                return new List<LicenseUser>();

            var companyusers = from u in _dbContext.SYSTEM_USERS
                               join a in accessedusers on u.ID equals a.USERID
                               where u.ROLEID != null && u.AGREEMENTDATE != null
                               select new
                               {
                                   ID = u.ID,
                                   ORGID = u.ORGID,
                                   ROLEID = u.ROLEID,
                                   OLDROLEID = u.OLDROLEID,
                                   AGREEMENTDATE = u.AGREEMENTDATE,
                                   ROLECHANGEDATE = u.ROLECHANGEDATE,
                                   LOGCOUNT = a.LOGCOUNT
                               };

            _log.LogInformation($"GetLicenseUsers.companyusers : {(companyusers == null ? 0 : companyusers.Count())}");
            if (companyusers == null || !companyusers.Any())
                return new List<LicenseUser>();

            var minDate = new DateTime(2000, 1, 1);
            var month = sdate.ToString("MM");
            var statusLogs = from u in companyusers
                             join s in _dbContext.SYSTEM_USER_STATUS_LOG on u.ID equals s.USERID
                             where s.LOGMONTH == month && s.LOGYEAR == sdate.Year
                             group s by s.USERID into g
                             select new
                             {
                                 ID = g.Key,
                                 COUNT = g.Count(),
                                 RESTOREDCOUNT = g.Sum(r => r.ENABLED == 1 ? 1 : 0),
                                 DELETEDCOUNT = g.Sum(r => r.ENABLED == 1 ? 0 : 1),
                                 RESTOREDATE = g.Max(r => r.ENABLED == 1 ? r.LOGDATE : minDate),
                                 DELETEDDATE = g.Max(r => r.ENABLED == 1 ? minDate : r.LOGDATE)
                             };

            _log.LogInformation($"GetLicenseUsers.statusLogs : {(statusLogs == null ? 0 : statusLogs.Count())}");
            var omg = statusLogs.ToList();
            //if (statusLogs == null || !statusLogs.Any())
            //    return companyusers.ToList();

            var completeusers = from u in companyusers
                                join l in statusLogs.ToList() on u.ID equals l.ID into lu
                                from a in lu.DefaultIfEmpty()
                                select new LicenseUser()
                                {
                                    ID = u.ID,
                                    ORGID = u.ORGID,
                                    AGREEMENTDATE = u.AGREEMENTDATE,
                                    ROLEID = u.ROLEID,
                                    OLDROLEID = u.OLDROLEID,
                                    ROLECHANGEDATE = u.ROLECHANGEDATE,
                                    LOGCOUNT = u.LOGCOUNT,
                                    DELETED = a.DELETEDCOUNT,
                                    DELETEDDATE = a.DELETEDDATE,
                                    RESTORED = a.RESTOREDCOUNT,
                                    RESTOREDDATE = a.RESTOREDATE
                                };

            _log.LogInformation($"GetLicenseUsers.completeusers : {(completeusers == null ? 0 : completeusers.Count())}");
            //return completeusers == null ? companyusers.ToList() : completeusers.ToList();
            return completeusers.ToList();
        }

        public static List<LicenseCompany> GetTestLicenseCompanies(OracleDbContext _dbContext, ILogger _log, List<LicenseUser> users)
        {
            var licensecompanies = users.Select(u => u.ORGID).Distinct().ToList();
            var companies = from c in _dbContext.SYSTEM_TEST_LIC_ORG
                            join l in licensecompanies on c.ID equals l
                            select new LicenseCompany()
                            {
                                ID = c.ID,
                                REGNO = c.REGNO,
                                ctrcnt = c.CTRCNT,
                                invamt = c.INVAMT,
                                payctrcd = c.PAYCTRCD,
                                paycycle = c.PAYCYCLE,
                                payjumcd = c.PAYJUMCD,
                                skucnt = c.SKUCNT
                                //SaleDesc = GetBluredAmt(c.INVAMT),
                                //MaxUserPrice = GetPercentValue(c.INVAMT, percent)
                            };

            return (companies == null) ? new List<LicenseCompany>() : companies.ToList();
        }

        public static List<LicenseCompany> GetLicenseCompanies(OracleDbContext _dbContext, ILogger _log,
            int storeid, DateTime sdate, DateTime edate, List<LicenseUser> companyusers)
        {
            var licensecompanies = companyusers.Select(u => u.ORGID).Distinct().ToList();

            var companies = from c in _dbContext.SYSTEM_ORGANIZATION.ToList()
                            join l in licensecompanies on c.ID equals l
                            where c.ORGTYPE == Enums.SystemEnums.ORGTYPE.Бизнес
                            select new LicenseCompany()
                            {
                                ID = c.ID,
                                REGNO = c.REGNO
                            };

            //if (companies == null || !companies.Any())
            //    return new List<LicenseCompany>();

            var companyList = companies.ToList();
            var reglist = companies.Select(x => new { data = x.REGNO });
            var regnolist = JsonConvert.SerializeObject(reglist.ToList());

            var restUtils = new HttpRestUtils(storeid, _dbContext);
            if (!restUtils.StoreServerConnected)
            {
                _log.LogWarning("LicenseLogic.GetLicenseCompanies StoreServerConnected=false");
                return companyList;
            }

            var response = restUtils.Post($"/api/licensedata/getvariables/{sdate.ToString("yyyyMM")}", regnolist).Result;
            if (!response.Success)
            {
                _log.LogWarning($"LicenseLogic.GetLicenseCompanies getvariables={response.Message}");
                return companyList;
            }

            var jsonData = Convert.ToString(response.Value);
            var licenses = JsonConvert.DeserializeObject<List<LicenseVariables>>(jsonData);

            if (licenses == null || !licenses.Any())
            {
                _log.LogWarning("LicenseLogic.GetLicenseCompanies licenses=0");
                return companyList;
            }

            foreach (var company in companyList)
            {
                var license = licenses.FirstOrDefault(l => company.REGNO.Equals(l.REGNO));
                if (license == null) continue;

                company.FillLicense(license);
                //company.SaleDesc = GetBluredAmt(license.invamt);
                //company.MaxUserPrice = GetPercentValue(license.invamt.Value, percent);
            }

            return companyList;
        }

        private static decimal GetPercentValue(decimal value, int percent)
        {
            return Math.Round(value * percent / 100, 0, MidpointRounding.AwayFromZero);
        }

        internal class CalculateResult
        {
            public int Count { get; set; }
            public decimal Total { get; set; }
        }

        public static ResponseClient Calculate(
            OracleDbContext _dbContext, ILogger _log, 
            int storeid, int maxPricePercent, decimal minAmount,
            bool sendMessage, DateTime sdate, DateTime edate,
            List<LicenseCompany> companies, List<LicenseUser> users,
            List<SYSTEM_LIC_CONT_CONFIG> contConfigs,
            List<SYSTEM_LIC_SALE_CONFIG> saleConfigs,
            List<SYSTEM_LIC_SKU_CONFIG> skuConfigs,
            List<SYSTEM_ROLE_CONFIG> configs)
        {
            var logMethod = "LicenseLogic.Calculate";
            try
            {
                var response = new CalculateResult() { Count = 0, Total = 0 };
                var month = sdate.ToString("MM");
                List<LicenseUser> currentUsers = null;
                foreach (var currentcompany in companies)
                {
                    if (currentcompany.invamt <= 0)
                    {
                        _log.LogWarning($"{logMethod} NO SALE : {currentcompany.REGNO} => {currentcompany.invamt}");
                        continue;
                    }

                    currentUsers = users.Where(u => u.ORGID == currentcompany.ID).ToList();
                    if (!currentUsers.Any()) continue;
                    var company = SetScore(currentcompany, maxPricePercent, minAmount, contConfigs, saleConfigs, skuConfigs);
                    
                    var result = GetAttributes(_dbContext, _log, storeid,
                        sdate, edate, company, currentUsers, configs);

                    if (result.AMOUNT.Value > 0)
                    {
                        response.Count++;
                        response.Total += result.AMOUNT.Value;
                    }
                    
                    var current = _dbContext.SYSTEM_BIZ_LICENSE.FirstOrDefault(
                        x => x.BUSINESSID == company.ID && x.STOREID == storeid
                        && x.YEAR == sdate.Year && x.MONTH == month);

                    if (current != null)
                    {
                        Fill(_log, current, company, result, minAmount);

                        Update(_dbContext, current, false);
                        _log.LogInformation($"{logMethod} {company.REGNO} => {current.AMOUNT} UPDATE");
                    }
                    else
                    {
                        var newLicense = new SYSTEM_BIZ_LICENSE()
                        {
                            YEAR = sdate.Year,
                            MONTH = sdate.ToString("MM"),
                            CREATEDATE = DateTime.Now,
                            PAYED = 0,
                            STOREID = storeid
                        };

                        Fill(_log, newLicense, company, result, minAmount);

                        Insert(_dbContext, newLicense, false);
                        _log.LogInformation($"{logMethod} {company.REGNO} => {newLicense.AMOUNT} INSERT");
                    }

                }
                _dbContext.SaveChanges();

                var amount = string.Format("{0:#,0.#}", float.Parse(Convert.ToString(response.Total)));
                _log.LogInformation($"{logMethod} TOTAL : {response.Count} => {amount}");
                if (sendMessage)
                    MailLogic.AddSMS(_dbContext, Enums.SystemEnums.MessageType.None, 
                        $"{amount} duntei {response.Count} shirheg license bodogdow. www.urto.mn",
                        Controllers.SendData.Messager.DEVELOPER_PHONE_NO);

                return ReturnResponce.SuccessMessageResponce(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _log.LogError($"{logMethod} ERROR : {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        private static void Fill(ILogger _log, SYSTEM_BIZ_LICENSE current, LicenseCompany company, SYSTEM_BIZ_LICENSE calculated, decimal minAmount)
        {
            // ХЭРВЭЭ ТУХАЙН АЙЛЫН БОРЛУУЛАЛТ 
            // ТОХИРГООНООС БАГА ДҮНТЭЙ БОЛ ТӨЛБӨР АВАХГҮЙ /30000/
            if (calculated.AMOUNT != 0 && company.invamt < minAmount)
            {
                _log.LogWarning($"LicenseLogic.Fill {company.ID} : {calculated.AMOUNT}");
                current.AMOUNT = 0;
            }
            else
                current.AMOUNT =  calculated.AMOUNT;

            current.ACTUALAMOUNT = calculated.ACTUALAMOUNT;
            current.SCORE = calculated.SCORE;
            current.USERCNT = calculated.USERCNT;

            current.SKUCNT = company.skucnt;
            current.CONTRACTCNT = Convert.ToInt32(company.ctrcnt);
            current.PAYCTRCD = company.payctrcd;
            current.PAYCYCLE = company.paycycle;
            current.PAYJUMCD = company.payjumcd;

            current.BUSINESSID = company.ID;
            current.SKUSCORE = company.SKUSCORE;
            current.CONTRACTSCORE = company.CONTRACTSCORE;
            current.SALESCORE = company.SALESCORE;
            current.SALEAMT = company.SaleDesc;
        }


        private static int RandomData(int end)
        {
            var rndm = new Random();
            return rndm.Next(0, end);
        }

        private static LicenseCompany SetScore(
            LicenseCompany company,
            int maxPricePercent,
            decimal minAmount,
            List<SYSTEM_LIC_CONT_CONFIG> contConfigs, 
            List<SYSTEM_LIC_SALE_CONFIG> saleConfigs,
            List<SYSTEM_LIC_SKU_CONFIG> skuConfigs)
        {
            var currentcontract = contConfigs.FirstOrDefault(x => x.MINVALUE <= company.ctrcnt && x.MAXVALUE >= company.ctrcnt);
            company.CONTRACTSCORE = currentcontract != null ? currentcontract.SCORE.Value : 0;

            var currentsku = skuConfigs.FirstOrDefault(x => x.MINVALUE <= company.skucnt && x.MAXVALUE >= company.skucnt);
            company.SKUSCORE = currentsku != null ? currentsku.SCORE.Value : 0;

            var currentsale = saleConfigs.FirstOrDefault(x => x.MINVALUE <= company.invamt && x.MAXVALUE >= company.invamt);
            company.SALESCORE = currentsale != null ? currentsale.SCORE.Value : 0;

            company.TOTALSCORE = company.CONTRACTSCORE + company.SKUSCORE + company.SALESCORE;
            
            company.SaleDesc = GetBluredAmt(company.invamt);
            company.MaxUserPrice = GetPercentValue(company.invamt, maxPricePercent);
            // ХЭРВЭЭ БАЙГУУЛЛАГЫН БОРЛУУЛАЛТ 30000-С ХЭТРЭХГҮЙ БОЛ ТӨЛБӨР АВАХГҮЙ
            company.NoPayment = company.invamt < minAmount;

            return company;
        }

        public static SYSTEM_BIZ_LICENSE GetAttributes(
            OracleDbContext _dbContext, ILogger _log, 
            int storeid, DateTime sdate, DateTime edate,
            LicenseCompany company, List<LicenseUser> users, 
            List<SYSTEM_ROLE_CONFIG> configs)
        {
            int standartdaycount = Convert.ToInt32((edate - sdate).TotalDays);
            var month = sdate.ToString("MM");
            //var oldValues = _dbContext.SYSTEM_BIZ_LIC_DETAIL.Where(x => x.BIZID == company.ID && x.STOREID == storeid
            //    && x.YEAR == sdate.Year && x.MONTH == month);
            //if (oldValues != null && oldValues.Any())
            //    _dbContext.SYSTEM_BIZ_LIC_DETAIL.RemoveRange(oldValues);

            var result = new SYSTEM_BIZ_LICENSE()
            {
                SCORE = company.TOTALSCORE,
                AMOUNT = 0,
                ACTUALAMOUNT = 0,
                USERCNT = 0
            };


            foreach (var user in users)
            {
                var startdate = new DateTime(sdate.Year, sdate.Month, sdate.Day);
                var enddate = new DateTime(edate.Year, edate.Month, edate.Day, edate.Hour, edate.Minute, edate.Second);

                // user.LOGCOUNT == 0 || 
                if (enddate < user.AGREEMENTDATE || user.REQUESTCOUNT == 0)
                {
                    _log.LogWarning($"LicenseLogic.GetAttributes NO PAYMENT : {user.ID} {user.AGREEMENTDATE} {user.LOGCOUNT} {user.REQUESTCOUNT}");
                    continue;
                }

                var oldDetail = new AboutLicenseDetail() { Duration = 0 };
                var newDetail = new AboutLicenseDetail() { Duration = 0 };

                if (startdate <= user.AGREEMENTDATE && user.AGREEMENTDATE <= enddate)
                {
                    // ЗӨВШӨӨРСӨН ОГНООНООС ТООЦНО.
                    startdate = new DateTime(user.AGREEMENTDATE.Value.Year, user.AGREEMENTDATE.Value.Month, user.AGREEMENTDATE.Value.Day);
                }

                if ((user.RESTORED + user.DELETED) == 1)
                {
                    if (user.RESTORED == 1)
                    {
                        // СЭРГЭЭСЭН ӨДРӨӨС ХОЙШ САР ДУУСТАЛ ТООЦНО.
                        //var date = statusLogs.FirstOrDefault(l => l.USERID == user.ID && l.ENABLED == 1).LOGDATE;
                        var date = user.RESTOREDDATE.Value;
                        if (startdate <= date && date <= enddate)
                            startdate = new DateTime(date.Year, date.Month, date.Day);
                    }
                    else
                    {
                        // САРЫН ЭХНЭЭС УСТГАСАН ОГНОО ХҮРТЭЛ ТООЦНО.
                        //var date = statusLogs.FirstOrDefault(l => l.USERID == user.ID && l.ENABLED == 0).LOGDATE;
                        var date = user.DELETEDDATE.Value;
                        if (startdate <= date && date <= enddate)
                            enddate = new DateTime(date.Year, date.Month, date.Day);
                    }
                }

                if ((user.RESTORED + user.DELETED) > 1)
                {
                    // САРД НЭГ БОЛОН ТҮҮНЭЭС ОЛОН СЭРГЭЭЖ УСТГАСАН БОЛ ТУХАЙН САРЫГ БҮХЭЛД НЬ ТООЦНО.
                    newDetail.Fill(startdate, enddate);
                }
                else
                {
                    if (user.OLDROLEID.HasValue)
                    {
                        // ХЭРВЭЭ ЭХЛЭХ БОЛОН ДУУСАХ ОГНООТОЙ ТЭНЦҮҮ ЭСВЭЛ ЭХЛЭХЭЭС БАГА
                        // ЭСВЭЛ ДУУСАХААС ИХ БОЛ 1 ROLCHANGE ХИЙСЭН ГЭЖ ҮЗЭХГҮЙ
                        if (user.ROLECHANGEDATE < startdate
                            || startdate.ToString("yyyyMMdd") == user.ROLECHANGEDATE.Value.ToString("yyyyMMdd")
                            || enddate.ToString("yyyyMMdd") == user.ROLECHANGEDATE.Value.ToString("yyyyMMdd")
                            || enddate < user.ROLECHANGEDATE)
                        {
                            newDetail.Fill(startdate, enddate);
                        }
                        else if (startdate < user.ROLECHANGEDATE && user.ROLECHANGEDATE < enddate)
                        {
                            oldDetail.Fill(startdate, user.ROLECHANGEDATE.Value.Date);
                            newDetail.Fill(user.ROLECHANGEDATE.Value.Date, enddate);
                        }
                    }
                    else
                    {
                        newDetail.Fill(startdate, enddate);
                    }

                    //if (!sameDay && user.OLDROLEID.HasValue && startdate < user.ROLECHANGEDATE && user.ROLECHANGEDATE <= enddate)
                    //{
                    //    oldDetail.Fill(startdate, user.ROLECHANGEDATE.Value.Date);
                    //    newDetail.Fill(user.ROLECHANGEDATE.Value.Date, enddate);
                    //}
                    //else if (user.ROLECHANGEDATE < startdate || sameDay)
                    //{
                    //    newDetail.Fill(startdate, enddate);
                    //}
                }

                result.USERCNT++;

                if (oldDetail.Duration > 0)
                {
                    //userprice = configs.FirstOrDefault(x => x.MINSCORE <= allscore && x.MAXSCORE >= allscore && x.ROLEID == oldroleid).PRICE;
                    //var licprice = Convert.ToDecimal(userprice / standartdaycount * oldroledaycount);
                    var detail = GetPrice(configs, Convert.ToInt16(user.OLDROLEID),
                        company, standartdaycount, oldDetail, user, storeid);

                    result.ACTUALAMOUNT += detail.ACTUALPRICE;
                    // БОРЛУУЛАЛТЫН ДҮНГИЙН 4 ХУВИАС ХЭТРЭХГҮЙ
                    if (company.MaxUserPrice > 0 && detail.LICPRICE.Value > 0
                        && company.MaxUserPrice < result.AMOUNT + detail.LICPRICE.Value)
                    {
                        _log.LogWarning($"LicenseLogic.GetAttributes {company.ID} : {user.ID} => {result.AMOUNT} + {detail.LICPRICE.Value} <= {company.MaxUserPrice}");
                        detail.LICPRICE = company.MaxUserPrice - result.AMOUNT <= 0 ? 0 : company.MaxUserPrice - result.AMOUNT;
                    }

                    //userPrice += detail.LICPRICE.Value;
                    result.AMOUNT += detail.LICPRICE.Value;
                    _dbContext.SYSTEM_BIZ_LIC_DETAIL.Add(detail);
                }

                if (newDetail.Duration > 0)
                {
                    //userprice = configs.FirstOrDefault(x => x.MINSCORE <= allscore && x.MAXSCORE >= allscore && x.ROLEID == newroleid).PRICE;
                    //var licprice = Convert.ToDecimal(userprice / standartdaycount * newroledaycount);
                    var detail = GetPrice(configs, Convert.ToInt16(user.ROLEID),
                        company, standartdaycount, newDetail, user, storeid);

                    result.ACTUALAMOUNT += detail.ACTUALPRICE;
                    // БОРЛУУЛАЛТЫН ДҮНГИЙН 4 ХУВИАС ХЭТРЭХГҮЙ
                    if (company.MaxUserPrice > 0 && detail.LICPRICE.Value > 0
                        && company.MaxUserPrice < result.AMOUNT + detail.LICPRICE.Value)
                    {
                        _log.LogWarning($"LicenseLogic.GetAttributes {company.ID} : {user.ID} => {result.AMOUNT} + {detail.LICPRICE.Value} <= {company.MaxUserPrice}");
                        detail.LICPRICE = company.MaxUserPrice - result.AMOUNT <= 0 ? 0 : company.MaxUserPrice - result.AMOUNT;
                    }

                    //userPrice += detail.LICPRICE.Value;
                    result.AMOUNT += detail.LICPRICE.Value;
                    _dbContext.SYSTEM_BIZ_LIC_DETAIL.Add(detail);
                }

                //result.AMOUNT += userPrice;
            }
            return result;
        }

        internal class AboutLicenseDetail
        {
            public int Duration { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            public void Fill(DateTime sdate, DateTime edate)
            {
                this.Duration = Convert.ToInt32((edate - sdate).TotalDays);
                this.StartDate = sdate;
                this.EndDate = edate;
            }
        }


        private static decimal GetPrice(List<SYSTEM_ROLE_CONFIG> configs, int roleid, int score)
        {
            var config = configs.FirstOrDefault(x => x.ROLEID == roleid && x.MINSCORE <= score && x.MAXSCORE >= score);
            return config == null ? 0 : config.PRICE.Value;
        }

        private static SYSTEM_BIZ_LIC_DETAIL GetPrice(
            List<SYSTEM_ROLE_CONFIG> configs, int roleid, LicenseCompany company,
            int total, AboutLicenseDetail license, LicenseUser user, 
            int storeid)
        {
            var configprice = GetPrice(configs, roleid, company.TOTALSCORE);
            var price = Math.Round(configprice * license.Duration / total, 0, MidpointRounding.AwayFromZero);

            return new SYSTEM_BIZ_LIC_DETAIL()
            {
                ID = Convert.ToString(Guid.NewGuid()),
                BIZID = user.ORGID,
                USERID = user.ID,
                YEAR = license.StartDate.Year,
                MONTH = license.StartDate.ToString("MM"),
                DAYCOUNT = license.Duration,
                ROLEID = roleid,
                LOGCOUNT = user.LOGCOUNT,
                REQUESTCOUNT = user.REQUESTCOUNT,
                SCORE = company.TOTALSCORE,
                LICPRICE = company.NoPayment ? 0 : price,
                ACTUALPRICE = price,
                STOREID = storeid,
                STATUSCOUNT = user.RESTORED + user.DELETED,
                STARTDATE = license.StartDate,
                ENDDATE = license.EndDate
            };
        }
    }
}
