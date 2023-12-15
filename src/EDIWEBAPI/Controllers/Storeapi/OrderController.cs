using EDIWEBAPI.Attributes;
using EDIWEBAPI.Context;
using EDIWEBAPI.Controllers.SendData;
using EDIWEBAPI.Controllers.SysManagement;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Entities.DBModel.Order;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Entities.RequestModels;
using EDIWEBAPI.SystemResource;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/orderdata")]
    public class OrderController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<OrderController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="log"></param>
        public OrderController(OracleDbContext context, ILogger<OrderController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion



        [HttpGet]
        [Authorize]
        [Route("orderpreview/{shortlink}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> PreviewOrderFile(string shortlink)
        {
            int userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            try
            {
                var url = Logics.OrderLogic.GetShortUrl(_dbContext, _log, shortlink);
                if (url == null)
                    return ReturnResponce.NotFoundResponce();

                var order = Logics.OrderLogic.GetOrder(_dbContext, _log, url);
                if (order == null)
                    return ReturnResponce.NotFoundResponce();

                order.ISSEEN = 1;
                order.SEENDATE = DateTime.Now;
                order.SEENUSER = userid;

                Logics.BaseLogic.Update(_dbContext, order);

                return ReturnResponce.SuccessMessageResponce(url.LONGURL);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, $"{UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod())} => {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpPost]
        [Route("vieworderfile")]
        [AllowAnonymous]
        public async Task<ResponseClient> OrderFileView([FromBody]FilePreviewModel value)
        {
            if (value == null)
                return ReturnResponce.NotFoundResponce();
            try
            {
                var url = Logics.OrderLogic.GetShortUrl(_dbContext, _log, value.url);
                if (url == null)
                    return ReturnResponce.NotFoundResponce();

                var order = Logics.OrderLogic.GetOrder(_dbContext, _log, url);
                if (order == null)
                    return ReturnResponce.NotFoundResponce();

                order.ISSEEN = 1;
                order.SEENDATE = DateTime.Now;
                //order.SEENUSER = userid;

                Logics.BaseLogic.Update(_dbContext, order);

                return ReturnResponce.ListReturnResponce(url.LONGURL);
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, $"{UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod())} => {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reciveorder")]
        public async Task<ResponseClient> ReciveOrder([FromBody]ReciveOrder param)
        {
            //var logMethod = UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod());
            var logMethod = "ORDERCONTROLLER.RECEIVEORDER";
            try
            {
                _log.LogInformation($"{logMethod} START {param.ContractNo} {param.RegNo} : {DateTime.Now}", param.ContractNo);

                if (!ModelState.IsValid)
                    return ReturnResponce.ModelIsNotValudResponce();

                // САРЫН ӨМНӨХ БҮХ ЗАХИАЛГЫГ УСТГАДАГ ХЭСЭГ
                //Logics.OrderLogic.RemoveOld(_dbContext);

                //2023-04-17 zahialgiin mail burtgehgui bsan tul shalgalt hiiv
                if (String.IsNullOrEmpty(param.FileUrl))
                {
                    _log.LogInformation($"{logMethod} SHORT URL START : {param.OrderNo}", param.ContractNo);
                    return ReturnResponce.ModelIsNotValudResponce();
                }
                decimal? recordid = 0;
                var oldorder = Logics.OrderLogic.Get(_dbContext, param.OrderDate, param.OrderNo, param.ContractNo);
                if (oldorder == null)
                {
                    var order = new REQ_ORDER()
                    {
                        ORDERID = Logics.BaseLogic.GetNewId(_dbContext, "REQ_ORDER"),
                        ORDERNO = param.OrderNo,
                        CONTRACTNO = param.ContractNo,
                        ORDERDATE = param.OrderDate,
                        CREATEDDATE = DateTime.Now,
                        FILEURL = param.FileUrl
                    };

                    Logics.BaseLogic.Insert(_dbContext, order);
                    recordid = order.ORDERID;

                }
                else
                {
                    // SYSTEM_SHORTURL - ААРАА ЯВАХ УЧРААС ШААРДЛАГАГҮЙ
                    //oldorder.FILEURL = param.FileUrl;
                    //Logics.BaseLogic.Update(_dbContext, oldorder);
                    recordid = oldorder.ORDERID;
                }
                _log.LogInformation($"{logMethod} SHORT URL START : {recordid}", param.ContractNo);

                var controller = new ServiceUtilsController(_dbContext, null);
                _log.LogInformation($"{logMethod} SHORT URL START 1: {recordid}", param.ContractNo);
                var rsshortUrl = await controller.GetUrlShortner(param.FileUrl, Enums.SystemEnums.MessageType.Order, recordid);
                _log.LogInformation($"{logMethod} SHORT URL START 2: {recordid}", rsshortUrl);
                var shorturl = rsshortUrl.Success ? Convert.ToString(rsshortUrl.Value) : string.Empty;

                _log.LogInformation($"{logMethod} SHORT URL END : {shorturl}", param.ContractNo);

                //var currentcontract = Logics.OrderLogic.GetContract(_dbContext, param.ContractNo);
                // ТУХАЙН БАЙГУУЛЛАГА БҮРТГЭГДСЭНИЙ ДАРАА НЭВТРЭХЭД ТУХАЙН ГЭРЭЭНҮҮДИЙГ АВТОМАТААР ТАТНА 
                //if (currentcontract == null && !string.IsNullOrEmpty(param.RegNo) && !string.IsNullOrWhiteSpace(param.RegNo))
                //{
                //    var organization = Logics.ManagementLogic.GetOrganization(_dbContext, param.RegNo);
                //    if (organization == null)
                //    {
                //        _log.LogInformation($"{logMethod} ADD ORGANIZATION {param.RegNo} : {DateTime.Now}", param.ContractNo);
                //        organization = Logics.ManagementLogic.AddOrganization(_dbContext, _log, param);
                //    }

                //    _log.LogInformation($"{logMethod} ADD CONTRACT {param.ContractNo} : {DateTime.Now}", param.ContractNo);
                //    currentcontract = Logics.OrderLogic.AddContract(_dbContext, _log, 2, organization.ID, param);
                //}
                var storeid = 2;
                //var storeid = currentcontract == null ? 2 : currentcontract.STOREID;
                // TODO ТҮР ХААВ УДААГААД БАЙГАА
                //if (currentcontract != null)
                //{
                //    using (var servicecontroller = new ServiceUtilsController(_dbContext, null))
                //        servicecontroller.SendNotifcationData(Enums.SystemEnums.NotifcationType.Захиалга, 
                //            currentcontract.STOREID,
                //            currentcontract.BUSINESSID, 
                //            Convert.ToDecimal(recordid)).Wait();

                //    _log.LogInformation($"{logMethod} SENDNOTIFICATION : {DateTime.Now}", param.ContractNo);
                //}

                var msgbody = MessageResource.OrderMessage
                    .Replace("#shorturl", shorturl)
                    .Replace("#orderdate", param.OrderDate)
                    .Replace("#storename", "Emart");

                //  controller.GetNotifcationData();
                if (Convert.ToBoolean(ConfigData.GetCongifData(ConfigData.ConfigKey.HostDevelopmentMode)))
                {
                    _log.LogInformation($"{logMethod} SMS : {DateTime.Now}", param.ContractNo);
                    Logics.MailLogic.AddSMS(_dbContext, Enums.SystemEnums.MessageType.Order, msgbody, Messager.DEVELOPER_PHONE_NO, storeid);

                    _log.LogInformation($"{logMethod} EMAIL : {DateTime.Now}", param.ContractNo);
                    var resultmail = Emailer.Send(_dbContext, _log, Emailer.DEVELOPER_EMAIL, null,
                        Enums.SystemEnums.MessageType.Order, null, shorturl, param.MailBody, param.ContractNo,
                        null, null, "", storeid);

                    if (resultmail.Success)
                    {
                        //_dbContext.SaveChanges();
                    }
                }
                else
                {
                    _log.LogInformation($"{logMethod} SMS : {DateTime.Now}", param.ContractNo);
                    if (param.SMSMobile != null)
                    {
                        var smsmobile = param.SMSMobile.Split(',');
                        foreach (string phonenumber in smsmobile)
                        {
                            if (!UsefulHelpers.IsActualPhone(phonenumber))
                            {
                                _log.LogWarning($"{logMethod} : {phonenumber} IS NOT A VALID PHONENUMBER.", param.ContractNo);
                                continue;
                            }

                            Logics.MailLogic.AddSMS(_dbContext, Enums.SystemEnums.MessageType.Order, msgbody, phonenumber, storeid);
                        }
                    }

                    var title = $"{param.ContractNo} ({param.OrderNo})";
                    _log.LogInformation($"{logMethod} EMAIL : {title}", param.ContractNo);
                    if (param.SendMail != null)
                    {
                        var mails = param.SendMail.Split(',');
                        // ХЭРВЭЭ 2 ХҮН РҮҮ ИЛГЭЭХ ТОХИРГООТОЙ ЗӨВ ИМЭЙЛ ХАЯГУУД БАЙВАЛ
                        // CC ДЭЭР 2 ДАХ ХҮНИЙГ НЭМЖ ЯВУУЛНА
                        if (UsefulHelpers.IsValidTwoEmail(mails))
                        {
                            _log.LogInformation($"{logMethod} CC => {mails[0]}, {mails[1]}");

                            Emailer.Send(_dbContext, _log, mails[0].Trim(), null, 
                                Enums.SystemEnums.MessageType.Order,
                                null, shorturl, param.MailBody, title,
                                null, null, "", storeid, mails[1].Trim());
                        }
                        else
                        {
                            foreach (string mail in mails)
                            {
                                if (!UsefulHelpers.IsActualEmail(mail))
                                {
                                    _log.LogWarning($"{logMethod} : {mail} IS NOT A VALID EMAIL.", param.ContractNo);
                                    continue;
                                }

                                Emailer.Send(_dbContext, _log, mail.Trim(), null, 
                                    Enums.SystemEnums.MessageType.Order,
                                    null, shorturl, param.MailBody, title,
                                    null, null, "", storeid);
                            }
                        }
                    }
                    _log.LogInformation($"{logMethod} END {param.ContractNo} : {DateTime.Now}", param.ContractNo);
                }
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, $" {logMethod} ERROR {param.ContractNo} : {ex}");

                Logics.ManagementLogic.ExceptionLog(_dbContext, _log, HttpContext,
                    JsonConvert.SerializeObject(param), "Order", "reciveorder", ex);

                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("returnorder")]
        public async Task<ResponseClient> ReturnOrder([FromBody]RetOrder param)
        {
            var logMethod = "ORDERCONTROLLER.RETURNORDER";
            try
            {
                _log.LogInformation($"{logMethod} START {param.ContractNo} : {DateTime.Now}", param.ContractNo);

                if (!ModelState.IsValid)
                    return ReturnResponce.ModelIsNotValudResponce();
                
                Logics.OrderLogic.RemoveOld(_dbContext);
                decimal? recordid = 0;

                //var currentcontract = Logics.OrderLogic.GetContract(_dbContext, param.ContractNo);
                //var storeid = currentcontract == null ? 2 : currentcontract.STOREID;
                var storeid = 2;
                var oldorder = Logics.OrderLogic.GetReturn(_dbContext, param.OrderDate, param.OrderNo, param.ContractNo, param.RetIndex);

                if (oldorder == null)
                {
                    var order = new REQ_RETURN_ORDER()
                    {
                        RETORDERID = Logics.OrderLogic.GetNewId(_dbContext, "REQ_RETURN_ORDER"),
                        ORDERNO = param.OrderNo,
                        ORDERDATE = param.OrderDate,
                        CONTRACTNO = param.ContractNo,
                        CREATEDDATE = DateTime.Now,
                        FILEURL = param.FileUrl,
                        RETURNINDEX = param.RetIndex
                    };

                    Logics.OrderLogic.Insert(_dbContext, order);
                    recordid = order.RETORDERID;
                }
                else
                {
                    recordid = oldorder.RETORDERID;
                }

                _log.LogInformation($"{logMethod} SHORT URL : {recordid}", param.ContractNo);

                var controller = new ServiceUtilsController(_dbContext, null);
                var rsshortUrl = controller.GetUrlShortner(param.FileUrl, Enums.SystemEnums.MessageType.Return, recordid).Result;
                var shorturl = rsshortUrl.Success ? Convert.ToString(rsshortUrl.Value) : string.Empty;

                _log.LogInformation($"{logMethod} SHORT URL END : {shorturl}", param.ContractNo);

                // TODO ТҮР ХААВ
                //if (currentcontract != null)
                //{
                //    using (var servicecontroller = new ServiceUtilsController(_dbContext, null))
                //    {
                //        servicecontroller.SendNotifcationData(Enums.SystemEnums.NotifcationType.Буцаалт,
                //            currentcontract.STOREID, 
                //            currentcontract.BUSINESSID, 
                //            Convert.ToDecimal(recordid)).Wait();
                //    }
                //    _log.LogInformation($"{logMethod} SENDNOTIFICATION : {DateTime.Now}", param.ContractNo);
                //}

                var msgbody = param.RetIndex == "1" ? MessageResource.ReturnOrderMessage :
                            param.RetIndex == "2" ? MessageResource.ReturnOrderMessage2 :
                            param.RetIndex == "3" ? MessageResource.ReturnOrderMessage3 : "";

                msgbody = msgbody.Replace("#shorturl", shorturl).Replace("#orderdate", param.OrderDate).Replace("#storename", "Emart");

                if (Convert.ToBoolean(ConfigData.GetCongifData(ConfigData.ConfigKey.HostDevelopmentMode)))
                {
                    _log.LogInformation($"{logMethod} SMS :{DateTime.Now}", param.ContractNo);
                    //controller.SmsSend(msgbody, "90005881").Wait();
                    Logics.MailLogic.AddSMS(_dbContext, Enums.SystemEnums.MessageType.Return, msgbody, Messager.DEVELOPER_PHONE_NO, storeid);

                    _log.LogInformation($"{logMethod} EMAIL :{DateTime.Now}", param.ContractNo);
                    //var resuletmail = mailController.Post("khosbayar@datacare.mn", null, Enums.SystemEnums.SystemSendMailType.ЗахиалгаБуцаалт, 
                    // null, shorturl, param.MailBody, param.ContractNo).Result;
                    var resuletmail = Emailer.Send(_dbContext, _log, Emailer.DEVELOPER_EMAIL,
                        null, Enums.SystemEnums.MessageType.Return,
                        null, shorturl, param.MailBody, param.ContractNo, null, null, "", storeid);

                    if (resuletmail.Success)
                    {
                        //_dbContext.SaveChanges();
                        //return ReturnResponce.SaveSucessResponce();
                    }
                }
                else
                {
                    _log.LogInformation($"{logMethod} SMS : {DateTime.Now}", param.ContractNo);
                    if (param.SMSMobile != null)
                    {
                        var smsmobile = param.SMSMobile.Split(',');
                        foreach (string phonenumber in smsmobile)
                        {
                            if (!UsefulHelpers.IsActualPhone(phonenumber))
                            {
                                _log.LogWarning($"{logMethod} : {phonenumber} IS NOT A VALID PHONENUMBER.", param.ContractNo);
                                continue;
                            }
                            Logics.MailLogic.AddSMS(_dbContext, Enums.SystemEnums.MessageType.Return, msgbody, phonenumber, storeid);
                        }
                    }

                    _log.LogInformation($"{logMethod} EMAIL : {DateTime.Now}", param.ContractNo);
                    if (param.SendMail != null)
                    {
                        var mails = param.SendMail.Split(',');
                        // ХЭРВЭЭ 2 ХҮН РҮҮ ИЛГЭЭХ ТОХИРГООТОЙ ЗӨВ ИМЭЙЛ ХАЯГУУД БАЙВАЛ
                        // CC ДЭЭР 2 ДАХ ХҮНИЙГ НЭМЖ ЯВУУЛНА
                        if (UsefulHelpers.IsValidTwoEmail(mails))
                        {
                            _log.LogInformation($"{logMethod} CC => {mails[0]}, {mails[1]}");

                            Emailer.Send(_dbContext, _log, mails[0].Trim(), null,
                                Enums.SystemEnums.MessageType.Return,
                                null, shorturl, param.MailBody, param.ContractNo, null, 
                                null, "", storeid, mails[1].Trim());
                        }
                        else
                        {
                            foreach (string mail in mails)
                            {
                                if (!UsefulHelpers.IsActualEmail(mail))
                                {
                                    _log.LogWarning($"{logMethod} : {mail} IS NOT A VALID EMAIL.", param.ContractNo);
                                    continue;
                                }

                                Emailer.Send(_dbContext, _log, mail.Trim(), null,
                                    Enums.SystemEnums.MessageType.Return,
                                    null, shorturl, param.MailBody, param.ContractNo, 
                                    null, null, "", storeid);
                            }
                        }
                    }
                }

                _log.LogInformation($"{logMethod} END {param.ContractNo} : {DateTime.Now}", param.ContractNo);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, $" {logMethod} ERROR {param.ContractNo} => {UsefulHelpers.GetExceptionMessage(ex)}");

                Logics.ManagementLogic.ExceptionLog(_dbContext, _log, HttpContext,
                    JsonConvert.SerializeObject(param), "Order", "returnorder", ex);

                return ReturnResponce.GetExceptionResponce(ex);
            }
        }



        #region OrderHeader


        /// <summary>
        ///	#Захиалгын хураангуй
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-01-16
        /// </remarks>
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="contractno">Гэрээний №</param>
        /// <param name="sdate">Эхлэх огноо</param>
        /// <param name="edate">Дуусах огноо</param>
        /// <param name="skucd">Бар код</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("orderheader/{branchcode}/{contractno}/{sdate}/{edate}/{skucd}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetOrderHeader(string branchcode, string contractno, DateTime sdate, DateTime edate, string skucd)
        {
            var companyid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                return Logics.OrderLogic.GetOrderHeader(_dbContext, UsefulHelpers.STORE_ID, companyid, branchcode, contractno, sdate, edate, skucd);
            }
            catch (Exception ex)
            {
                var methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }



        /// <summary>
        ///	#Захиалгын дэлгэрэнгүй
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-01-16
        /// </remarks>
        /// <param name="branchcode">Салбарын код</param>
        /// <param name="orderdate">Захиалгын огноо</param>
        /// <param name="orderno">Захиалгын дугаар</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("orderdetail/{branchcode}/{orderdate}/{orderno}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetOrderDetail(string branchcode, string orderdate, string orderno)
        {
            try
            {
                return Logics.OrderLogic.GetOrderDetail(_dbContext, _log, branchcode, orderdate, orderno);
            }
            catch (Exception ex)
            {
                var methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        ///	#Захиалгын дэлгэрэнгүй
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2018-01-16
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("orderdetail")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetOrderDetail([FromBody] List<OrderDetailRequest> headers)
        {
            try
            {
                return Logics.OrderLogic.GetOrderDetail(_dbContext, _log, headers);
            }
            catch (Exception ex)
            {
                var methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        #endregion


        #region OrderSeen && Approve

        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("approveorder")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient PostApproveOrder([FromBody]REQ_ORDER param)
        {
            if (param == null)
                return ReturnResponce.NotFoundResponce();

            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            try
            {
                //var currentOrder = _dbContext.REQ_ORDER.FirstOrDefault(x => x.ORDERNO == param.ORDERNO 
                //    && x.ORDERDATE == param.ORDERDATE && x.CONTRACTNO == param.CONTRACTNO);
                var currentOrder = Logics.OrderLogic.Get(_dbContext, param.ORDERDATE, param.ORDERNO, param.CONTRACTNO);
                if (currentOrder != null)
                {
                    if (currentOrder.ISSEEN == 0)
                        return ReturnResponce.NotFoundResponce();

                    currentOrder.APPROVEDDATE = DateTime.Now;
                    currentOrder.APPROVEDUSER = userid;

                    Logics.BaseLogic.Update(_dbContext, currentOrder);
                    //_dbContext.Entry(currentOrder).State = System.Data.Entity.EntityState.Modified;
                    //_dbContext.SaveChanges();
                }
                else
                {
                    var order = new REQ_ORDER()
                    {
                        //ORDERID = _dbContext.GetTableID("REQ_ORDER"),
                        ORDERID = Logics.BaseLogic.GetNewId(_dbContext, "REQ_ORDER"),
                        ORDERNO = param.ORDERNO,
                        ORDERDATE = param.ORDERDATE,
                        CONTRACTNO = param.CONTRACTNO,
                        APPROVEDDATE = DateTime.Now,
                        APPROVEDUSER = userid
                    };
                    Logics.BaseLogic.Insert(_dbContext, order);
                    //_dbContext.REQ_ORDER.Add(order);
                    //_dbContext.SaveChanges();
                }
                return ReturnResponce.SuccessMessageResponce("Амжилттай!");
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, $"{UsefulHelpers.GetMethodName(MethodBase.GetCurrentMethod())} => {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("seenorder")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient PostSeenOrder([FromBody]REQ_ORDER param)
        {
            if (param == null)
                return ReturnResponce.NotFoundResponce();

            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            try
            {
                //var currentOrder = _dbContext.REQ_ORDER.FirstOrDefault(x => x.ORDERNO == param.ORDERNO
                //    && x.ORDERDATE == param.ORDERDATE && x.CONTRACTNO == param.CONTRACTNO);
                var currentOrder = Logics.OrderLogic.Get(_dbContext, param.ORDERDATE, param.ORDERNO, param.CONTRACTNO);

                if (currentOrder == null)
                {
                    var order = new REQ_ORDER()
                    {
                        ORDERID = Logics.BaseLogic.GetNewId(_dbContext, "REQ_ORDER"),
                        ORDERNO = param.ORDERNO,
                        ORDERDATE = param.ORDERDATE,
                        CONTRACTNO = param.CONTRACTNO,
                        SEENDATE = DateTime.Now,
                        SEENUSER = userid,
                        ISSEEN = 1
                    };
                    Logics.BaseLogic.Insert(_dbContext, order);
                    //_dbContext.REQ_ORDER.Add(order);
                    //_dbContext.SaveChanges();
                }
                else
                {
                    currentOrder.SEENDATE = DateTime.Now;
                    currentOrder.SEENUSER = userid;

                    Logics.BaseLogic.Update(_dbContext, currentOrder);
                    //_dbContext.Entry(currentOrder).State = System.Data.Entity.EntityState.Modified;
                    //_dbContext.SaveChanges();
                }
                return ReturnResponce.SuccessMessageResponce("Амжилттай!");
                //return ReturnResponce.FailedMessageResponce("Аль хэдийн харах төлөвт орсон байна.");
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, $"{UsefulHelpers.GetMethodName(System.Reflection.MethodBase.GetCurrentMethod())} => {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }



        #endregion
        
        #region ReturnOrder

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("returnorderheader/{branchcode}/{contractno}/{sdate}/{edate}/{skucd}")]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetReturnOrderHeader(string branchcode, string contractno, DateTime sdate, DateTime edate, string skucd = "")
        {
            var companyid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            try
            {
                return Logics.OrderLogic.GetReturnHeader(_dbContext, UsefulHelpers.STORE_ID, companyid, branchcode, contractno, sdate, edate, skucd);
            }
            catch (Exception ex)
            {
                var methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("returnorderdetail/{branchcode}/{orderdate}/{orderno}")]
        [ServiceFilter(typeof(LogFilter))]

        public async Task<ResponseClient> GetReturnDetail(string branchcode, string orderdate, string orderno)
        {
            try
            {
                return Logics.OrderLogic.GetReturnDetail(_dbContext, _log, branchcode, orderdate, orderno).Result;
            }
            catch (Exception ex)
            {
                var methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("returnorderdetail")]
        [ServiceFilter(typeof(LogFilter))]

        public async Task<ResponseClient> GetReturnDetail([FromBody] List<OrderDetailRequest> headers)
        {
            try
            {
                return Logics.OrderLogic.GetReturnDetail(_dbContext, _log, headers).Result;
            }
            catch (Exception ex)
            {
                var methodBase = MethodBase.GetCurrentMethod();
                _log.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("approvereturn")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient PostApproveReturn([FromBody]REQ_RETURN_ORDER param)
        {
            if (param == null)
                return ReturnResponce.NotFoundResponce();

            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            try
            {
                //var currentOrder = _dbContext.REQ_RETURN_ORDER.FirstOrDefault(x => x.ORDERNO == param.ORDERNO
                //    && x.ORDERDATE == param.ORDERDATE && x.CONTRACTNO == param.CONTRACTNO && x.RETURNINDEX == param.RETURNINDEX);
                var currentOrder = Logics.OrderLogic.GetReturn(_dbContext, param.ORDERDATE, param.ORDERNO, param.CONTRACTNO, param.RETURNINDEX);
                if (currentOrder != null)
                {
                    if (currentOrder.ISSEEN == 0)
                        return ReturnResponce.NotFoundResponce();

                    currentOrder.APPROVEDDATE = DateTime.Now;
                    currentOrder.APPROVEDUSER = userid;
                    currentOrder.RETURNINDEX = param.RETURNINDEX;

                    Logics.BaseLogic.Update(_dbContext, currentOrder);
                    //_dbContext.Entry(currentOrder).State = System.Data.Entity.EntityState.Modified;
                    //_dbContext.SaveChanges();
                }
                else
                {
                    var order = new REQ_RETURN_ORDER()
                    {
                        //RETORDERID = _dbContext.GetTableID("REQ_ORDER"),
                        RETORDERID = Logics.BaseLogic.GetNewId(_dbContext, "REQ_RETURN_ORDER"),
                        ORDERNO = param.ORDERNO,
                        ORDERDATE = param.ORDERDATE,
                        CONTRACTNO = param.CONTRACTNO,
                        APPROVEDDATE = DateTime.Now,
                        APPROVEDUSER = userid,
                        ISSEEN = 1,
                        SEENDATE = DateTime.Now
                    };

                    Logics.BaseLogic.Insert(_dbContext, order);
                    //_dbContext.REQ_RETURN_ORDER.Add(order);
                    //_dbContext.SaveChanges();
                }
                return ReturnResponce.SuccessMessageResponce("Амжилттай!");
            }
            catch (Exception ex)
            {
                _log.LogError(1, ex, $"{UsefulHelpers.GetMethodName(System.Reflection.MethodBase.GetCurrentMethod())} => {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("seenreturn")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient PostSeenReturn([FromBody]REQ_RETURN_ORDER param)
        {
            if (param == null)
                return ReturnResponce.ModelIsNotValudResponce();

            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            try
            {
                //var currentOrder = _dbContext.REQ_RETURN_ORDER.FirstOrDefault(x => x.ORDERNO == param.ORDERNO
                //    && x.ORDERDATE == param.ORDERDATE && x.CONTRACTNO == param.CONTRACTNO && x.RETURNINDEX == param.RETURNINDEX);
                var currentOrder = Logics.OrderLogic.GetReturn(_dbContext, param.ORDERDATE, param.ORDERNO, param.CONTRACTNO, param.RETURNINDEX);
                if (currentOrder == null)
                {
                    var order = new REQ_RETURN_ORDER()
                    {
                        RETORDERID = Logics.BaseLogic.GetNewId(_dbContext, "REQ_RETURN_ORDER"),
                        ORDERNO = param.ORDERNO,
                        ORDERDATE = param.ORDERDATE,
                        SEENUSER = userid,
                        CONTRACTNO = param.CONTRACTNO,
                        SEENDATE = DateTime.Now,
                        RETURNINDEX = param.RETURNINDEX,
                        ISSEEN = 1
                    };
                    Logics.BaseLogic.Insert(_dbContext, order);
                    //_dbContext.REQ_RETURN_ORDER.Add(order);
                    //_dbContext.SaveChanges();
                }
                else
                {
                    currentOrder.SEENUSER = userid;
                    Logics.BaseLogic.Update(_dbContext, userid);
                    //_dbContext.Entry(currentOrder).State = System.Data.Entity.EntityState.Modified;
                    //_dbContext.SaveChanges();
                }
                return ReturnResponce.SuccessMessageResponce("Амжилттай!");
            }
            catch (Exception ex)
            {
                _log.LogError($"OrderController.PostSeenReturn ERROR : {UsefulHelpers.GetExceptionMessage(ex)}");
                return ReturnResponce.GetExceptionResponce(ex);
            }
            //return ReturnResponce.FailedMessageResponce("Аль хэдийн харах төлөвт орсон байна.");
        }





        #endregion

    }


    public class FilePreviewModel
    {
        public string url { get; set; }
    }
}
