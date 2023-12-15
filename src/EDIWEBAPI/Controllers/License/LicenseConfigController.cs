using EDIWEBAPI.Context;
using EDIWEBAPI.Controllers.SysManagement;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Entities.LicenseConfig;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.License
{
    [Route("api/licenseconfig")]
    public class LicenseConfigController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<LicenseConfigController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public LicenseConfigController(OracleDbContext context, ILogger<LicenseConfigController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        [HttpGet]
        [Authorize]
        [Route("getcycleconfigs/{storeid}")]
        public ResponseClient GetCycleConfigs(int storeid)
        {
            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            if ((ORGTYPE)orgType == ORGTYPE.Бизнес)
                return ReturnResponce.AccessDeniedResponce();

            var response = Logics.ManagementLogic.GetCycleConfigs(_dbContext, storeid);
            return response.Any() ? 
                ReturnResponce.ListReturnResponce(response.ToList()) : 
                ReturnResponce.NotFoundResponce();
        }


        [HttpDelete]
        [Authorize]
        [Route("deletecycleconfig/{id}")]
        public ResponseClient DeleteCycleConfig(int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            if ((ORGTYPE)orgType == ORGTYPE.Бизнес)
                return ReturnResponce.AccessDeniedResponce();
            
            var currentdata = Logics.ManagementLogic.GetCycleConfig(_dbContext, id);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            Logics.ManagementLogic.Delete(_dbContext, currentdata);
            return ReturnResponce.SuccessMessageResponce("Амжилттай устгалаа!");
        }


        [HttpPost]
        [Authorize]
        [Route("postcycleconfig")]
        public ResponseClient PostCycleConfig([FromBody] SYSTEM_STORECYCLE_CONFIG param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            if ((ORGTYPE)orgType == ORGTYPE.Бизнес)
                return ReturnResponce.AccessDeniedResponce();

            param.ID = Convert.ToInt32(Logics.ManagementLogic.GetNewId(_dbContext, "SYSTEM_STORECYCLE_CONFIG"));

            Logics.ManagementLogic.Insert(_dbContext, param);
            return ReturnResponce.SaveSucessResponce();
        }

        [HttpPut]
        [Authorize]
        [Route("putcycleconfig")]
        public ResponseClient PutCycleConfig([FromBody]SYSTEM_STORECYCLE_CONFIG param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            if ((ORGTYPE)orgType == ORGTYPE.Бизнес)
                return ReturnResponce.AccessDeniedResponce();

            var currentdata = Logics.ManagementLogic.GetCycleConfig(_dbContext, param.ID);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            currentdata.CYCLEINDEX = param.CYCLEINDEX;
            currentdata.DAYNAMES = param.DAYNAMES;
            currentdata.DAYTYPE = param.DAYTYPE;
            currentdata.DURATION = param.DURATION;
            currentdata.STARTDAY = param.STARTDAY;

            Logics.ManagementLogic.Update(_dbContext, currentdata);
            return ReturnResponce.SaveSucessResponce();
        }

        #region ContractConifg


        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("getallconfigcont")]
        public ResponseClient GetALLConfigCont()
        {
            var returndata = _dbContext.SYSTEM_LIC_CONT_CONFIG.ToList();
            return returndata.Count > 0 ?
                ReturnResponce.ListReturnResponce(returndata) :
                ReturnResponce.NotFoundResponce();
        }

        [HttpPost]
        [Authorize(Policy = "EdiApiUser")]
        [Route("postcontractconfig")]
        public ResponseClient PostContractConfig([FromBody] SYSTEM_LIC_CONT_CONFIG param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            param.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_LIC_CONT_CONFIG"));

            Logics.ManagementLogic.Insert(_dbContext, param);
            return ReturnResponce.SaveSucessResponce();
        }



        [HttpPut]
        [Authorize(Policy = "EdiApiUser")]
        [Route("putcontractconfig")]
        public ResponseClient PutContractConfig([FromBody]SYSTEM_LIC_CONT_CONFIG param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var currentdata = _dbContext.SYSTEM_LIC_CONT_CONFIG.FirstOrDefault(x => x.ID == param.ID);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            currentdata.ID = param.ID;
            currentdata.MINVALUE = param.MINVALUE;
            currentdata.MAXVALUE = param.MAXVALUE;
            currentdata.SCORE = param.SCORE;
            currentdata.RANGENAME = param.RANGENAME;

            Logics.ManagementLogic.Update(_dbContext, currentdata);
            return ReturnResponce.SaveSucessResponce();
        }



        [HttpDelete]
        [Authorize(Policy = "EdiApiUser")]
        [Route("deletecontractconfig/{configid}")]
        public ResponseClient DeleteContractConfig(int configid)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var currentdata = _dbContext.SYSTEM_LIC_CONT_CONFIG.FirstOrDefault(x => x.ID == configid);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            Logics.ManagementLogic.Delete(_dbContext, currentdata);
            return ReturnResponce.SuccessMessageResponce("Амжилттай устгалаа!");
        }


        #endregion

        #region SkuConfig
        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("getallconfigsku")]
        public ResponseClient GetALLConfigSku()
        {
            var returndata = _dbContext.SYSTEM_LIC_SKU_CONFIG.ToList();
            return returndata.Count > 0 ?
                ReturnResponce.ListReturnResponce(returndata) :
                ReturnResponce.NotFoundResponce();
        }



        [HttpPost]
        [Route("postskuconfig")]
        [Authorize(Policy = "EdiApiUser")]
        public ResponseClient PostSkuConfig([FromBody]SYSTEM_LIC_SKU_CONFIG param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            param.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_LIC_SKU_CONFIG"));

            Logics.ManagementLogic.Insert(_dbContext, param);
            return ReturnResponce.SaveSucessResponce();
        }

        [HttpPut]
        [Route("putskuconfig")]
        [Authorize(Policy = "EdiApiUser")]
        public ResponseClient PutSkuConfig([FromBody]SYSTEM_LIC_SKU_CONFIG param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            
            var currentdata = _dbContext.SYSTEM_LIC_SKU_CONFIG.FirstOrDefault(x => x.ID == param.ID);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            currentdata.ID = param.ID;
            currentdata.MINVALUE = param.MINVALUE;
            currentdata.MAXVALUE = param.MAXVALUE;
            currentdata.SCORE = param.SCORE;
            currentdata.RANGENAME = param.RANGENAME;

            Logics.ManagementLogic.Update(_dbContext, currentdata);
            return ReturnResponce.SaveSucessResponce();
        }


        [HttpDelete]
        [Authorize(Policy = "EdiApiUser")]
        [Route("deleteskuconfig/{configid}")]
        public ResponseClient DeleteSkuConfig(int configid)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            
            var currentdata = _dbContext.SYSTEM_LIC_SKU_CONFIG.FirstOrDefault(x => x.ID == configid);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            Logics.ManagementLogic.Delete(_dbContext, currentdata);
            return ReturnResponce.SuccessMessageResponce("Амжилттай устгалаа!");
        }

        #endregion 

        #region SaleConfig
        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("getallconfigsale")]
        public ResponseClient GetALLConfigSale()
        {
            var returndata = _dbContext.SYSTEM_LIC_SALE_CONFIG.ToList();
            return returndata.Count > 0 ?
                ReturnResponce.ListReturnResponce(returndata) :
                ReturnResponce.NotFoundResponce();
        }

        [HttpPost]
        [Route("postsaleconfig")]
        [Authorize(Policy = "EdiApiUser")]
        public ResponseClient PostsaleConfig([FromBody]SYSTEM_LIC_SALE_CONFIG param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            param.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_LIC_SALE_CONFIG"));

            Logics.ManagementLogic.Insert(_dbContext, param);
            return ReturnResponce.SaveSucessResponce();
        }

        [HttpPut]
        [Route("putsaleconfig")]
        [Authorize(Policy = "EdiApiUser")]
        public ResponseClient PutsaleConfig([FromBody]SYSTEM_LIC_SALE_CONFIG param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var currentdata = _dbContext.SYSTEM_LIC_SALE_CONFIG.FirstOrDefault(x => x.ID == param.ID);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            currentdata.ID = param.ID;
            currentdata.MINVALUE = param.MINVALUE;
            currentdata.MAXVALUE = param.MAXVALUE;
            currentdata.SCORE = param.SCORE;
            currentdata.RANGENAME = param.RANGENAME;

            Logics.ManagementLogic.Update(_dbContext, currentdata);
            return ReturnResponce.SaveSucessResponce();
        }



        [HttpDelete]
        [Authorize(Policy = "EdiApiUser")]
        [Route("deletesaleconfig/{configid}")]
        public ResponseClient DeletesaleConfig(int configid)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var currentdata = _dbContext.SYSTEM_LIC_SALE_CONFIG.FirstOrDefault(x => x.ID == configid);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            Logics.ManagementLogic.Delete(_dbContext, currentdata);
            return ReturnResponce.SuccessMessageResponce("Амжилттай устгалаа!");
        }



        #endregion

        #region roleconfig
        [HttpGet]
        [Authorize(Policy = "EdiApiUser")]
        [Route("getallconfigrole")]
        public ResponseClient GetALLConfigRole()
        {
            var returndata = _dbContext.SYSTEM_ROLE_CONFIG.ToList();
            return returndata.Count > 0 ?
                ReturnResponce.ListReturnResponce(returndata) :
                ReturnResponce.NotFoundResponce();
        }

        [HttpPost]
        [Route("postroleconfig")]
        [Authorize(Policy = "EdiApiUser")]
        public ResponseClient PostroleConfig([FromBody]SYSTEM_ROLE_CONFIG param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            param.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ROLE_CONFIG"));

            Logics.ManagementLogic.Insert(_dbContext, param);
            return ReturnResponce.SaveSucessResponce();
        }

        [HttpPut]
        [Route("putroleconfig")]
        [Authorize(Policy = "EdiApiUser")]
        public ResponseClient PutroleConfig([FromBody]SYSTEM_ROLE_CONFIG param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var currentdata = _dbContext.SYSTEM_ROLE_CONFIG.FirstOrDefault(x => x.ID == param.ID);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            currentdata.ID = param.ID;
            currentdata.MINSCORE = param.MINSCORE;
            currentdata.MAXSCORE = param.MAXSCORE;
            currentdata.RANGENAME = param.RANGENAME;
            currentdata.PRICE = param.PRICE;

            Logics.ManagementLogic.Update(_dbContext, currentdata);
            return ReturnResponce.SaveSucessResponce();
        }



        [HttpDelete]
        [Authorize(Policy = "EdiApiUser")]
        [Route("deleteroleconfig/{configid}")]
        public ResponseClient DeleteroleConfig(int configid)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var currentdata = _dbContext.SYSTEM_ROLE_CONFIG.FirstOrDefault(x => x.ID == configid);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            Logics.ManagementLogic.Delete(_dbContext, currentdata);
            return ReturnResponce.SuccessMessageResponce("Амжилттай устгалаа!");
        }

        #endregion
        /// <summary>
        /// Дэлгүүрийн лицензийн тооцоолол бодуулах
        /// </summary>
        /// <param name="storeid">Дэлгүүрийн ID</param>
        /// <returns></returns>
        #region LicenseVariables
        [HttpGet]
        [AllowAnonymous]
        [Route("executelicensedata/{storeid}")]
        public ResponseClient ExecucteLicenseData(int storeid)
        {
            return Logics.LicenseLogic.Calculate(_dbContext, _log, storeid, DateTime.Today.AddMonths(-1));
            //return Calculate(storeid, DateTime.Today.AddMonths(-1), false);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("executelicensestore/{storeid}/{year}/{month}")]
        public ResponseClient ExecuteLicenseStore(int storeid, int year, int month)
        {
            var sdate = new DateTime(year, month, 1);
            //return Calculate(storeid, sdate, test);
            return Logics.LicenseLogic.Calculate(_dbContext, _log, storeid, sdate);
        }

        //[HttpGet]
        //[AllowAnonymous]
        //[Route("executelicensestore/{storeid}/{regno}/{year}/{month}/{test}")]
        //public ResponseClient ExecuteLicenseStore(int storeid, string regno, int year, int month, bool test)
        //{
        //    var sdate = new DateTime(year, month, 1);
        //    return Calculate(storeid, regno, sdate, test);
        //}

        private ResponseClient Calculate(int storeid, DateTime currentDate, bool test)
        {
            var logMethod = "LicenseConfigController.Calculate";
            try
            {
                Logics.LicenseLogic.DeleteExisting(_dbContext, _log, storeid, currentDate);

                var sdate = new DateTime(currentDate.Year, currentDate.Month, 1);
                var edate = sdate.AddMonths(1).AddSeconds(-1);
                
                _log.LogInformation($"{logMethod} BEGIN : {storeid} ({sdate} => {edate})");
                var users = test ? 
                    Logics.LicenseLogic.GetTestLicenseUser(_dbContext) :
                    Logics.LicenseLogic.GetLicenseUsers(_dbContext, _log, storeid, sdate, edate);
                
                if (users == null || !users.Any())
                    return ReturnResponce.NotFoundResponce();

                return Calculate(storeid, sdate, edate, test, users.ToList());
            }
            catch (Exception ex)
            {
                _log.LogError($"{logMethod} ERROR : {ex.Message}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        private ResponseClient Calculate(int storeid, DateTime sdate, DateTime edate, bool test, List<LicenseUser> companyusers)
        {
            var logMethod = "LicenseConfigController.Calculate";

            var companies = test ?
                Logics.LicenseLogic.GetTestLicenseCompanies(_dbContext, _log, companyusers) :
                Logics.LicenseLogic.GetLicenseCompanies(_dbContext, _log, storeid, sdate, edate, companyusers);

            if (companies == null || !companies.Any())
                return ReturnResponce.NotFoundResponce();

            _log.LogInformation($"{logMethod} USERS : {companyusers.Count()} COMPANIES : {companies.Count()}");

            var contConfigs = _dbContext.SYSTEM_LIC_CONT_CONFIG.ToList();
            var saleConfigs = _dbContext.SYSTEM_LIC_SALE_CONFIG.ToList();
            var skuConfigs = _dbContext.SYSTEM_LIC_SKU_CONFIG.ToList();
            var configs = _dbContext.SYSTEM_ROLE_CONFIG.ToList();

            var pricePercent = ConfigData.GetConfigData(ConfigData.ConfigKey.MaxPricePercent, "4");
            var minAmount = ConfigData.GetConfigData(ConfigData.ConfigKey.MinUserAmount, "30000");

            _log.LogInformation($"{logMethod} CONFIG => {pricePercent} : {minAmount}");

            return Logics.LicenseLogic.Calculate(_dbContext, _log, storeid,
                int.Parse(pricePercent), decimal.Parse(minAmount),
                !test, sdate, edate,
                companies.ToList(), companyusers, contConfigs, saleConfigs, skuConfigs, configs);
        }


        private ResponseClient Calculate(int storeid, string regno, DateTime currentDate, bool test)
        {
            var logMethod = "LicenseConfigController.Calculate";
            try
            {
                var organization = Logics.ManagementLogic.GetOrganization(_dbContext, regno);
                if (organization == null)
                    return ReturnResponce.NotFoundResponce();

                Logics.LicenseLogic.DeleteExisting(_dbContext, _log, storeid, organization.ID, currentDate);

                var sdate = new DateTime(currentDate.Year, currentDate.Month, 1);
                var edate = sdate.AddMonths(1).AddSeconds(-1);

                _log.LogInformation($"{logMethod} BEGIN : {storeid} {organization.ID} ({sdate} => {edate})");
                var users = test ?
                    Logics.LicenseLogic.GetTestLicenseUser(_dbContext, organization.ID) :
                    Logics.LicenseLogic.GetLicenseUsers(_dbContext, _log, storeid, organization.ID, sdate, edate);

                if (users == null || !users.Any())
                    return ReturnResponce.NotFoundResponce();

                return Calculate(storeid, sdate, edate, test, users.ToList());
            }
            catch (Exception ex)
            {
                _log.LogError($"{logMethod} ERROR : {ex.Message}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("licensetest/{year}/{month}/{comid}")]
        [AllowAnonymous]
        public ResponseClient TestLicenseAttribute(int year, int month, int comid)
        {
            //var sdate = new DateTime(year, month, 1);
            //var edate = sdate.AddMonths(1).AddSeconds(-1);

            //var contConfigs = _dbContext.SYSTEM_LIC_CONT_CONFIG.ToList();
            //var saleConfigs = _dbContext.SYSTEM_LIC_SALE_CONFIG.ToList();
            //var skuConfigs = _dbContext.SYSTEM_LIC_SKU_CONFIG.ToList();
            //var configs = _dbContext.SYSTEM_ROLE_CONFIG.ToList();

            //var result = LicenseAttributes(comid, 2, sdate, edate,
            //    contConfigs, saleConfigs, skuConfigs, configs,
            //    2, 100, 1500000);

            //_dbContext.SaveChanges();

            return ReturnResponce.SaveSucessResponce();
        }


        #endregion
        
        #region licenseReport

        [HttpGet]
        [AllowAnonymous]
        [Route("reportlicensedata/{storeid}/{licdate}")]
        public ResponseClient ReportLicenseData(int storeid, DateTime licdate)
        {
            var listHeader = _dbContext.SYSTEM_REPORT_LICENSE_HEADER(licdate.Year, licdate.ToString("MM"), 0, storeid).ToList();
            var listDetail = _dbContext.SYSTEM_REPORT_LICENSE_DETAIL(licdate.Year, licdate.ToString("MM"), 0, storeid).ToList();
            var lData = new List<LicenseReportData>();
            foreach (SYSTEM_REPORT_LICENSE_HEADER header in listHeader)
            {
                var ldataone = new LicenseReportData()
                {
                    Header = header,
                    Detail = new List<SYSTEM_REPORT_LICENSE_DETAIL>()
                };
                var detaildata = listDetail.Where(x => x.YEAR == header.YEAR && x.MONTH == header.MONTH && x.STOREID == header.STOREID && x.BIZID == header.BUSINESSID);
                if (detaildata != null)
                {
                    ldataone.Detail.AddRange(detaildata);
                }
                lData.Add(ldataone);
            }

            return ReturnResponce.ListReturnResponce(lData);

        }

        #endregion

        #region LicensenMenuRole



        /// <summary>
        /// Тухайн role дээрх хандах цэсийн жагсаалт roleId-р шүүж авчрах API    
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : Давхарбаяр
        /// Үүсгэсэн огноо : 2018.05-25 
        /// <param name = "type" > STORE - 2, BUSINESS - 1, SYSTEM - 3 </param>
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">STORE - 2, BUSINESS - 1, SYSTEM - 3</response>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Route("menulistbyroletype/{type}")]
        [Authorize]
        public async Task<ResponseClient> GetMenusByOrganizationType(int type)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var menus = _dbContext.GET_MENUS_BY_ORGANIZATION_TYPE(type);
            if (menus == null)
                return ReturnResponce.NotFoundResponce();

            return ReturnResponce.ListReturnResponce(menus);
        }



        /// <summary>
        ///	#Тухайн модульд хамаарах роль буюу багцуудыг буцаана
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-05-25
        /// </remarks>
        /// <param name="moduletype">Модуль төрөл</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Route("rolelistbymodule/{moduletype}")]
        [Authorize]
        public async Task<ResponseClient> GetRoleListByModuleType(int moduletype)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var roles = _dbContext.SYSTEM_ROLES.Where(x => x.MODULETYPE == moduletype);
            if (roles == null)
                return ReturnResponce.NotFoundResponce();

            return ReturnResponce.ListReturnResponce(roles);
        }



        /// <summary>
        /// Тухайн role дээрх хандах цэсийн жагсаалт roleId-р шүүж авчрах API 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-05-25 
        /// Зассан огноо : 
        /// </remarks>
        /// <param name = "roleid" > Roleid </param >
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Route("rolebymenulist/{roleid}")]
        [Authorize]
        public async Task<ResponseClient> GetMenusByRoleId(int roleid)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var menus = _dbContext.GET_MENUS_BY_ROLE(roleid);
            if (menus == null)
                return ReturnResponce.NotFoundResponce();

            return ReturnResponce.ListReturnResponce(menus);
        }


        /// <summary>
        ///	#Тухайн ROLE-д хамаарах үнийн тохиргоог татаж байгаа функц
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-05-25
        /// </remarks>
        /// <param name="roleid"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Route("roleconfigbyroleid/{roleid}")]
        [Authorize]
        public async Task<ResponseClient> GetRoleConfigByRoleID(int roleid)
        {
            return ReturnResponce.ListReturnResponce( _dbContext.SYSTEM_ROLE_CONFIG.Where(x => x.ROLEID == roleid));
        }
        #endregion

        #region Role
        #region Post

        [HttpPost]
        [Route("newrole")]
        [Authorize]
        public ResponseClient NewRole([FromBody]SYSTEM_ROLES param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var vdata = new SYSTEM_ROLES();
            vdata.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ROLES"));

            vdata.ROLENAME = vdata.ROLENAME;
            vdata.MODULETYPE = param.MODULETYPE;
            _dbContext.SYSTEM_ROLES.Add(vdata);
            _dbContext.SaveChanges();

            return ReturnResponce.SaveSucessResponce();
        }

        #endregion

        #region Put
        /// <summary>
        ///	#SYSTEM_MENU_ROLE ХҮСНЭГТИЙН put
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-05-30
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpPut]
        [Authorize]
        [Route("updaterole")]
        public ResponseClient UpdateRole([FromBody]SYSTEM_ROLES param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var currentdata = _dbContext.SYSTEM_ROLES.FirstOrDefault(x => x.ID == param.ID);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            currentdata.ID = param.ID;
            currentdata.ROLENAME = param.ROLENAME;

            _dbContext.Entry(currentdata).State = System.Data.Entity.EntityState.Modified;
            _dbContext.SaveChanges();

            return ReturnResponce.SaveSucessResponce();
        }


        #endregion


        [HttpGet]
        [Route("rolebyid/{id}")]
        [Authorize]
        public ResponseClient RoleByID(int id)
        {
            var currentroles = _dbContext.SYSTEM_ROLES.FirstOrDefault(x => x.ID == id);
            return currentroles != null ? 
                ReturnResponce.ListReturnResponce(currentroles) :
                ReturnResponce.NotFoundResponce();
        }

        #endregion
    }
}
