using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Context;
using EDIWEBAPI.Utils;
using Newtonsoft.Json;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Controllers.Storeapi;
using System.Globalization;
using Microsoft.Extensions.Logging;
using EDIWEBAPI.Entities.DBModel.Payment;
using EDIWEBAPI.Entities.DBModel.SystemManagement;

namespace EDIWEBAPI.Logics
{
    public class PaymentLogic : BaseLogic
    {
        public static REQ_PAYMENT GetPayment(OracleDbContext _context, string contractNo, string STPYMD)
        {
            return _context.REQ_PAYMENT.FirstOrDefault(x => x.CONTRACTNO == contractNo && x.STPYMD == STPYMD);
        }

        private static DateTime GetWorkDay(DateTime currdate)
        {
            var todaybusinessdate = new DateTime();

            if (currdate.DayOfWeek == DayOfWeek.Sunday)
                todaybusinessdate = currdate.AddDays(1);
            else if (currdate.DayOfWeek == DayOfWeek.Saturday)
                todaybusinessdate = currdate.AddDays(2);
            else
                todaybusinessdate = currdate;

            return todaybusinessdate;
        }

        private static DateTime[] GetEndDate(DateTime currdate, SYSTEM_STORECYCLE_CONFIG config)
        {
            var todaybusinessdate = GetWorkDay(currdate);

            var cyclestartdate = new DateTime(todaybusinessdate.Year, todaybusinessdate.Month, Convert.ToInt32(config.STARTDAY));
            var cycleenddate = new DateTime(todaybusinessdate.Year, todaybusinessdate.Month, Convert.ToInt32(config.STARTDAY)).AddDays(config.DURATION);

            int wknddaycount = 0;
            for (DateTime date = cyclestartdate; date <= cycleenddate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                    wknddaycount++;
            }

            return new DateTime[] { cyclestartdate, GetWorkDay(cycleenddate.AddDays(wknddaycount)) };
        }
        private static List<string> GetDayNames(string name, int duration)
        {
            var index = 0;
            if (DayOfWeek.Monday.ToString().ToUpper() == name.ToUpper())
                index = 1;
            else if (DayOfWeek.Tuesday.ToString().ToUpper() == name.ToUpper())
                index = 2;
            else if (DayOfWeek.Wednesday.ToString().ToUpper() == name.ToUpper())
                index = 3;
            else if (DayOfWeek.Thursday.ToString().ToUpper() == name.ToUpper())
                index = 4;
            else if (DayOfWeek.Friday.ToString().ToUpper() == name.ToUpper())
                index = 5;
            else if (DayOfWeek.Saturday.ToString().ToUpper() == name.ToUpper())
                index = 6;
            else if (DayOfWeek.Sunday.ToString().ToUpper() == name.ToUpper())
                index = 7;

            var names = new string[]
            {
                DayOfWeek.Monday.ToString().ToUpper(), DayOfWeek.Tuesday.ToString().ToUpper(),
                DayOfWeek.Wednesday.ToString().ToUpper(), DayOfWeek.Thursday.ToString().ToUpper(),
                DayOfWeek.Friday.ToString().ToUpper(), DayOfWeek.Saturday.ToString().ToUpper(),
                DayOfWeek.Sunday.ToString().ToUpper()
            };

            var days = new List<string>();
            for (int i = index; i <= duration + index; i++)
                days.Add(names[(i > 7 ? i - 7 : i) - 1]);

            return days;
        }

        public static bool CheckCycle(OracleDbContext _context, ILogger _logger, int storeid, string cycleindex, DateTime currdate)
        {
            try
            {
                var currentdata = _context.SYSTEM_STORECYCLE_CONFIG.Where(x => x.CYCLEINDEX == cycleindex && x.STOREID == storeid);
                if (currentdata == null)
                    return false;

                if (currentdata.ToList().Count == 1)
                {
                    var currentcycle = currentdata.ToList<SYSTEM_STORECYCLE_CONFIG>();
                    if (currentcycle[0].DAYTYPE == 0)
                    {
                        string[] daynames = currentcycle[0].DAYNAMES.ToUpper().Split(',');
                        if (daynames.Length == 0)
                            return false;

                        var days = GetDayNames(daynames[0], currentcycle[0].DURATION);
                        return days.Contains(currdate.DayOfWeek.ToString().ToUpper());
                    }
                    else
                    {
                        var dates = GetEndDate(currdate, currentcycle[0]);
                        return currdate >= dates[0] && currdate <= dates[1];
                    }

                }
                else
                {
                    bool found = false;
                    foreach (SYSTEM_STORECYCLE_CONFIG currdata in currentdata.ToList())
                    {
                        var dates = GetEndDate(currdate, currdata);
                        if (currdate >= dates[0] && currdate <= dates[1])
                        {
                            found = true;
                            break;
                        }
                    }
                    return found;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"PaymentLogic.CheckCycle : {UsefulHelpers.GetExceptionMessage(ex)}");
                return false;
            }
        }

