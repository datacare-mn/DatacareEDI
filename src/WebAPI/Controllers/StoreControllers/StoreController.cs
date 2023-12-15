using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.CustomModels;
using WebAPI.Models.Entities;
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Controllers.StoreControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StoreController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<StoreController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public StoreController(OracleDbContext context, ILogger<StoreController> log)
        {
            _dbContext = context;
            _log = log;
        }



        [HttpGet]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetStoreInfo()
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentStore = _dbContext.BIZ_COMPANY.Where(x => x.COMID == comid && x.COMTYPE == 1).SingleOrDefault();
            if (currentStore != null)
            {
                return ReturnResponce.ListReturnResponce(currentStore);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }
        /// <summary>
        /// Дэлгүүрийн ажлын цагийн мэдээлэл татах
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetWorkTime")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetWorkTime()
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentTime = _dbContext.BIZ_COMPANY.Where(x => x.COMID == comid).Select(x => new { x.WSTIME, x.WETIME }).ToList();
            if (currentTime != null)
            {
                WorkTime wtime = new WorkTime();
                wtime.WorkStartTime = currentTime.Select(x => x.WSTIME).SingleOrDefault();
                wtime.WorkEndTime = currentTime.Select(x => x.WETIME).SingleOrDefault();
                return ReturnResponce.ListReturnResponce(wtime);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }


        /// <summary>
        /// Дэлгүүрийн ажлын цагийн мэдээлэл татах
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTeaTime")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetTeaTime()
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentTime = _dbContext.BIZ_COMPANY.Where(x => x.COMID == comid).Select(x => new { x.TSTIME, x.TETIME }).ToList();
            if (currentTime != null)
            {
                TeaTime wtime = new TeaTime();
                wtime.TeaTimeStart = currentTime.Select(x => x.TSTIME).SingleOrDefault();
                wtime.TeaTimeEnd = currentTime.Select(x => x.TETIME).SingleOrDefault();
                return ReturnResponce.ListReturnResponce(wtime);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }



        /// <summary>
        /// Ажлын цагийн тохиргоо
        /// </summary>
        /// <param name="wstime">Ажлын эхлэх цаг</param>
        /// <param name="wetime">Ажлын дуусах цаг</param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateWorkTime")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient UpdateWorkTime([FromBody]WorkTime wtime)
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentCopmapny = _dbContext.BIZ_COMPANY.FirstOrDefault(x => x.COMID == comid && x.COMTYPE == 1);
            if (currentCopmapny != null)
            {
                currentCopmapny.WSTIME = wtime.WorkStartTime;
                currentCopmapny.WETIME = wtime.WorkEndTime;
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        /// <summary>
        /// Уулзалт үргэлжлэх хугацаа
        /// </summary>
        /// <param name="meetminute"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateMeetTime/{meetminute}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient UpdateMeetTime(int meetminute)
        {
            ResponseClient response = new ResponseClient();
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));

            var currentCopmapny = _dbContext.BIZ_COMPANY.FirstOrDefault(x => x.COMID == comid && x.COMTYPE == 1);
            if (currentCopmapny != null)
            {
                currentCopmapny.MEEMINUTE = meetminute;
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }









        /// <summary>
        /// Цайны цагийн тохиргоо
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateTeaTime")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient UpdateTeaTime([FromBody]TeaTime teatime)
        {
            ResponseClient response = new ResponseClient();
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentCopmapny = _dbContext.BIZ_COMPANY.FirstOrDefault(x => x.COMID == comid && x.COMTYPE == 1);
            if (currentCopmapny != null)
            {
                currentCopmapny.TSTIME = teatime.TeaTimeStart;
                currentCopmapny.TETIME = teatime.TeaTimeEnd;
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }



        /// <summary>
        ///Дэлгүүрийн мэдээлэл өөрчлөх
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> UpdateStore([FromBody]BIZ_COMPANY store)
        {
            var currentCompany = _dbContext.BIZ_COMPANY.SingleOrDefault(x => x.COMID == store.COMID && x.COMTYPE == 1);
            if (currentCompany != null)
            {
                currentCompany.ADDRESS = store.ADDRESS;
                currentCompany.CEONAME = store.CEONAME;
                currentCompany.COMREG = store.COMREG;
                currentCompany.FAX = store.FAX;
                currentCompany.LOCATION = store.LOCATION;
                currentCompany.LOGO = store.LOGO;
                currentCompany.MAIL = store.MAIL;
                currentCompany.NAME = store.NAME;
                currentCompany.PHONE = store.PHONE;
                currentCompany.SLOGAN = store.SLOGAN;
                currentCompany.WEB = store.WEB;
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }


        [HttpPost]
        [Authorize(Policy = "StoreApiUser")]
        public async Task<ResponseClient> Insert([FromBody]BIZ_COMPANY store)
        {
            store.COMID =Convert.ToInt32(_dbContext.GetTableID("BIZ_COMPANY"));
            try
            {
                if (ModelState.IsValid)
                {
                    _dbContext.BIZ_COMPANY.Add(store);
                    _dbContext.SaveChanges(HttpContext);
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                {
                    return ReturnResponce.ModelIsNotValudResponce();
                }

            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpGet]
        [Route("AllBusiness")]
        [Authorize(Policy = ("StoreApiUser"))]
        public  ResponseClient GetAllBusiness()
        {
            var currentCompanyID = UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId);
            var allcompanyID = _dbContext.BIZ_STORE_BUSINESS.Where(x => x.STOREID == 3).Select(x => new { x.BUSINESSID }).ToList();
            int[] array = UsefulHelpers.toListArray(allcompanyID);

            var companys = from company in _dbContext.BIZ_COMPANY
                           where array.Contains(Convert.ToInt32(company.COMID))
                           select company;
            return ReturnResponce.ListReturnResponce(companys.ToList());

        }








    }

}
