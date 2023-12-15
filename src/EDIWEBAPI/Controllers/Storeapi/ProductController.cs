using EDIWEBAPI.Attributes;
using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Entities.DBModel.Product;
using EDIWEBAPI.Entities.DBModel.MasterSku;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;
using System.IO;
using EDIWEBAPI.Entities.Interfaces;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<ProductController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public ProductController(OracleDbContext context, ILogger<ProductController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        #region UNUSED

        [HttpGet]
        [Route("departments")]
        [Authorize]
        public ResponseClient GetDepartments()
        {
            try
            {
                var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
                var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                return ReturnResponce.ListReturnResponce(Logics.MasterLogic.GetDepartments(_dbContext, _log, orgType, userId));
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("contractdepartments/{contractNo}")]
        [Authorize]
        public ResponseClient GetContractDepartments(string contractNo)
        {
            try
            {
                return ReturnResponce.ListReturnResponce(Logics.MasterLogic.GetContractDepartments(_dbContext, _log, contractNo));
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Route("getproducts")]
        [Authorize]
        public ResponseClient GetProducts([FromBody] Entities.FilterViews.ProductFilterView filter)
        {
            try
            {
                filter.ORGID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
                var response = Logics.MasterLogic.GetProducts(_dbContext, _log, filter);
                return ReturnResponce.ListReturnResponce(response);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("getproduct/{id}")]
        [Authorize]
        protected internal ResponseClient GetProduct(int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var product = Logics.MasterLogic.GetProduct(_dbContext, _log, id);
                if (product == null)
                    return ReturnResponce.NotFoundResponce();

                var details = Logics.MasterLogic.GetProductStores(_dbContext, _log, product.ID);
                var container = new ProductView()
                {
                    Product = product,
                    Details = details
                };
                return ReturnResponce.ListReturnResponce(container);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Route("product")]
        [Authorize]

        public ResponseClient AddProduct([FromBody]MST_PRODUCT param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                param.ID = Convert.ToInt16(Logics.BaseLogic.GetNewId(_dbContext, "MST_PRODUCT"));
                param.ORGID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                param.IMAGEQTY = 0;
                param.STOREQTY = 0;
                param.ENABLED = 1;
                param.CREATEDBY = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
                param.CREATEDDATE = DateTime.Now;

                Logics.BaseLogic.Insert(_dbContext, param);

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPut]
        [Route("product")]
        [Authorize]
        public ResponseClient SetProduct([FromBody]MST_PRODUCT param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var found = Logics.MasterLogic.GetProduct(_dbContext, _log, param.ID);
                if (found == null)
                    return ReturnResponce.NotFoundResponce();

                found.BARCODE = param.BARCODE;
                found.BRANDNAME = param.BRANDNAME;
                found.DEPARTMENTID = param.DEPARTMENTID;
                found.MEASUREID = param.MEASUREID;
                found.NAME = param.NAME;
                found.PRICE = param.PRICE;
                found.STORENAME = param.STORENAME;

                Logics.BaseLogic.Update(_dbContext, found);

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(System.Reflection.MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        #endregion

        [HttpGet]
        [Authorize]
        [Route("statuses")]
        public async Task<ResponseClient> GetStatuses()
        {
            try
            {
                var statuses = from s in _dbContext.MST_PRODUCT_STATUS
                               orderby s.VIEWORDER
                               select s;

                return ReturnResponce.ListReturnResponce(statuses.ToList());
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("products/{storeid}/{branchcode}/{contractno}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetProducts(int storeid, string branchcode, string contractno)
        {
            try
            {
                var businessid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));

                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = storeid,
                    CompanyId = businessid,
                    ContractNo = contractno,
                    BeginDate = DateTime.Today,
                    Index = 9,
                    Controller = "",
                    Route = ""
                };
                var response = Logics.ReportLogic.GetReportData<StockModel>(_dbContext, request).Result;
                if (!response.Success || UsefulHelpers.IsNull(branchcode))
                    return response;

                var values = (List<StockModel>)response.Value;
                return ReturnResponce.ListReturnResponce(values.Where(a => a.branch.StartsWith(branchcode)).Select(a => a).ToList());
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        [Route("storeproducts/{storeid}/{regno}/{branchcode}/{contractno}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetStoreProducts(int storeid, string regno, string branchcode, string contractno)
        {
            try
            {
                var businessid = Logics.ManagementLogic.GetOrganization(_dbContext, regno).ID;

                var request = new Entities.RequestModels.ReportRequest()
                {
                    StoreId = storeid,
                    CompanyId = businessid,
                    ContractNo = contractno,
                    BeginDate = DateTime.Today,
                    Index = 9,
                    Controller = "",
                    Route = ""
                };
                var response = Logics.ReportLogic.GetReportData<StockModel>(_dbContext, request).Result;
                if (!response.Success || UsefulHelpers.IsNull(branchcode))
                    return response;

                var values = (List<StockModel>)response.Value;
                return ReturnResponce.ListReturnResponce(values.Where(a => a.branch.StartsWith(branchcode)).Select(a => a).ToList());
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Route("getrequests")]
        [Authorize]
        public ResponseClient GetProductRequests([FromBody] Entities.FilterViews.ProductRequestFilterView filter)
        {
            try
            {
                if (filter == null)
                    return ReturnResponce.ModelIsNotValudResponce();
                
                var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
                Entities.ResultModels.ProductRequestListModel response = null;
                filter.ENDDATE = filter.ENDDATE.AddDays(1);

                if (orgType == (int)ORGTYPE.Бизнес)
                {
                    filter.ORGID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                    response = Logics.MasterLogic.GetProductRequests(_dbContext, _log, filter);
                }
                else if (orgType == (int)ORGTYPE.Дэлгүүр)
                {
                    filter.STOREID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                    filter.USERID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
                    response = Logics.MasterLogic.GetStoreProductRequests(_dbContext, _log, filter);
                }

                return response == null ?
                    ReturnResponce.NotFoundResponce() :
                    ReturnResponce.ListReturnResponce(response);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Route("orgrequest")]
        [AllowAnonymous]
        public ResponseClient AddOrgRequest(IList<IFormFile> files, string json)
        {
            _log.LogInformation($"PRODUCT.ADDORGREQUEST STARTED");
            //if (files == null || files.Count == 0)
            //    return ReturnResponce.NotFoundResponce();
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var param = JsonConvert.DeserializeObject<OrgRequestDto>(json);
                var orgRequest = _dbContext.MST_PRODUCT_REQUEST.FirstOrDefault(a => a.CODE == "ORG");
                if (orgRequest == null)
                    return ReturnResponce.NotFoundResponce();

                var request = new REQ_PRODUCT_ORG()
                {
                    REGNO = param.REGNO,
                    ORGNAME = param.ORGNAME,
                    CEONAME = param.CEONAME,
                    EMAIL = param.EMAIL,
                    MOBILE = param.MOBILE,
                    DEPARTMENTID = param.DEPARTMENTID,
                    REQUESTID = orgRequest.ID,
                    STOREID = UsefulHelpers.STORE_ID,
                    REQUESTBY = param.EMAIL,
                    ATTACHMENT = files.Count > 0 ? 1 : 0
                };

                _log.LogInformation($"PRODUCT.ADDORGREQUEST LOGIC CALLED");
                return Logics.MasterLogic.AddRequest<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_IMAGE, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, 
                        request, files, 0, param.NOTE);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Route("productrequest")]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient AddProductRequest(IList<IFormFile> files, string json)
        {
            //if (files == null || files.Count == 0)
            //    return ReturnResponce.NotFoundResponce();
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var param = JsonConvert.DeserializeObject<RequestProductDto>(json);
                var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                var request = new REQ_PRODUCT()
                {
                    CONTRACTID = param.CONTRACTID,
                    CONTRACTNO = param.CONTRACTNO,
                    CONTRACTDESC = param.CONTRACTDESC,
                    DEPARTMENTID = param.DEPARTMENTID,
                    REQUESTID = param.REQUESTID,
                    STOREID = param.STOREID,
                    ORGID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId)),
                    REQUESTBY = userId,
                    ATTACHMENT = files.Count > 0 ? 1 : 0
                };

                return Logics.MasterLogic.AddRequest<REQ_PRODUCT, REQ_PRODUCT_IMAGE, REQ_PRODUCT_LOG>(_dbContext, _log, request, files, userId, param.NOTE);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPut]
        [Route("productrequest")]
        [Authorize]
        public ResponseClient SetProductRequest(IList<IFormFile> files, string json)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var param = JsonConvert.DeserializeObject<RequestProductDto>(json);
                var found = (REQ_PRODUCT)Logics.MasterLogic.GetProductRequest(_dbContext, _log, param.ID);
                if (found == null)
                    return ReturnResponce.NotFoundResponce();

                var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));

                // ХЭРВЭЭ МЭДЭЭЛЭЛ ЗАСЧ БАЙВАЛ
                if (found.CONTRACTNO != param.CONTRACTNO
                    || found.DEPARTMENTID != param.DEPARTMENTID)
                {
                    var status = Logics.MasterLogic.GetStatus(_dbContext, _log, found.STATUS);
                    if (status.EDITABLE != 1)
                        throw new Exception($"{status.NAME} төлөвтэй үед засах боломжгүй.");

                    if (found.DEPARTMENTID != param.DEPARTMENTID)
                    {
                        var notifdata = new SYSTEM_NOTIFCATION_DATA()
                        {
                            COMID = 1,
                            ID = Convert.ToString(Guid.NewGuid()),
                            NOTIFMODULETYPE = 4,
                            CREATEDDATE = DateTime.Now,
                            RECORDID = found.ID,
                            STOREID = found.STOREID,
                            ISSEEN = 0
                        };

                        Logics.BaseLogic.Insert(_dbContext, notifdata);
                    }
                    found.CONTRACTNO = param.CONTRACTNO;
                    found.DEPARTMENTID = param.DEPARTMENTID;
                    found.STOREID = param.STOREID;

                    var newLog = new REQ_PRODUCT_LOG()
                    {
                        ID = Convert.ToInt32(Logics.BaseLogic.GetNewId(_dbContext, typeof(REQ_PRODUCT_LOG).Name)),
                        HEADERID = found.ID,
                        USERID = userId,
                        ORGTYPE = ORGTYPE.Бизнес,
                        TYPE = RequestLogType.Edited,
                        STATUS = found.STATUS,
                        ACTIONDATE = DateTime.Now
                    };

                    Logics.BaseLogic.Update(_dbContext, found, false);
                    Logics.BaseLogic.Insert(_dbContext, newLog, false);

                    _dbContext.SaveChanges();
                }

                if (string.IsNullOrEmpty(param.NOTE))
                    return ReturnResponce.SaveSucessResponce();

                // ХЭРВЭЭ ТАЙЛБАР НЭМСЭН БОЛ
                var requestNote = new RequestNoteDto()
                {
                    Id = param.ID,
                    Note = param.NOTE
                };

                if (files == null || files.Count == 0)
                    return Logics.MasterLogic.AddNote<REQ_PRODUCT, REQ_PRODUCT_LOG>(_dbContext, _log, requestNote,
                        orgType, NotifcationType.Бараа, userId);
                else
                    return Logics.MasterLogic.AddImages<REQ_PRODUCT, REQ_PRODUCT_IMAGE, REQ_PRODUCT_LOG>(_dbContext, _log, requestNote, files,
                        orgType, NotifcationType.Бараа, userId);

            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPut]
        [Route("orgstorerequest")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient SetOrgStoreRequest(IList<IFormFile> files, string json)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var param = JsonConvert.DeserializeObject<OrgRequestDto>(json);
                var found = (REQ_PRODUCT_ORG)Logics.MasterLogic.GetOrgRequest(_dbContext, _log, param.ID);
                if (found == null)
                    return ReturnResponce.NotFoundResponce();
                
                var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));

                // STORE - С ЗӨВХӨН АНГИЛЛЫН МЭДЭЭЛЭЛ ЗАСНА
                if (found.DEPARTMENTID != param.DEPARTMENTID)
                {
                    var status = Logics.MasterLogic.GetStatus(_dbContext, _log, found.STATUS);
                    if (status.EDITABLE != 1)
                        throw new Exception($"{status.NAME} төлөвтэй үед засах боломжгүй.");

                    //found.REGNO = param.REGNO;
                    //found.ORGNAME = param.ORGNAME;
                    //found.CEONAME = param.CEONAME;
                    //found.MOBILE = param.MOBILE;
                    found.DEPARTMENTID = param.DEPARTMENTID;

                    var newLog = new REQ_PRODUCT_ORG_LOG()
                    {
                        ID = Convert.ToInt32(Logics.BaseLogic.GetNewId(_dbContext, typeof(REQ_PRODUCT_ORG_LOG).Name)),
                        HEADERID = found.ID,
                        USERID = 0,
                        ORGTYPE = ORGTYPE.Бизнес,
                        TYPE = RequestLogType.Edited,
                        STATUS = found.STATUS,
                        ACTIONDATE = DateTime.Now
                    };

                    Logics.BaseLogic.Update(_dbContext, found, false);
                    Logics.BaseLogic.Insert(_dbContext, newLog, false);

                    _dbContext.SaveChanges();
                }

                // ХЭРВЭЭ ТАЙЛБАР НЭМСЭН БОЛ
                if (!string.IsNullOrEmpty(param.NOTE))
                {
                    var requestNote = new RequestNoteDto()
                    {
                        Id = param.ID,
                        Note = param.NOTE
                    };

                    if (files == null || files.Count == 0)
                        Logics.MasterLogic.AddNote<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, requestNote,
                            orgType, NotifcationType.ШинэХарилцагч, userId);
                    else
                        Logics.MasterLogic.AddImages<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_IMAGE, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, requestNote, files,
                            orgType, NotifcationType.ШинэХарилцагч, userId);
                }
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPut]
        [Route("orgrequest")]
        [AllowAnonymous]
        public ResponseClient SetOrgRequest(IList<IFormFile> files, string json)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var param = JsonConvert.DeserializeObject<OrgRequestDto>(json);
                var found = (REQ_PRODUCT_ORG) Logics.MasterLogic.GetOrgRequest(_dbContext, _log, param.ID);
                if (found == null)
                    return ReturnResponce.NotFoundResponce();

                // ХЭРВЭЭ МЭДЭЭЛЭЛ ЗАСЧ БАЙВАЛ
                if (found.REGNO != param.REGNO
                    || found.ORGNAME != param.ORGNAME
                    || found.CEONAME != param.CEONAME
                    || found.MOBILE != param.MOBILE
                    || found.DEPARTMENTID != param.DEPARTMENTID)
                {
                    var status = Logics.MasterLogic.GetStatus(_dbContext, _log, found.STATUS);
                    if (status.EDITABLE != 1)
                        throw new Exception($"{status.NAME} төлөвтэй үед засах боломжгүй.");

                    found.REGNO = param.REGNO;
                    found.ORGNAME = param.ORGNAME;
                    found.CEONAME = param.CEONAME;
                    found.MOBILE = param.MOBILE;
                    found.DEPARTMENTID = param.DEPARTMENTID;

                    var newLog = new REQ_PRODUCT_ORG_LOG()
                    {
                        ID = Convert.ToInt16(Logics.BaseLogic.GetNewId(_dbContext, typeof(REQ_PRODUCT_ORG_LOG).Name)),
                        HEADERID = found.ID,
                        USERID = 0,
                        ORGTYPE = ORGTYPE.Бизнес,
                        TYPE = RequestLogType.Edited,
                        STATUS = found.STATUS,
                        ACTIONDATE = DateTime.Now
                    };

                    Logics.BaseLogic.Update(_dbContext, found, false);
                    Logics.BaseLogic.Insert(_dbContext, newLog, false);

                    _dbContext.SaveChanges();
                }

                // ХЭРВЭЭ ТАЙЛБАР НЭМСЭН БОЛ
                if (!string.IsNullOrEmpty(param.NOTE))
                {
                    var requestNote = new RequestNoteDto()
                    {
                        Id = param.ID,
                        Note = param.NOTE
                    };

                    if (files == null || files.Count == 0)
                        Logics.MasterLogic.AddNote<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, requestNote,
                            (int) ORGTYPE.Бизнес, NotifcationType.ШинэХарилцагч, 0);
                    else
                        Logics.MasterLogic.AddImages<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_IMAGE, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, requestNote, files,
                            (int)ORGTYPE.Бизнес, NotifcationType.ШинэХарилцагч, 0);
                }

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("getproductrequest/{type}/{id}")]
        [Authorize]
        public ResponseClient GetProductRequest(string type, int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
            var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));

            return UsefulHelpers.IsNewOrgType(type) ?
                Logics.MasterLogic.GetRequest<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, id, orgType, userId) :
                Logics.MasterLogic.GetRequest<REQ_PRODUCT, REQ_PRODUCT_LOG>(_dbContext, _log, id, orgType, userId);
        }

        [HttpGet]
        [Route("getrequestnote/{id}/{email}")]
        [AllowAnonymous]
        public ResponseClient GetOrgRequest(int id, string email)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var log = _dbContext.REQ_PRODUCT_ORG_LOG.FirstOrDefault(a => a.ID == id);
                if (log == null)
                    return ReturnResponce.NotFoundResponce();

                var found = _dbContext.REQ_PRODUCT_ORG.FirstOrDefault(a => a.ID == log.HEADERID && a.REQUESTBY == email);
                if (found == null)
                    return ReturnResponce.NotFoundResponce();

                if (log.SEEN != 1)
                {
                    log.SEEN = 1;
                    Logics.BaseLogic.Update(_dbContext, log);
                }

                return Logics.MasterLogic.GetRequest<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, found.ID, (int)ORGTYPE.Бизнес, 0);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpDelete]
        [Route("productrequest/{type}/{id}/{imageId}")]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient DeleteProductRequest(string type, int id, int imageId)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var orgId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));

            return UsefulHelpers.IsNewOrgType(type) ?
                Logics.MasterLogic.DeleteImage<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_IMAGE>(_dbContext, _log, id, imageId, orgId, userId) :
                Logics.MasterLogic.DeleteImage<REQ_PRODUCT, REQ_PRODUCT_IMAGE>(_dbContext, _log, id, imageId, orgId, userId);
        }

        [HttpDelete]
        [Route("orgrequest/{id}/{imageId}")]
        [AllowAnonymous]
        public ResponseClient DeleteOrgRequest(int id, int imageId)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            return Logics.MasterLogic.DeleteImage<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_IMAGE>(_dbContext, _log, id, imageId, 0, 0);
        }

        [HttpPost]
        [Route("productrequestimage")]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient AddProductRequestImage(IList<IFormFile> files, string json)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var param = JsonConvert.DeserializeObject<RequestNoteDto>(json);
                var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));

                return UsefulHelpers.IsNewOrgType(param.Type) ?
                    Logics.MasterLogic.AddImages<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_IMAGE, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, param, files, 
                        orgType, NotifcationType.ШинэХарилцагч, userId) :
                    Logics.MasterLogic.AddImages<REQ_PRODUCT, REQ_PRODUCT_IMAGE, REQ_PRODUCT_LOG>(_dbContext, _log, param, files, 
                        orgType, NotifcationType.Бараа, userId);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Route("orgrequestimage")]
        [AllowAnonymous]
        public ResponseClient AddOrgRequestImage(IList<IFormFile> files, string json)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            try
            {
                var param = JsonConvert.DeserializeObject<RequestNoteDto>(json);
                return Logics.MasterLogic.AddImages<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_IMAGE, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, param, files, 
                    (int) ORGTYPE.Бизнес, NotifcationType.ШинэХарилцагч, 0);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Route("productrequestnote")]
        [Authorize]
        public ResponseClient AddProductRequestNote([FromBody] RequestNoteDto param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));

            return UsefulHelpers.IsNewOrgType(param.Type) ? 
                Logics.MasterLogic.AddNote<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, param, 
                    orgType, NotifcationType.ШинэХарилцагч, userId) :
                Logics.MasterLogic.AddNote<REQ_PRODUCT, REQ_PRODUCT_LOG>(_dbContext, _log, param, 
                    orgType, NotifcationType.Бараа, userId);
        }

        [HttpPost]
        [Route("orgrequestnote")]
        [AllowAnonymous]
        public ResponseClient AddOrgRequestNote([FromBody] RequestNoteDto param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            
            return Logics.MasterLogic.AddNote<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, param, 
                (int) ORGTYPE.Бизнес, NotifcationType.ШинэХарилцагч, 0);
        }

        [HttpGet]
        [Route("productrequestlogs/{type}/{id}")]
        [Authorize]
        public ResponseClient GetProductRequestLogs(string type, int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            var orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));

            return UsefulHelpers.IsNewOrgType(type) ?
                Logics.MasterLogic.GetNotes<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, id, orgType) :
                Logics.MasterLogic.GetNotes<REQ_PRODUCT, REQ_PRODUCT_LOG>(_dbContext, _log, id, orgType);
        }

        [HttpGet]
        [Route("requeststatus/{type}/{id}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetRequestStatuses(string type, int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            return UsefulHelpers.IsNewOrgType(type) ?
                Logics.MasterLogic.GetStatuses<REQ_PRODUCT_ORG>(_dbContext, _log, id) :
                Logics.MasterLogic.GetStatuses<REQ_PRODUCT>(_dbContext, _log, id);
        }

        [HttpPost]
        [Route("requeststatuses")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetStatuses([FromBody] List<RequestModel> records)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            return Logics.MasterLogic.GetStatuses(_dbContext, _log, records);
        }

        [HttpPost]
        [Route("productrequeststatuses")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient SetProductRequestStatuses([FromBody] RequestMultiNote param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var unsuccessful = 0;
                var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));

                foreach (var current in param.Requests)
                {
                    var newStatus = Logics.MasterLogic.GetStatusId(_dbContext, _log, param.Status, current.TYPE);
                    if (newStatus == null)
                    {
                        unsuccessful++;
                        continue;
                    }

                    try
                    {
                        if (UsefulHelpers.IsNewOrgType(current.TYPE))
                            Logics.MasterLogic.ChangeStatus<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_LOG>(_dbContext, _log,
                                current.ID, param.Note, userId, newStatus);
                        else
                            Logics.MasterLogic.ChangeStatus<REQ_PRODUCT, REQ_PRODUCT_LOG>(_dbContext, _log,
                                current.ID, param.Note, userId, newStatus);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                        unsuccessful++;
                    }
                }
                return unsuccessful > 0 ?
                    ReturnResponce.FailedMessageResponce($"{unsuccessful} амжилтгүй боллоо.") :
                    ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Route("productrequeststatus")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient SetProductRequestStatus([FromBody] RequestNoteDto param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            try
            {
                var userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                var newStatus = Logics.MasterLogic.GetStatus(_dbContext, _log, param.Status);

                if (UsefulHelpers.IsNewOrgType(param.Type))
                    Logics.MasterLogic.ChangeStatus<REQ_PRODUCT_ORG, REQ_PRODUCT_ORG_LOG>(_dbContext, _log, 
                        param.Id, param.Note, userId, newStatus);
                else
                    Logics.MasterLogic.ChangeStatus<REQ_PRODUCT, REQ_PRODUCT_LOG>(_dbContext, _log, 
                        param.Id, param.Note, userId, newStatus);

                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("sendnewrequests/{key}/{value}")]
        [AllowAnonymous]
        public ResponseClient SendNewRequests(string key, DateTime value)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();

            if (!UsefulHelpers.CheckSecurityKey(key))
                return ReturnResponce.AccessDeniedResponce();

            try
            {
                Logics.MasterLogic.SendNewRequests(_dbContext, _log, value);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod()));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
    }
}