        public static ResponseClient UpdatePayment(OracleDbContext _context, int storeId, DateTime edate, string ctrcd, string status)
        {
            var restUtils = new HttpRestUtils(storeId, _context);
            if (!restUtils.StoreServerConnected)
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

            return restUtils.Get($"/api/paymentdata/updatepayment/{edate.ToString("yyyy-MM-dd")}/{ctrcd}/{status}").Result;
        }

        public static ResponseClient GetPaymentReport(OracleDbContext _dbContext, ILogger _logger, int storeId, string regNo, DateTime beginDate, DateTime endDate, bool attached)
        {
            var payments = new List<REQ_PAYMENT_REPORT>();
            if (UsefulHelpers.IsNull(regNo))
            {
                var values = _dbContext.REQ_PAYMENT_REPORT.Where(x => x.COMID == storeId && beginDate <= x.APPROVEDATE && x.APPROVEDATE <= endDate 
                    && (attached ? x.ATTACHDATE != null : 1 == 1));

                if (values != null && values.Any())
                    payments = values.ToList();
            }
            else
            {
                var values = _dbContext.REQ_PAYMENT_REPORT.Where(x => x.REGNO == regNo && x.COMID == storeId && beginDate <= x.APPROVEDATE && x.APPROVEDATE <= endDate
                    && (attached ? x.ATTACHDATE != null : 1 == 1));

                if (values != null && values.Any())
                    payments = values.ToList();
            }

            if (!payments.Any())
                return ReturnResponce.NotFoundResponce();

            var query = from reports in payments.ToList()
                        join companys in _dbContext.SYSTEM_ORGANIZATION.ToList()
                        on reports.REGNO equals companys.REGNO into su
                        from allreports in su.DefaultIfEmpty()

                        join users in _dbContext.SYSTEM_USERS.ToList()
                        on reports.ATTACHUSER equals users.ID into employee
                        from attacheduser in employee.DefaultIfEmpty()

                        join approveduser in _dbContext.SYSTEM_USERS.ToList()
                        on reports.APPROVEDUSER equals approveduser.ID into approvedemployee
                        from appuser in approvedemployee.DefaultIfEmpty()
                        select new PaymentReportDto()
                        {
                            ID = reports.ID,
                            REGNO = reports.REGNO,
                            COMPANYNAME = allreports?.COMPANYNAME,
                            ATTACHDATE = reports.ATTACHDATE,
                            ATTACHFILE = reports.ATTACHFILE,
                            DESCRIPTION = reports.DESCRIPTION,
                            AMOUNT = reports.AMOUNT,
                            APPROVEDATE = reports.APPROVEDATE,
                            STARTDATE = reports.STARTDATE,
                            ENDDATE = reports.ENDDATE,
                            STOREPRINT = reports.STOREPRINT,
                            attacheduser = attacheduser?.FIRSTNAME,
                            approveduser = appuser?.FIRSTNAME
                        };

            return ReturnResponce.ListReturnResponce(query.ToList());
        }

