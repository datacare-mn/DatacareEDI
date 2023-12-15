using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EDIWEBAPI.Context;
using EDIWEBAPI.Utils;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Entities.DBModel.Test;
using EDIWEBAPI.Entities.TestModel;
using static EDIWEBAPI.Enums.SystemEnums;
using EDIWEBAPI.Entities.Interfaces;
using System.Xml.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security;
using System.Runtime.InteropServices;

namespace EDIWEBAPI.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {

        private readonly OracleDbContext _dbContext;
        readonly ILogger<TestController> _log;

        public TestController(OracleDbContext context, ILogger<TestController> log)
        {
            _dbContext = context;
            _log = log;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("modify/{regno}")]
        public async Task<ResponseClient> Modify(string regno)
        {
            try
            {
                return Logics.ContractLogic.Modify(_dbContext, _log, 2, regno);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("departments/{contractNo}")]
        [AllowAnonymous]
        public ResponseClient GetContractDepartments(string contractNo)
        {
            try
            {
                return ReturnResponce.ListReturnResponce(Logics.MasterLogic.GetContractDepartments(_dbContext, _log, contractNo));
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("mytest/{value}")]
        public async Task<ResponseClient> MyTest(string value)
        {
            try
            {
                var list = Logics.BaseLogic.GetBasicEntities(_dbContext, value);
                return ReturnResponce.ListReturnResponce(list);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        //[HttpGet]
        //[AllowAnonymous]
        //[Route("accounttest/{account}")]
        //public async Task<ResponseClient> PasswordTest(string account)
        //{
        //    try
        //    {
        //        return ReturnResponce.ListReturnResponce(Cryptography.Sha256Hash(account));
        //    }
        //    catch (Exception ex)
        //    {
        //        return ReturnResponce.GetExceptionResponce(ex);
        //    }
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("testmail")]
        //public async Task<ResponseClient> Upload([FromBody] SendData.EmailMessage message)
        //{
        //    try
        //    {
        //        SendData.Emailer.Send(message);
        //        return ReturnResponce.SuccessMessageResponce("AAA");
        //    }
        //    catch (Exception ex)
        //    {
        //        return ReturnResponce.GetExceptionResponce(ex);
        //    }
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("testupload")]
        //public async Task<ResponseClient> Upload(IFormFile uploadedfile)
        //{
        //    try
        //    {
        //        var restUtils = new HttpRestUtils(2, _dbContext);
        //        if (!restUtils.StoreServerConnected)
        //            return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

        //        return restUtils.Post($"/api/reciveorder/attachimage", uploadedfile).Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return ReturnResponce.GetExceptionResponce(ex);
        //    }
        //}

        [HttpGet]
        [AllowAnonymous]
        [Route("phonetest/{phoneno}")]

        public async Task<ResponseClient> PhoneTest(string phoneno = null)
        {
            return UsefulHelpers.IsActualPhone(phoneno) ?
                ReturnResponce.SuccessMessageResponce("") :
                ReturnResponce.FailedMessageResponce("");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("test/{storeid}/{sdate}/{edate}/{key}/{sendmessage}")]
        public async Task<ResponseClient> Test(int storeid, DateTime? sdate, DateTime? edate, string key, bool sendmessage)
        {
            try
            {
                if (!SendData.MailSendController.HasPrivilige(key))
                    return ReturnResponce.AccessDeniedResponce();

                var organizations = Logics.TestLogic.GetOrganizations(_dbContext, storeid);

                if (organizations == null || !organizations.Any())
                    return ReturnResponce.NotFoundResponce();

                if (!sdate.HasValue || !edate.HasValue)
                {
                    var currentDate = DateTime.Today.Day == 1 ? DateTime.Today.AddDays(-1) : DateTime.Today;

                    sdate = new DateTime(currentDate.Year, currentDate.Month, 1);
                    edate = new DateTime(currentDate.Year,
                        currentDate.Month,
                        DateTime.Today.Day == 1 ? DateTime.DaysInMonth(currentDate.Year, currentDate.Month) : currentDate.Day);
                }

                IEnumerable<OrganizationDetailModel> cases = null;
                ResponseClient response = null;
                var messageMapping = new Dictionary<int, string>();
                foreach (var organization in organizations)
                {
                    cases = Logics.TestLogic.GetDetails(_dbContext, organization.ID);
                    if (cases == null || !cases.Any()) continue;

                    foreach (var method in cases)
                    {
                        try
                        {
                            response = GetResponse(storeid, organization, method, sdate.Value, edate.Value);
                        }
                        catch (Exception ex)
                        {
                            response = new ResponseClient()
                            {
                                Success = false,
                                Message = $"Test.GetResponse : {ex.Message}"
                            };
                        }

                        var success = method.RESPONSE == 2 ?
                            (response.Success ? response.RowCount > 0 : false) :
                            response.Success;

                        var newLog = new SYSTEM_TEST_LOG()
                        {
                            ID = Convert.ToInt16(Logics.BaseLogic.GetNewId(_dbContext, "SYSTEM_TEST_LOG")),
                            CASEID = method.ID,
                            DETAILID = method.DETAILID,
                            TESTID = organization.ID,
                            LOGDATE = DateTime.Today,
                            TESTDATE = DateTime.Now,
                            SUCCESS = success ? 1 : 0,
                            MESSAGE = response.Success ? "" : response.Message,
                            RESULTCOUNT = response.Success ? response.RowCount : 0
                        };

                        if (!success && method.TYPE == 2)
                        {
                            var message = messageMapping.ContainsKey(organization.ORGANIZATIONID) ?
                                messageMapping[organization.ORGANIZATIONID] :
                                organization.DESCRIPTION;

                            message = $"{message} {method.DESCRIPTION}, ";

                            if (messageMapping.ContainsKey(organization.ORGANIZATIONID))
                                messageMapping[organization.ORGANIZATIONID] = message;
                            else
                                messageMapping.Add(organization.ORGANIZATIONID, message);
                        }

                        Logics.BaseLogic.Insert(_dbContext, newLog, false);
                    }
                }
                _dbContext.SaveChanges();

                var warnings = new List<string>();

                foreach (var message in messageMapping.Values)
                {
                    var warning = $"{(message.Length > 2 ? message.Substring(0, message.Length - 2) : message)} шалгана уу.";
                    warnings.Add(warning);
                    if (sendmessage)
                        Logics.MailLogic.AddSMS(_dbContext, MessageType.None, warning, SendData.Messager.DEVELOPER_PHONE_NO);
                }

                return ReturnResponce.ListReturnResponce(warnings);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        private ResponseClient GetResponse(int storeid, OrganizationContractModel organization, OrganizationDetailModel currentCase,
            DateTime sdate, DateTime edate)
        {
            var result = new ResponseClient();
            if (currentCase.CONTROLLER.ToUpper() == "PAYMENTCONTROLLER")
            {
                if (currentCase.ROUTE.ToUpper() == "GET")
                    result = Logics.PaymentLogic.GetPayment(_dbContext, _log, storeid, organization.ORGANIZATIONID, organization.REGNO, organization.CONTRACTNO,
                        sdate, edate);
            }
            else if (currentCase.CONTROLLER.ToUpper() == "PAYMENTSTORECONTROLLER")
            {
                if (currentCase.ROUTE.ToUpper() == "GET")
                    result = Logics.PaymentLogic.GetStorePayment(_dbContext, _log, storeid, organization.REGNO, organization.CONTRACTNO,
                        sdate, edate, false, false);
            }
            else if (currentCase.CONTROLLER.ToUpper() == "PAYMENTREPORTCONTROLLER")
            {
                if (currentCase.ROUTE.ToUpper() == "GET")
                    result = Logics.BILogic.GetPaymentReport(_dbContext, _log, storeid, organization.REGNO, sdate, edate);
            }
            else if (currentCase.CONTROLLER.ToUpper() == "ORDERCONTROLLER")
            {
                if (currentCase.ROUTE.ToUpper() == "GETORDERHEADER")
                    result = Logics.OrderLogic.GetOrderHeader(_dbContext, storeid, organization.ORGANIZATIONID, "%", organization.CONTRACTNO, sdate, edate, "%");
            }
            else if (currentCase.CONTROLLER.ToUpper() == "BUYINGCONTROLLER")
            {
                if (currentCase.ROUTE.ToUpper() == "GETBUYINGHEADER")
                    result = Logics.BILogic.GetBuyingHeader(_dbContext, storeid, organization.REGNO, organization.CONTRACTNO, sdate, edate, string.Empty);
            }

            return result;
        }
    }
}
