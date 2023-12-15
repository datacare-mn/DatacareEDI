using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.IMSModels;
using EDIWEBAPI.Enums;
using EDIWEBAPI.Logics;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.IMSApi
{
    [Route("api/ims")]
    public class IMSController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<IMSController> _logger;
        #endregion

        public IMSController(OracleDbContext dbContext, ILogger<IMSController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region ATTRIBUTE
        [HttpGet]
        [Authorize]
        [Route("attribute")]
        public async Task<ResponseClient> GetAttribute([FromQuery] string deptcd)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/attribute?attrnm={deptcd}").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("attribute")]
        public async Task<ResponseClient> PostAttribute([FromBody] PostAttributeModel model, [FromQuery] string insemp)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/attribute?insemp={insemp}", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        [HttpGet]
        [Authorize]
        [Route("attribute/{id}")]
        public async Task<ResponseClient> GetAttributeId(decimal id)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/attribute/{id}").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        [HttpPut]
        [Authorize]
        [Route("attribute/{id}")]
        public async Task<ResponseClient> PutAttribute([FromBody] PostAttributeModel model, decimal id, [FromQuery] string insemp)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Put($"/api/attribute/{id}?insemp={insemp}", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }

        [HttpDelete]
        [Authorize]
        [Route("attribute/{id}")]
        public async Task<ResponseClient> DeleteAttribute(decimal id)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Delete($"/api/attribute/{id}").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        public class PostAttrValue
        {
            public decimal attrid { get; set; }
            public string value { get; set; }
            public string skucd { get; set; }
        }
        [HttpPost]
        [Authorize]
        [Route("attribute/value")]
        public async Task<ResponseClient> PostAttributeValue([FromBody] PostAttrValue model, [FromQuery] string insemp)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/attribute/value?insemp={insemp}", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }

        [HttpGet]
        [Authorize]
        [Route("attribute/value/subclass/{id}")]
        public async Task<ResponseClient> GetValueSubclass(decimal id)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/attribute/value/subclass/" + id.ToString()).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("attribute/value/subclass/{id}")]
        public async Task<ResponseClient> PutValueSubclass(decimal id, [FromQuery] string insemp, [FromBody] List<string> model)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Put($"/api/attribute/value/subclass/{id}?insemp={insemp}", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("attribute/measure")]
        public async Task<ResponseClient> GetMeasure()
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/attribute/measure").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpGet]
        [Authorize]
        [Route("attribute/sublass/{id}")]
        public async Task<ResponseClient> GetMeasureSubclass(decimal id)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/attribute/subclass/{id}").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        #endregion

        #region CLASS
        [HttpGet]
        [Authorize]
        [Route("class")]
        public async Task<ResponseClient> GetClass([FromQuery] string deptcd, [FromQuery] string subcatnm)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    List<string> depcds = new List<string>();
                    string biDeptcd = null;
                    if (deptcd != null)
                    {
                        var mapping = ContractLogic.GetDepartmentMapping(_dbContext, _logger, Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.UserId)));
                        foreach (var d in mapping)
                        {
                            if (d.DEPARTMENTID == Convert.ToDecimal(deptcd))
                            depcds.Add(d.DEPARTMENTCODE);
                        }
                    }
                    return restUtils.Post($"/api/class?subcatnm={subcatnm}", JsonConvert.SerializeObject(depcds)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpGet]
        [Authorize]
        [Route("class/{subcatcd}")]
        public async Task<ResponseClient> GetClassCatcd(string subcatcd)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/class/{subcatcd}").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        [HttpPut]  
        [Authorize]
        [Route("class/{subcatcd}")]
        public async Task<ResponseClient> PutClass(string subcatcd, [FromBody] List<decimal> model, [FromQuery] string insemp)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Put($"/api/class/{subcatcd}?insemp={insemp}", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        [HttpGet]
        [Authorize]
        [Route("class/department")]
        public async Task<ResponseClient> GetClassDepartment()
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/class/department").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        #endregion

        #region DASHBOARD
        [HttpPost]
        [Authorize(Policy = "StoreApiUser")]
        [Route("dashboard/buyer")]
        public async Task<ResponseClient> DashboardBuyer()
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/dashboard/buyer", "").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("dashboard/vendor")]
        public async Task<ResponseClient> DashboardVendor()
        {
            try
            {
                var list = new List<string>();
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
                    var currentdata = Logics.ContractLogic.GetContracts(_dbContext, _logger, comid);
                    foreach (var c in currentdata)
                        list.Add(c.CONTRACTNO);
                    return restUtils.Post($"/api/dashboard/vendor", JsonConvert.SerializeObject(new { data = list })).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        #endregion

        #region PRODUCT
        [HttpPost]
        [Authorize]
        [Route("product/list/{jumcd}")]
        public async Task<ResponseClient> ProductList([FromQuery] string contractno, [FromQuery] string regno, [FromQuery] decimal? status, string jumcd, [FromQuery] string catcd ,[FromQuery] string subcatcd, [FromQuery] string departmentcode, [FromQuery] string itemcd)
        {
            try
            {
                var list = new List<string>();
                var deptcd = new List<string>();
                int orgType = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.OrgType));
                if ((SystemUserTypes)orgType == SystemUserTypes.Store)
                {
                    var mapping = ContractLogic.GetDepartmentMapping(_dbContext, _logger, Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.UserId)));
                    foreach (var c in mapping)
                        deptcd.Add(c.DEPARTMENTCODE);
                }

                //if (String.IsNullOrEmpty(contractno))
                //{
                //    if (String.IsNullOrEmpty(regno))
                //    {
                //        int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
                //        var currentdata = Logics.ContractLogic.GetContracts(_dbContext, _logger, comid);
                //        foreach (var c in currentdata)
                //            list.Add(c.CONTRACTNO);
                //    }
                //    else
                //    {
                //        var currentcompany = Logics.ManagementLogic.GetOrganization(_dbContext, regno);
                //        if (currentcompany == null)
                //            return ReturnResponce.NotFoundResponce();

                //        int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.CompanyId));
                //        var currentdata = Logics.ContractLogic.GetContracts(_dbContext, _logger, comid, currentcompany.ID);
                //        foreach (var c in currentdata)
                //            list.Add(c.CONTRACTNO);
                //    }
                //    return ReturnResponce.ListReturnResponce(null);
                //}
                //else
                //{
                //    list.Add(contractno);
                //}
                if (!String.IsNullOrEmpty(contractno))
                {
                    list.Add(contractno);
                }
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/product/list", JsonConvert.SerializeObject( new {data = list, jumcd = jumcd, deptcd = deptcd, status = status, catcd = catcd, subcatcd = subcatcd, deptcode = departmentcode, itemcd = itemcd})).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        public class ClassDep
        {
            public string jumcd { get; set; }
            public List<string> data { get; set; }
        }
        [HttpPost]
        [Authorize]
        [Route("product/department")]
        public async Task<ResponseClient> ProductDepartment([FromBody] ClassDep model)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/product/department", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("product/class")]
        public async Task<ResponseClient> ProductClass([FromBody] ClassDep model)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/product/class", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("product/subclass")]
        public async Task<ResponseClient> ProductSubClass([FromBody] ClassDep model)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/product/subclass", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("product/{code}")]
        public async Task<ResponseClient> GetProductCode(string code)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/product/{code}").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        [HttpGet]
        [Authorize]
        [Route("product/image/{code}")]
        public async Task<ResponseClient> GetProductCodeImage(string code)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/product/image/{code}").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        [HttpPut]
        [Authorize]
        [Route("product/image/{code}")]
        public async Task<ResponseClient> PutProductImage(string code, [FromForm] PostProductImage model, [FromQuery] string insemp)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    var multipartContent = new MultipartFormDataContent();
                    foreach (var file in model.file.OrEmptyIfNull())
                    {
                        var fileContent = new StreamContent(file.OpenReadStream())
                        {
                            Headers =  {
                                ContentLength = file.Length,
                                ContentType = new MediaTypeHeaderValue(file.ContentType)
                                 }
                        };
                        multipartContent.Add(fileContent, "file", file.FileName);
                    }
                    foreach (var d in model.ismain)
                    {
                        multipartContent.Add(new StringContent(d.ToString()), "ismain");
                    }
                    foreach (var img in model.imgnm)
                    {
                        multipartContent.Add(new StringContent(img), "imgnm");
                    }
                    foreach (var sku in model.skucds)
                    {
                        multipartContent.Add(new StringContent(sku), "skucds");
                    }
                    return restUtils.PutForm($"/api/product/image/{code}?insemp={insemp}", multipartContent).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
      

        [HttpGet]
        [Authorize]
        [Route("product/attribute/{code}")]
        public async Task<ResponseClient> GetProductAttribute(string code)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/product/attribute/{code}").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        public class PutAttributeProd
        {
            public List<PostProdAttr> attributes { get; set; } = new List<PostProdAttr>();
            public List<string> skucds { get; set; } = new List<string>();
        }
        [HttpPut]
        [Authorize]
        [Route("product/attribute/{code}")]
        public async Task<ResponseClient> PutListAtt(string code, [FromBody] PutAttributeProd model, [FromQuery] string insemp)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Put($"/api/product/attribute/{code}?insemp={insemp}", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        public class ContarctModel
        {
            public List<string> deptcd { get; set; }
            public string deptid { get; set; }
            public string status { get; set; }
        }
        [HttpPost]
        [Route("product/contract")]
        [Authorize]
        public async Task<ResponseClient> ProductApprove([FromBody] ContarctModel model)
        {
            try
            {
                model.deptcd = new List<string>();
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    var mapping = ContractLogic.GetDepartmentMapping(_dbContext, _logger, Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.UserId)));
                    if (!String.IsNullOrEmpty(model.deptid))
                    {
                        var dep = mapping.Where(x => x.DEPARTMENTID == Convert.ToDecimal(model.deptid));
                        foreach (var d in dep)
                            model.deptcd.Add(d.DEPARTMENTCODE);
                    }
                    else
                    {
                        foreach (var c in mapping)
                            model.deptcd.Add(c.DEPARTMENTCODE);
                    }
                    int userId = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.UserId));
                    var user = _dbContext.SYSTEM_USERS.FirstOrDefault(x => x.ID == userId);
                    return restUtils.Post($"/api/product/contract?insemp={(user == null ? "" : user.FIRSTNAME)}", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        
        [HttpGet]
        [Authorize]
        [Route("product/desc/{code}")]
        public async Task<ResponseClient> GetProductDesc(string code)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/product/desc/{code}").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        public class PostDesc 
        {
            public string description { get; set; }
            public List<string> skucds { get; set; } = new List<string>();
        }
        [HttpPut]
        [Authorize]
        [Route("product/desc/{code}")]
        public async Task<ResponseClient> PutListDesc(string code, [FromBody] PostDesc model, [FromQuery] string insemp)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Put($"/api/product/desc/{code}?insemp={insemp}", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        [HttpPost]
        [Authorize]
        [Route("product/approve")]
        public async Task<ResponseClient> ProductApprove([FromBody] List<string> model, [FromQuery] string insemp)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/product/approve?insemp={insemp}", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("product/return")]
        public async Task<ResponseClient> ProductReturn([FromBody] List<string> model, [FromQuery] string reason, [FromQuery] string insemp)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/product/return?reason={reason}&insemp={insemp}", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("product/export/{jumcd}")]
        public async Task<ResponseClient> ProductExport([FromBody] List<string> list, string jumcd)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/product/export", JsonConvert.SerializeObject(new { data = list, jumcd = jumcd})).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("product/import")]
        public async Task<ResponseClient> ProductImport([FromBody] List<PostImport> model, [FromQuery] string insemp)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/product/import?insemp={insemp}", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        public class GetItemcdsModel
        {
            public string ctrcd { get; set; }
            public string deptcd { get; set; }
            public string catcd { get; set; }
            public string jumcd { get; set; }
        }
        [HttpPost]
        [Authorize]
        [Route("product/itemcds")]
        public async Task<ResponseClient> GetItemcds([FromBody] GetItemcdsModel model)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/product/itemcds", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }

        [HttpGet]
        [Authorize]
        [Route("product/itemskus/{code}")]
        public async Task<ResponseClient> GetItemskus(string code)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/product/itemskus/{code}").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }

        }
        public class FilterScango
        {
            public DateTime sdate { get; set; }
            public DateTime edate { get; set; }
            public string skucd { get; set; }
            public string deptcd { get; set; }
            public string catcd { get; set; }
            public string subcatcd { get; set; }
        }
        [HttpPost]
        [Authorize]
        [Route("scango/list")]
        public async Task<ResponseClient> PostScango([FromBody] FilterScango model)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    var deptcd = new List<string>();
                    var mapping = ContractLogic.GetDepartmentMapping(_dbContext, _logger, Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.UserId)));
                    foreach (var c in mapping)
                        deptcd.Add(c.DEPARTMENTCODE);
                    if (String.IsNullOrEmpty(model.deptcd))
                        model.deptcd = String.Join(", ", deptcd);
                    else
                    {
                        if (deptcd.FirstOrDefault(x => x == model.deptcd) == null)
                            model.deptcd = null;
                    }
                    return restUtils.Post($"/api/scango/list", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("changedlist")]
        public async Task<ResponseClient> PostChangedList([FromBody] FilterScango model)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    var deptcd = new List<string>();
                    var mapping = ContractLogic.GetDepartmentMapping(_dbContext, _logger, Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, SystemEnums.UserProperties.UserId)));
                    foreach (var c in mapping)
                        deptcd.Add(c.DEPARTMENTCODE);
                    if (String.IsNullOrEmpty(model.deptcd))
                        model.deptcd = String.Join(", ", deptcd);
                    else
                    {
                        if (deptcd.FirstOrDefault(x => x == model.deptcd) == null)
                            model.deptcd = null;
                    }
                    return restUtils.Post($"/api/product/changedlist", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("changedlistdetail")]
        public async Task<ResponseClient> PostChangedListDetail([FromBody] FilterScango model)
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Post($"/api/product/changedlistdetail", JsonConvert.SerializeObject(model)).Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        #endregion

        #region SYSTEM
        [HttpGet]
        [Authorize]
        [Route("system/product")]
        public async Task<ResponseClient> GetSystemProd()
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/system/product").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpGet]
        [Authorize]
        [Route("system/productmaster")]
        public async Task<ResponseClient> GetSystemProdMaster()
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/system/productmaster").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        [HttpGet]
        [Authorize]
        [Route("system/productitem")]
        public async Task<ResponseClient> GetSystemProdItem()
        {
            try
            {
                IMSUtils restUtils = new IMSUtils(_dbContext);
                if (restUtils.StoreServerConnected)
                {
                    return restUtils.Get($"/api/system/productitem").Result;
                }
                return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        #endregion

        [HttpGet]
        [Route("departments/{id}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetDepartments(decimal id)
        {
            try
            {
                return ReturnResponce.ListReturnResponce((from d in _dbContext.SYSTEM_USER_DEPARTMENT
                                                          join s in _dbContext.MST_DEPARTMENT on d.DEPARTMENTID equals s.ID
                                                          where d.USERID == id
                                                          select s).ToList());
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
    }
}