        public static ResponseClient GetPayment(OracleDbContext _dbContext, ILogger _logger,
            int storeid, int companyid, string regno, string contractno,
            DateTime sdate, DateTime edate)
        {
            contractno = UsefulHelpers.ReplaceNull(contractno);

            ContractLogic.Modify(_dbContext, _logger, storeid, regno);

            var restUtils = new HttpRestUtils(storeid, _dbContext);
            if (!restUtils.StoreServerConnected)
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

            var response = restUtils.Get($"/api/paymentdata/paymentheader/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}").Result;
            if (!response.Success || response.Value == null)
                return response;

            var jsonData = Convert.ToString(response.Value);
            var headers = JsonConvert.DeserializeObject<List<PaymentHeader>>(jsonData);
            if (headers == null || headers.Count == 0)
                return ReturnResponce.NotFoundResponce();

            //_logger.LogInformation($"PaymentLogic.GetPayment : {regno}");

            var organization = ManagementLogic.GetOrganization(_dbContext, companyid);

            var contracts = from c in _dbContext.MST_CONTRACT.ToList()
                            where c.BUSINESSID == companyid && c.STOREID == storeid && (contractno == "%" || c.CONTRACTNO == contractno)
                            select c.CONTRACTNO;

            var requests = from pymt in _dbContext.REQ_PAYMENT.ToList()
                           join disabled in _dbContext.REQ_PAYMENT_DISABLED.ToList() on pymt.ID equals disabled.PAYMENTID into dilj
                           from dlj in dilj.DefaultIfEmpty()

                           join c in contracts on pymt.CONTRACTNO equals c
                           join users in _dbContext.SYSTEM_USERS on pymt.APPROVEDUSER equals users?.ID into su
                           from appuser in su.DefaultIfEmpty()

                           select new
                           {
                               pymt.ID,
                               pymt.CONTRACTNO,
                               pymt.STPYMD,
                               pymt.APPROVEDDATE,
                               pymt.APPROVEDUSER,
                               pymt.ATTACHFILE,
                               pymt.ATTACHDATE,
                               pymt.DESCRIPTION,
                               pymt.STORESEEN,
                               appuser?.FIRSTNAME,
                               FAILED = pymt.APPROVEDDATE.HasValue && !pymt.ATTACHDATE.HasValue,
                               DISABLED = pymt.APPROVEDDATE.HasValue && !pymt.ATTACHDATE.HasValue && dlj != null,
                               DISABLEDREASON = pymt.APPROVEDDATE.HasValue && !pymt.ATTACHDATE.HasValue && dlj != null ? dlj.DESCRIPTION : string.Empty
                           };

            var query = (from header in headers.ToList()
                         join request in requests
                         on new { stopdate = header.stpymd, contractno = header.ctrcd }
                         equals new { stopdate = request.STPYMD, contractno = request.CONTRACTNO } into t
                         from rt in t.DefaultIfEmpty()
                         join contractslist in _dbContext.MST_CONTRACT.ToList() on header.ctrcd equals contractslist.CONTRACTNO into con
                         from contr in con.DefaultIfEmpty()
                             //join licensedat in licenseamount.ToList()
                             //on new { ctrcd = header.ctrcd, stpymd = header.stpymd }
                             // equals new { ctrcd = licensedat.ContractCode, stpymd = licensedat.stpymd } into licdata
                             //from licdatas in licdata.DefaultIfEmpty()

                         select new PaymentDto()
                         {
                             ID = rt?.ID,
                             CONTRACTNO = rt?.CONTRACTNO,
                             APPROVEDDATE = rt?.APPROVEDDATE,
                             APPROVEDUSER = rt?.FIRSTNAME,
                             ATTACHFILE = rt?.ATTACHFILE,
                             ATTACHDATE = rt?.ATTACHDATE,
                             DESCRIPTION = rt?.DESCRIPTION,
                             STORESEEN = rt?.STORESEEN,
                             TYPE = rt != null && rt.FAILED ? (rt.DISABLED ? 1 : 2) : 0,
                             REASON = rt != null && rt.FAILED ? (rt.DISABLED ? $"Цуцласан. {rt.DISABLEDREASON}" : "Хавсаргалт амжилтгүй болсон.") : string.Empty,

                             amt = header.amt,
                             ctrcd = header.ctrcd,
                             ctrnm = header.ctrnm,
                             edi = header.edi,
                             evnamt = header.evnamt,
                             frbtamt = header.frbtamt,
                             ifrbtamt = header.ifrbtamt,
                             normalstk = header.normalstk,
                             payamt = header.payamt,
                             //payamt = licdatas == null ? header.payamt : header.payamt - licdatas?.LicenseAMT,
                             PBGB = header.PBGB,
                             stpymd = header.stpymd,
                             strymd = header.strymd,
                             banknm = header.banknm,
                             accno = header.accno,
                             penamt = header.penamt,
                             crdfee = header.crdfee,
                             invamt = contr == null ? header.invamt : (contr.CONTRACTTYPE == 2 || contr.CONTRACTTYPE == 5) ? 0 : header.invamt, // 2 5 type iin shiveh dun 0 bh zaswar orow 23.06.27
                             paycycle = header.paycycle,
                             comid = organization.ID,
                             regno = organization.REGNO,
                             LICENSEAMOUNT = 0
                             //LICENSEAMOUNT = licdatas == null ? 0 : licdatas?.LicenseAMT
                         })
                         .Where(x => x.ctrcd == contractno || contractno == "%")
                         .OrderBy(x => x.ctrcd).ThenBy(x => x.stpymd);

            var result = query.Distinct().ToList();
            // ЛИЦЕНЗ САР ДУУСАХАД БОДОГДОХ ТУЛ ШАЛГАХ ШААРДЛАГАГҮЙ
            var license = LicenseLogic.GetLicenseData(_dbContext, companyid, edate);
            if (license != null)
            {
                // НИЙТ ГЭРЭЭН ДУНД ХАМГИЙН ИХ ДҮНТЭЙ ЛИЦЕНЗИЙН ТӨЛБӨР ХАРУУЛАХ НЬ
                var licenseHeader = headers.OrderByDescending(t => t.stpymd).ThenByDescending(x => x.payamt).First();
                // ШҮҮСЭН ГЭРЭЭН ДУНД ХАМГИЙН ИХ ДҮНТЭЙ ЛИЦЕНЗИЙН ТӨЛБӨР ХАРУУЛАХ НЬ
                var data = result.OrderByDescending(t => t.stpymd).ThenByDescending(x => x.payamt).First();
                // ЭНЭ 2 НЭГ ГЭРЭЭН ДЭЭР ТААРВАЛ ЛИЦЕНЗИЙН ТӨЛБӨРИЙГ ТУХАЙН ГЭРЭЭН ДЭЭР ХАРУУЛНА.
                // ХЭРВЭЭ ЯМАР НЭГ БАЙДЛААР ӨӨР ГЭРЭЭГЭЭР ШҮҮВЭЛ ЛИЦЕНЗИЙН ТӨЛБӨР ХАРУУЛАХГҮЙ
                if (licenseHeader.ctrcd == data.ctrcd)
                    data.SetLicenseAmount(license.AMOUNT.Value); // license.TOTAL.Value
            }

            //_logger.LogInformation($"PaymentLogic.GetPayment : {regno}");
            return ReturnResponce.ListReturnResponce(result);
        }

