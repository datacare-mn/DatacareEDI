using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.FilterViews;
using EDIWEBAPI.Enums;
using EDIWEBAPI.Entities.ResultModels;
using EDIWEBAPI.Controllers.Storeapi;
using EDIWEBAPI.Attributes;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/[controller]")]
    public class CompanyController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<CompanyController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public CompanyController(OracleDbContext context, ILogger<CompanyController> log)
        {
            _dbContext = context;
            _log = log;
        }

        #endregion

        #region Get
        /// <summary>
        /// Компанийн мэдээлэл
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Get(int id)
        {
            try
            {
                var currentcompany = Logics.ManagementLogic.GetOrganization(_dbContext, id);
                if (currentcompany == null)
                    return ReturnResponce.NotFoundResponce();

                var parentCompany = currentcompany.PARENTID.HasValue ?
                    Logics.ManagementLogic.GetOrganization(_dbContext, currentcompany.PARENTID.Value) :
                    null;

                var result = new
                {
                    id = currentcompany.ID,
                    parentid = currentcompany.PARENTID,
                    orgName = currentcompany.COMPANYNAME,
                    orgRegistrationNumber = currentcompany.REGNO,
                    orgType = Convert.ToInt32(currentcompany.ORGTYPE.Value),
                    director = currentcompany.CEONAME,
                    phone = currentcompany.MOBILE,
                    web = currentcompany.WEBSITE,
                    facebook = currentcompany.FBADDRESS,
                    website = currentcompany.WEBSITE,
                    email = currentcompany.EMAIL,
                    address = currentcompany.ADDRESS,
                    longtidue = currentcompany.LONGITUDE,
                    isforeign = currentcompany.ISFOREIGN,
                    lattitude = currentcompany.LATITUDE,
                    logo = currentcompany.LOGO,
                    parentOrgRegistrationNumber = parentCompany != null ? parentCompany.REGNO : string.Empty,
                    parentOrgName = parentCompany != null ? parentCompany.COMPANYNAME : string.Empty
                };
                
                return ReturnResponce.ListReturnResponce(result);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }




        /// <summary>
        ///	#Компанийн мэдээллийг регистрээр шүүх
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-12-21
        /// </remarks>
        /// <param name="regno">Рег №</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpGet]
        [Authorize]
        [Route("companyinforegno/{regno}")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetCompanyInfo(string regno)
        {
            var currentcompany = Logics.ManagementLogic.GetOrganization(_dbContext, regno);
            if (currentcompany == null)
                return ReturnResponce.NotFoundResponce();
            
            var result = new
            {
                id = currentcompany.ID,
                parentid = currentcompany.PARENTID,
                orgName = currentcompany.COMPANYNAME,
                orgRegistrationNumber = currentcompany.REGNO,
                orgType = Convert.ToInt32(currentcompany.ORGTYPE.Value),
                director = currentcompany.CEONAME,
                phone = currentcompany.MOBILE,
                web = currentcompany.WEBSITE,
                facebook = currentcompany.FBADDRESS,
                website = currentcompany.WEBSITE,
                email = currentcompany.EMAIL,
                address = currentcompany.ADDRESS,
                longtidue = currentcompany.LONGITUDE,
                lattitude = currentcompany.LATITUDE
            };
            return ReturnResponce.ListReturnResponce(result);
        }

        /// <summary>
        /// Компанийн жагсаалт идэвхитэй
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpGet]
        [Authorize]
        [Route("getheadcompanies")]
        public ResponseClient GetHeadCompanyList()
        {
            try
            {
                var headCompanies = _dbContext.SYSTEM_ORGANIZATION
                    .Where(x => x.ENABLED == 1 && x.PARENTID == null)
                    .Select(a => new AbbCompany()
                    {
                        ID = a.ID,
                        COMPANYNAME = a.COMPANYNAME,
                        REGNO = a.REGNO
                    }).ToList();

                return headCompanies == null ?
                    ReturnResponce.NotFoundResponce() :
                    ReturnResponce.ListReturnResponce(headCompanies);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }




        [HttpGet]
        [Route("GetAll")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetAll()
        {
            try
            {
                var companies = _dbContext.SYSTEM_ORGANIZATION
                    .Where(x => x.ENABLED == 1 && (x.PARENTID == null || x.PARENTID == 0))
                    .Select(a => new TreeCompany()
                    {
                        ADDRESS = a.ADDRESS,
                        CEONAME = a.CEONAME,
                        COMPANYNAME = a.COMPANYNAME,
                        EMAIL = a.EMAIL,
                        ENABLED = a.ENABLED,
                        FBADDRESS = a.FBADDRESS,
                        ID = a.ID,
                        LATITUDE = a.LATITUDE,
                        LOGO = a.LOGO,
                        LONGITUDE = a.LONGITUDE,
                        MOBILE = a.MOBILE,
                        ORGTYPE = a.ORGTYPE,
                        PARENTID = a.PARENTID,
                        PARENTREGNO = a.PARENTREGNO,
                        REGNO = a.REGNO,
                        SLOGAN = a.SLOGAN,
                        WEBSITE = a.WEBSITE
                    }).ToList();

                if (companies != null && companies.Any())
                    companies.ForEach(a => a.CHIDLCOMPANYS = _dbContext.SYSTEM_ORGANIZATION.Where(c => c.PARENTID == a.ID && c.ENABLED == 1).ToList());

                return companies != null ?
                    ReturnResponce.ListReturnResponce(companies) :
                    ReturnResponce.NotFoundResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        /// <summary>
        /// Байгууллагын жагсаалт шүүлтүүртэй хуудаслалтай 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : Л.Идэр
        /// Үүсгэсэн огноо : 2017.12.18 
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpPost]
        [Route("CompanyList")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetCompanyListWithFilter([FromBody]OrganizationFilterView filter)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var orgtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            if (ORGTYPE.Бизнес == (ORGTYPE)orgtype)
                return ReturnResponce.AccessDeniedResponce();

            if (ORGTYPE.Дэлгүүр == (ORGTYPE)orgtype)
                filter.orgnizationType = (int)ORGTYPE.Бизнес;

            var companyList = _dbContext.GET_COMPANIES(filter);
            if (companyList == null)
                return ReturnResponce.NotFoundResponce();

            return ReturnResponce.ListReturnResponce(companyList);
        }

        /// <summary>
        /// Идэвхигүй компанийн жагсаалт 
        /// </summary>
        /// <remarks>
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpGet]
        [Route("getalldisabledcompanylist")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetAllDisabledCompanyList()
        {
            var companys = _dbContext.SYSTEM_ORGANIZATION.Where(x => x.ENABLED == 0).ToList();
            return companys != null ?
                ReturnResponce.ListReturnResponce(companys) :
                ReturnResponce.NotFoundResponce();
        }
        #endregion

        #region Post

        /// <summary>
        /// Компанийн мэдээлэл хадгалах 
        /// </summary>
        /// <remarks>
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <param name="param">компанийн мэдээлэл лист жсон </param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Post([FromBody]SYSTEM_ORGANIZATION param)
        {
            if (param == null)
                return ReturnResponce.NotFoundResponce();

            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            try
            {
                var existing = Logics.ManagementLogic.GetOrganization(_dbContext, param.REGNO);
                if (existing != null)
                    return ReturnResponce.FailedMessageResponce($"{param.REGNO} регистерийн дугаартай байгууллага бүртгэгдсэн байна...");

                param.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ORGANIZATION"));
                var organization = new SYSTEM_ORGANIZATION()
                {
                    ID = param.ID,
                    COMPANYNAME = param.COMPANYNAME,
                    REGNO = param.REGNO,
                    ADDRESS = param.ADDRESS,
                    CEONAME = param.CEONAME,
                    WEBSITE = param.WEBSITE,
                    EMAIL = param.EMAIL,
                    FBADDRESS = param.FBADDRESS,
                    LONGITUDE = param.LONGITUDE,
                    LATITUDE = param.LATITUDE,
                    MOBILE = param.MOBILE,
                    SLOGAN = param.SLOGAN,
                    LOGO = param.LOGO,
                    ORGTYPE = param.ORGTYPE,
                    ISFOREIGN = param.ISFOREIGN,
                    ENABLED = 1,
                    PARENTID = param.PARENTID
                };
                Logics.ManagementLogic.Insert(_dbContext, organization);
                Logics.ContractLogic.Modify(_dbContext, _log, UsefulHelpers.STORE_ID, organization.REGNO);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        #endregion

        #region Put

        /// <summary>
        /// Компанийн мэдээлэл засах
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <param name="param">company json</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Put([FromBody]SYSTEM_ORGANIZATION param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            try
            {
                var currentdata = Logics.ManagementLogic.GetOrganization(_dbContext, param.ID);

                if (currentdata == null)
                    return ReturnResponce.NotFoundResponce();

                if (currentdata.REGNO != param.REGNO && Logics.ManagementLogic.GetOrganization(_dbContext, param.REGNO) != null)
                    return ReturnResponce.FailedMessageResponce($"{param.REGNO} регистерийн дугаартай байгууллага бүртгэгдсэн байна...");

                currentdata.ID = param.ID;
                currentdata.COMPANYNAME = param.COMPANYNAME;
                currentdata.REGNO = param.REGNO;
                currentdata.ADDRESS = param.ADDRESS;
                currentdata.CEONAME = param.CEONAME;
                currentdata.WEBSITE = param.WEBSITE;
                currentdata.EMAIL = param.EMAIL;
                currentdata.FBADDRESS = param.FBADDRESS;
                currentdata.LONGITUDE = param.LONGITUDE;
                currentdata.LATITUDE = param.LATITUDE;
                currentdata.MOBILE = param.MOBILE;
                currentdata.SLOGAN = param.SLOGAN;
                currentdata.LOGO = param.LOGO;
                currentdata.ORGTYPE = param.ORGTYPE;
                currentdata.ISFOREIGN = param.ISFOREIGN;
                currentdata.ENABLED = param.ENABLED;
                currentdata.PARENTID = param.PARENTID;

                Logics.ManagementLogic.Update(_dbContext, currentdata);
                Logics.ContractLogic.Modify(_dbContext, _log, UsefulHelpers.STORE_ID, currentdata.REGNO);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        /// Компанийн мэдээллийг идэвхигүй болгох
        /// </summary>
        /// <remarks>
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <param name="id">компанийн ID</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpPost]
        [Route("disablecompany/{id}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient DisableCompany(int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var currentdata = Logics.ManagementLogic.GetOrganization(_dbContext, id);
                if (currentdata == null)
                    return ReturnResponce.NotFoundResponce();

                if (currentdata.ENABLED == 0)
                    return ReturnResponce.FailedMessageResponce("Идэвхгүй төлөвтэй байна.");

                currentdata.ENABLED = 0;
                Logics.BaseLogic.Update(_dbContext, currentdata, false);

                var users = _dbContext.SYSTEM_USERS.Where(a => a.ORGID == id && a.ENABLED == ENABLED.Идэвхитэй);
                foreach (var user in users)
                {
                    user.ENABLED = ENABLED.Идэвхигүй;
                    Logics.BaseLogic.Update(_dbContext, user, false);
                }

                _dbContext.SaveChanges();
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        /// Компанийн мэдээллийг сэргээх
        /// </summary>
        /// <remarks>
        /// Програмист : 
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <param name="id">компанийн ID</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        [HttpPost]
        [Route("enablecompany/{id}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient EnableCompany(int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            try
            {
                var currentdata = Logics.ManagementLogic.GetOrganization(_dbContext, id);
                if (currentdata == null)
                    return ReturnResponce.NotFoundResponce();

                if (currentdata.ENABLED == 1)
                    return ReturnResponce.FailedMessageResponce("Идэвхтэй төлөвтэй байна.");

                currentdata.ENABLED = 1;
                Logics.BaseLogic.Update(_dbContext, currentdata);

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        #endregion

        #region Branch


        /// <summary>
        ///	#Салбарын бүртгэл
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-03
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpPost]
        [Authorize]
        [Route("branch")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Post([FromBody]List<SYSTEM_BRANCH> param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            
            var uData = new List<SYSTEM_BRANCH>();
            foreach (SYSTEM_BRANCH vdata in param)
            {
                vdata.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_BRANCH"));
                uData.Add(new SYSTEM_BRANCH()
                {
                    ID = vdata.ID,
                    STOREID = vdata.STOREID,
                    BRANCHNAME = vdata.BRANCHNAME,
                    ADDRESS = vdata.ADDRESS,
                    LOGO = vdata.LOGO,
                    MOBILE = vdata.MOBILE,
                });
            }
            _dbContext.SYSTEM_BRANCH.AddRange(uData);
            _dbContext.SaveChanges();
            return ReturnResponce.SaveSucessResponce();
        }

        [HttpPut]
        [Authorize]
        [Route("branch")]
        public ResponseClient Put([FromBody]SYSTEM_BRANCH param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var currentdata = _dbContext.SYSTEM_BRANCH.FirstOrDefault(x => x.ID == param.ID);
            if (currentdata == null)
                return ReturnResponce.NotFoundResponce();

            currentdata.STOREID = param.STOREID;
            currentdata.BRANCHNAME = param.BRANCHNAME;
            currentdata.ADDRESS = param.ADDRESS;
            currentdata.LOGO = param.LOGO;
            currentdata.MOBILE = param.MOBILE;
            _dbContext.Entry(currentdata).State = System.Data.Entity.EntityState.Modified;
            _dbContext.SaveChanges();

            return ReturnResponce.SaveSucessResponce();
        }

        [HttpGet]
        [Authorize]
        [Route("branch")]
        public ResponseClient Get()
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var branchlist = _dbContext.SYSTEM_BRANCH.Where(x => x.STOREID == comid).ToList();
            return branchlist != null ?
                ReturnResponce.ListReturnResponce(branchlist) :
                ReturnResponce.NotFoundResponce();
        }

        [HttpGet]
        [Route("httpmodel/{comid}")]
        [ServiceFilter(typeof(LogFilter))]
        public HttpRequestModel GetHttpModelData(int comid)
        {
            return Logics.ManagementLogic.GetHttpModelData(_dbContext, comid);
        }

        [HttpPost]
        [Route("{comid}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public bool UpdateCompanyToken(int comid, TokenValue  token)
        {
            return Logics.ManagementLogic.UpdateAppToken(_dbContext, comid, token);
        }
        #endregion

        [Authorize]
        [Route("vendorlistall")]
        [HttpGet]
        public async Task<ResponseClient> GetOrganizationVendlist()
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            int orgtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            return Logics.ContractLogic.GetVendorList(_dbContext, _log, orgtype, comid, null);
        }



        #region VendorList

        /// <summary>
        ///	#Харилцагчдын жагсаалт хайлын хэсэгтэй
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-23
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpPost]
        [Route("vendorlist")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]

        public async Task<ResponseClient> GetStoreVendorList([FromBody] VendorFilterView filterview)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            int orgtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            return Logics.ContractLogic.GetVendorList(_dbContext, _log, orgtype, comid, filterview);
        }

        /// <summary>
        ///	#Харилцагчдын жагсаалт хайлын хэсэгтэй
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-23
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpPost]
        [Route("companyvendorlist/{comid}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetCompanyVendorList([FromBody] VendorFilterView filterview, int comid)
        {
            var currentcompany = Logics.ManagementLogic.GetOrganization(_dbContext, comid);
            return Logics.ContractLogic.GetVendorList(_dbContext, _log, (int)currentcompany.ORGTYPE, comid, filterview);
        }

        #endregion

        /// <summary>
        ///	#Байгууллагын харилцагчдыг гэрээний хамт буцаана
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-23
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("paymentvendor")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetPaymentVendor()
        {
            try
            {
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
                string regno = Convert.ToString(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyReg));

                Logics.ContractLogic.Modify(_dbContext, _log, UsefulHelpers.STORE_ID, regno);

                var contractedstores = Logics.ContractLogic.GetContracts(_dbContext, _log, comid)
                    .Select(m => m.STOREID).Distinct().ToArray();
                var vendors = new List<VendorList>();
                if (contractedstores.Length == 0)
                    return ReturnResponce.NotFoundResponce();

                foreach (int storeid in contractedstores)
                {
                    var store = Logics.ManagementLogic.GetOrganization(_dbContext, storeid);
                    if (store == null) continue;

                    var vendor = new VendorList()
                    {
                        organization = store,
                        contracts = Logics.ContractLogic.GetContracts(_dbContext, _log, store.ID, comid).Distinct().ToList()
                    };
                    vendors.Add(vendor);
                }
                //.Select(x=> new { x.contracts, ID =  x.organization.ID, COMPANYNAME =  x.organization.COMPANYNAME  })
                return ReturnResponce.ListReturnResponce(vendors);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpGet]
        [Authorize]
        [Route("paymentvendorstore/{regno}")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetPaymentVendorStore(string regno)
        {
            try
            {
                int storeid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
                var currentcompany = Logics.ManagementLogic.GetOrganization(_dbContext, regno);
                if (currentcompany == null)
                    return ReturnResponce.NotFoundResponce();

                var contractedstores = Logics.ContractLogic.GetContracts(_dbContext, _log, storeid, currentcompany.ID)
                    .Select(m => m.BUSINESSID).Distinct().ToArray();
                var vendors = new List<VendorList>();
                if (contractedstores.Length == 0)
                    return ReturnResponce.NotFoundResponce();

                foreach (int bizid in contractedstores)
                {
                    var bizcoms = Logics.ManagementLogic.GetOrganization(_dbContext, bizid);
                    if (bizcoms == null) continue;

                    var vendor = new VendorList()
                    {
                        organization = bizcoms,
                        contracts = _dbContext.MST_CONTRACT
                            .Where(x => x.BUSINESSID == bizid && x.STOREID == storeid)
                            .GroupBy(x => x.CONTRACTNO)
                            .Select(g => g.FirstOrDefault()).ToList()
                    };
                    vendors.Add(vendor);
                }
                return ReturnResponce.ListReturnResponce(vendors);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        ///	#Тайлбар
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-24
        /// </remarks>
        /// <param name="comid">Компанийн ID</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize]
        [Route("role/{comid}")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetCompanyRole(int comid)
        {
            try
            {
                var roles = _dbContext.SYSTEM_ORGANIZATION_ROLES.Where(x => x.ORGID == comid);
                return roles == null ?
                    ReturnResponce.NotFoundResponce() :
                    ReturnResponce.ListReturnResponce(roles);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        #region ContractUsers

        /// <summary>
        ///	#Хэрэглэгчийн гэрээний эрх  бүртгэх
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-24
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("usercontract")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Post([FromBody]List<MST_CONTRACT_USERS> param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                int userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.UserId));

                var usercontracts = _dbContext.MST_CONTRACT_USERS.Where(x => x.USERID == userid);
                if (usercontracts != null)
                {
                    _dbContext.Entry(usercontracts).State = System.Data.Entity.EntityState.Deleted;
                    _dbContext.SaveChanges();
                }

                var uData = new List<MST_CONTRACT_USERS>();
                foreach (MST_CONTRACT_USERS vdata in param)
                {
                    vdata.ID = Convert.ToInt32(_dbContext.GetTableID("MST_CONTRACT_USERS"));
                    uData.Add(new MST_CONTRACT_USERS()
                    {
                        ID = vdata.ID,
                        CONTRACTID = vdata.CONTRACTID,
                        USERID = vdata.USERID,
                        INSYMD = DateTime.Now,
                        INSEMP = userid
                    });
                }
                _dbContext.MST_CONTRACT_USERS.AddRange(uData);
                _dbContext.SaveChanges();
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        /// <summary>
        ///	#Гэрээний жагасаалт
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : Цогийнлоовон
        /// Үүсгэсэн огноо : 2023-01-03
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Route("contracts")]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient Contracts()
        {
            int companyId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
            var currentdata = Logics.ContractLogic.GetContracts(_dbContext, _log, companyId);

            return currentdata == null ?
                ReturnResponce.NotFoundResponce() :
                ReturnResponce.ListReturnResponce(currentdata);
        }


        [HttpGet]
        [Route("storecontracts/{regno}")]
        [Authorize(Policy = "StoreApiUser")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient StoreContracts(string regno)
        {
            if (regno != "null")
            {
                var currentcompany = Logics.ManagementLogic.GetOrganization(_dbContext, regno);
                if (currentcompany == null)
                    return ReturnResponce.NotFoundResponce();

                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
                var currentdata = Logics.ContractLogic.GetContracts(_dbContext, _log, comid, currentcompany.ID);

                return currentdata == null ?
                    ReturnResponce.NotFoundResponce() :
                    ReturnResponce.ListReturnResponce(currentdata);
            }
            else
            {
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
                var currentdata = Logics.ContractLogic.GetStoreContracts(_dbContext, _log, comid);

                return currentdata == null ?
                    ReturnResponce.NotFoundResponce() :
                    ReturnResponce.ListReturnResponce(currentdata);
            }
        }

        [HttpGet]
        [Route("systemcontracts/{regno}/{comid}")]
        [Authorize(Policy = "EdiApiUser")]
        [ServiceFilter(typeof(LogFilter))]

        public ResponseClient SystemContracts(string regno, int comid)
        {
            if (regno != "null")
            {
                var currentcompany = Logics.ManagementLogic.GetOrganization(_dbContext, regno);
                if (currentcompany == null)
                    return ReturnResponce.NotFoundResponce();

                Logics.ContractLogic.Modify(_dbContext, _log, comid, regno);
                var currentdata = Logics.ContractLogic.GetContracts(_dbContext, _log, comid, currentcompany.ID);
                
                return currentdata == null ?
                    ReturnResponce.NotFoundResponce() :
                    ReturnResponce.ListReturnResponce(currentdata);
            }
            else
            {
                var currentdata = Logics.ContractLogic.GetStoreContracts(_dbContext, _log, comid);

                return currentdata == null ?
                    ReturnResponce.NotFoundResponce() :
                    ReturnResponce.ListReturnResponce(currentdata);
            }
        }


        #endregion

        #region StoreList

        /// <summary>
        ///	#Дэлгүүрийн жагсаалт 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <returns>Дэлгүүрийн жагсаалт</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize]
        [Route("storelist")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetStorelist()
        {
            try
            {
                return ReturnResponce.ListReturnResponce(_dbContext.SYSTEM_ORGANIZATION.Where(x => x.ORGTYPE == SystemEnums.ORGTYPE.Дэлгүүр));
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        /// <summary>
        ///	#Бизнесийн байгууллагын жагсаалт
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>Байгууллага </returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize]
        [Route("businesslist")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetBusinesslist()
        {
            try
            {
                var query = _dbContext.SYSTEM_ORGANIZATION.Where(x => x.ORGTYPE == SystemEnums.ORGTYPE.Бизнес).Select(x => new { x.REGNO, x.COMPANYNAME, x.ID });
                return ReturnResponce.ListReturnResponce(query);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        #endregion



        #region OrderMobile
        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("getordermobile")]
        public ResponseClient GetOrderMobileList()
        {
            try
            {
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
                var contracts = (from contract in _dbContext.MST_CONTRACT
                                 join ordermobile in _dbContext.MST_ORDER_MOBILECONFIG
                                 on contract.CONTRACTNO equals ordermobile.CONTRACTNO into xj
                                  from x in xj.DefaultIfEmpty()
                                 
                                 select new
                                 {
                                     contract.BUSINESSID,
                                     contract.CONTRACTNO,
                                     contract.CONTRACDESC,
                                     x.MOBILE,
                                     x.INSYMD,
                                     x.INSEMP,
                                     x.UPDEMP,
                                     x.UPDYMD,
                                     x.EXPORTEDDATE   
                                 }).Where(x=> x.BUSINESSID == comid);
                return ReturnResponce.ListReturnResponce(contracts);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("postordermobile")]
        public ResponseClient Post([FromBody]MST_ORDER_MOBILECONFIG param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
                int userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.UserId));
                param.BUSINESSID = comid;
                var currentordermobile = _dbContext.MST_ORDER_MOBILECONFIG.FirstOrDefault(x => x.BUSINESSID == comid && x.CONTRACTNO == param.CONTRACTNO);
                if (currentordermobile == null)
                {
                    param.INSYMD = DateTime.Now;
                    param.UPDYMD = DateTime.Now;
                    param.INSEMP = userid;

                    param.EXPORTEDDATE = null;
                    _dbContext.MST_ORDER_MOBILECONFIG.Add(param);
                }
                else
                {
                    currentordermobile.MOBILE = param.MOBILE;
                    currentordermobile.UPDYMD = param.UPDYMD;
                    currentordermobile.UPDEMP = param.UPDEMP;
                    param.EXPORTEDDATE = null;
                    _dbContext.Entry(currentordermobile).State = System.Data.Entity.EntityState.Modified;
                }
                _dbContext.SaveChanges();
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        /// <summary>
        ///	#fgdgfd
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("getstoreordermobile")]
        public ResponseClient GetStoreOrderMobileList()
        {
            try
            {
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
                var contracts = (from contract in _dbContext.MST_CONTRACT
                                 join ordermobile in _dbContext.MST_ORDER_MOBILECONFIG
                                 on contract.CONTRACTNO equals ordermobile.CONTRACTNO into xj
                                 from x in xj.DefaultIfEmpty()
                                 join company in _dbContext.SYSTEM_ORGANIZATION
                                 on contract.BUSINESSID equals company.ID

                                 select new
                                 {
                                     company.COMPANYNAME,
                                     company.REGNO,
                                     contract.BUSINESSID,
                                     contract.CONTRACTNO,
                                     contract.CONTRACDESC,
                                     x.MOBILE,
                                     x.INSYMD,
                                     x.INSEMP,
                                     x.UPDEMP,
                                     x.UPDYMD,
                                     x.EXPORTEDDATE
                                 }).OrderBy(x=> x.UPDEMP);
                return ReturnResponce.ListReturnResponce(contracts);

            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("exportstatusmodify")]
        public ResponseClient ModifyOrderMobile()
        {
            try
            {
                foreach (MST_ORDER_MOBILECONFIG mobile in _dbContext.MST_ORDER_MOBILECONFIG)
                {
                    mobile.EXPORTEDDATE = DateTime.Now;
                    _dbContext.Entry(mobile).State = System.Data.Entity.EntityState.Modified;
                }
                _dbContext.SaveChanges();
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        #endregion


    }
}