        public static ResponseClient GetStorePayment(OracleDbContext _dbContext, ILogger _logger, int storeid, string regno, string contractno, 
            DateTime sdate, DateTime edate, bool attached, bool approved)
        {
            bool contractNull = UsefulHelpers.IsNull(contractno);
            contractno = UsefulHelpers.ReplaceNull(contractno);
            var restUtils = new HttpRestUtils(storeid, _dbContext);

            if (!restUtils.StoreServerConnected)
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

            var headers = new List<PaymentHeader>();
            var filter = new PaymentFilter();
            if (!UsefulHelpers.IsNull(regno))
            {
                var response = restUtils.Get($"/api/paymentdata/paymentheader/{sdate.ToString("yyyy-MM-dd")}/{edate.ToString("yyyy-MM-dd")}/{regno}").Result;
                if (!response.Success)
                    return response;

                var jsonData = Convert.ToString(response.Value);
                headers = JsonConvert.DeserializeObject<List<PaymentHeader>>(jsonData);

                if (approved && attached)
                    filter.AttachType = 2;
                else if (approved && !attached)
                    filter.ApprovedType = 2;
            }
            else
            {
                if (approved == false && attached == false)
                    return ReturnResponce.NotFoundResponce();
                // APPROVED = FALSE, ATTACHED = FALSE ҮЕД ЗӨВХӨН РЕГИСТЕРЭЭР ШҮҮНЭ
                // БУСАД ҮЕД APPROVED = TRUE БАЙНА
                var contracts = (from p in _dbContext.REQ_PAYMENT
                                 where attached ? (sdate <= p.ATTACHDATE && p.ATTACHDATE <= edate) :
                                    (p.ATTACHDATE == null && sdate <= p.APPROVEDDATE && p.APPROVEDDATE <= edate)
                                 orderby p.STPYMD
                                 select new { p.CONTRACTNO, p.STPYMD }).ToList();

                if (contracts != null && contracts.Any())
                {
                    var startdate = UsefulHelpers.ConvertToDatetime(contracts.First().STPYMD);
                    var enddate = UsefulHelpers.ConvertToDatetime(contracts.Last().STPYMD);

                    var regnos = (from p in contracts
                                  join c in _dbContext.MST_CONTRACT on p.CONTRACTNO equals c.CONTRACTNO
                                  join o in _dbContext.SYSTEM_ORGANIZATION on c.BUSINESSID equals o.ID
                                  select new { data = o.REGNO }).Distinct().ToList();

                    var reglist = JsonConvert.SerializeObject(regnos);
                    var response = restUtils.Post($"/api/paymentdata/paymentstoreheader/{startdate.ToString("yyyy-MM-dd")}/{enddate.ToString("yyyy-MM-dd")}", reglist).Result;
                    if (!response.Success)
                        return response;

                    var jsonData = Convert.ToString(response.Value);
                    headers = JsonConvert.DeserializeObject<List<PaymentHeader>>(jsonData);
                }
                
                if (approved && attached)
                {
                    filter.AttachType = 3;
                    filter.AttachBeginDate = sdate;
                    filter.AttachEndDate = edate;
                }
                else if (approved && !attached)
                {
                    //filter.ApprovedType = 2;
                    filter.ApprovedType = 3;
                    filter.ApprovedBeginDate = sdate;
                    filter.ApprovedEndDate = edate;
                    filter.AttachType = 1;
                }
            }

            if (headers == null || !headers.Any())
                return ReturnResponce.ListReturnResponce(new List<PaymentDto>());

            var query = GetPayments(_dbContext, headers, contractno, filter);
            if (query.Any())
                SetLicenseAmount(_dbContext, _logger, query);

            return ReturnResponce.ListReturnResponce(query);
        }

        private static void SetLicenseAmount(OracleDbContext _context, ILogger _logger, List<PaymentDto> payments)
        {
            var orgazations = (from p in payments
                               where p.comid.HasValue
                               group p by new { COMID = p.comid.Value, YEARANDMONTH = p.YEARANDMONTH } into g
                               select new
                               {
                                   g.Key.COMID,
                                   YEARANDMONTH = g.Key.YEARANDMONTH
                               }).ToList();

            var licenses = from o in orgazations
                           join l in _context.SYSTEM_LICENSE_BUSINESS on
                                 new { BUSINESSID = o.COMID, MONTH = o.YEARANDMONTH }
                                 equals new { BUSINESSID = l.BUSINESSID, MONTH = l.YEARANDMONTH.ToString() }
                           select new
                           {
                               l.BUSINESSID,
                               l.YEAR,
                               l.MONTH,
                               o.YEARANDMONTH,
                               l.TOTAL,
                               l.AMOUNT
                           };

            foreach (var license in licenses)
            {
                var payment = payments.Where(a => a.comid == license.BUSINESSID && a.YEARANDMONTH == license.YEARANDMONTH)
                    .OrderByDescending(t => t.stpymd).ThenByDescending(x => x.payamt).First();

                //payment.SetLicenseAmount(license.TOTAL.Value);
                payment.SetLicenseAmount(license.AMOUNT.Value);
            }
        }

        private static bool Check(REQ_PAYMENT record, PaymentFilter filter)
        {
            var approvedResult = true;
            if (filter.ApprovedType == 1)
                approvedResult = record != null && record.APPROVEDDATE == null;
            else if (filter.ApprovedType == 2)
                approvedResult = record != null && record.APPROVEDDATE != null;
            else if (filter.ApprovedType == 3)
                approvedResult = record != null && filter.ApprovedBeginDate <= record.APPROVEDDATE && record.APPROVEDDATE <= filter.ApprovedEndDate;

            var attachedResult = true;
            if (filter.AttachType == 1)
                attachedResult = record != null && record.ATTACHDATE == null;
            else if (filter.AttachType == 2)
                attachedResult = record != null && record.ATTACHDATE != null;
            else if (filter.AttachType == 3)
                attachedResult = record != null && filter.AttachBeginDate <= record.ATTACHDATE && record.ATTACHDATE <= filter.AttachEndDate;

            return approvedResult && attachedResult;
        }

        private static Func<OracleDbContext, List<PaymentHeader>, string, PaymentFilter, List<PaymentDto>> GetPayments = (
            _dbContext, headers, contractNo, filter) =>
        {
            if (contractNo != "%")
                headers = headers.Where(a => a.ctrcd == contractNo).ToList();

            //var tree = Utils.Expression.ExpressionRetriever.Contruct<REQ_PAYMENT>(filters);
            //var func = tree.Compile();

            return (from header in headers
                    join request in _dbContext.REQ_PAYMENT.ToList() on new { stopdate = header.stpymd, contractno = header.ctrcd }
                                         equals new { stopdate = request.STPYMD, contractno = request.CONTRACTNO } into t
                    from rt in t.DefaultIfEmpty()

                    join disabled in _dbContext.REQ_PAYMENT_DISABLED.ToList() on rt?.ID equals disabled?.PAYMENTID into dilj
                    from dlj in dilj.DefaultIfEmpty()

                    join users in _dbContext.SYSTEM_USERS.ToList() on rt?.APPROVEDUSER equals users?.ID into su
                    from appuser in su.DefaultIfEmpty()

                    join contractslist in _dbContext.MST_CONTRACT.ToList() on header.ctrcd equals contractslist.CONTRACTNO into con
                    from contr in con.DefaultIfEmpty()
                    join company in _dbContext.SYSTEM_ORGANIZATION.ToList() on contr?.BUSINESSID equals company.ID into companycontract
                    from comcontract in companycontract.DefaultIfEmpty()

                    where Check(rt, filter) // func (request)

                    //join licenselist in _dbContext.SYSTEM_BIZ_LICENSE.ToList()
                    //on new { businessid = comcontract.ID, begindate = header.stpymd, contract = contr.CONTRACTNO }
                    //equals new { businessid = licenselist.BUSINESSID, begindate = GetEndDate(licenselist.YEAR, licenselist.MONTH), contract = licenselist.PAYCTRCD } into l
                    //from license in l.DefaultIfEmpty()

                    select new PaymentDto()
                    {
                        ID = rt?.ID,
                        comid = comcontract?.ID,
                        regno = comcontract?.REGNO,

                        APPROVEDDATE = rt?.APPROVEDDATE,
                        APPROVEDUSER = appuser?.FIRSTNAME,
                        ATTACHDATE = rt?.ATTACHDATE,
                        ATTACHFILE = rt?.ATTACHFILE,

                        TYPE = rt != null && rt.APPROVEDDATE.HasValue && !rt.ATTACHDATE.HasValue ? (dlj != null ? 1 : 2) : 0,
                        REASON = rt != null && rt.APPROVEDDATE.HasValue && !rt.ATTACHDATE.HasValue ? (dlj != null ? $"Цуцласан. {dlj.DESCRIPTION}" : "Хавсаргалт амжилтгүй болсон.") : string.Empty,

                        amt = header.amt,
                        ctrcd = header.ctrcd,
                        ctrnm = header.ctrnm,
                        edi = header.edi,
                        evnamt = header.evnamt,
                        frbtamt = header.frbtamt,
                        ifrbtamt = header.ifrbtamt,
                        normalstk = header.normalstk,
                        //payamt = license == null ? header.payamt : header.payamt - license?.AMOUNT,
                        payamt = header.payamt,
                        PBGB = header.PBGB,
                        stpymd = header.stpymd,
                        strymd = header.strymd,
                        YEARANDMONTH = UsefulHelpers.SubString(header.stpymd, 0, 6),
                        banknm = header.banknm,
                        accno = header.accno,
                        penamt = header.penamt,
                        crdfee = header.crdfee,
                        invamt = contr == null ? header.invamt : (contr.CONTRACTTYPE == 2 || contr.CONTRACTTYPE == 5) ? 0 : header.invamt, // 2 5 type iin shiveh dun 0 bh zaswar orow 22.10.27
                        paycycle = header.paycycle,
                        CONTRACTNO = rt?.CONTRACTNO,
                        DESCRIPTION = rt?.DESCRIPTION,
                        STORESEEN = rt?.STORESEEN,
                        STOREPRINT = rt?.STOREPRINT,
                        LICENSEAMOUNT = 0
                        //LICENSEAMOUNT = license?.AMOUNT
                    }).OrderBy(x => x.ctrcd).ThenBy(x => x.stpymd).ToList();
        };

    }

    public class PaymentFilter
    {
        public int AttachType { get; set; }
        public DateTime? AttachBeginDate { get; set; }
        public DateTime? AttachEndDate { get; set; }
        public int ApprovedType { get; set; }
        public DateTime? ApprovedBeginDate { get; set; }
        public DateTime? ApprovedEndDate { get; set; }

    }
}
