using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.Entities;
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Controllers.BusinessControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CompanyController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<CompanyController> _log;

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

        /// <summary>
        /// Компанийн мэдээллийг харах
        /// </summary>
        /// <param name="id">№</param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient GetCompanyInfo()
        {
            var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var currentCompany = _dbContext.BIZ_COMPANY.Where(x => x.COMID == comid).SingleOrDefault();
            if (currentCompany != null)
            {
                return  ReturnResponce.ListReturnResponce(currentCompany);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        
        /// <summary>
        /// Компанийн мэдээллийг засах
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>

        [HttpPut]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient UpdateCompany([FromBody]BIZ_COMPANY company)
        {
            var currentCompany = _dbContext.BIZ_COMPANY.SingleOrDefault(x => x.COMID == company.COMID && x.COMTYPE == 0);
            if (currentCompany != null)
            {
                currentCompany.ADDRESS = company.ADDRESS;
                currentCompany.CEONAME = company.CEONAME;
                currentCompany.COMREG = company.COMREG;
                currentCompany.FAX = company.FAX;
                currentCompany.LOCATION = company.LOCATION;
                currentCompany.LOGO = company.LOGO;
                currentCompany.MAIL = company.MAIL;
                currentCompany.NAME = company.NAME;
                currentCompany.PHONE = company.PHONE;
                currentCompany.SLOGAN = company.SLOGAN;
                currentCompany.WEB = company.WEB;
                return ReturnResponce.SaveSucessResponce();
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }

        }
        ///// <summary>
        ///// Шинэ компанийн мэдээлэл оруулах
        ///// </summary>
        ///// <param name="company"></param>
        ///// <param name="Image"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public ResponseClient InsertCompany([FromBody] BIZ_COMPANY company, IFormFile Image)
        //{

        //     ResponseClient response = new ResponseClient();
        //    var file = Image;
        //    var uploads = Path.Combine("uploads\\logos");
        //    if (file.Length > 0)
        //    {
        //        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //        System.Console.WriteLine(fileName);
        //        using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
        //        {
        //            file.CopyToAsync(fileStream);
        //         company.LOGO = Path.Combine(uploads, file.FileName);
        //        }
                
        //        var imageUrl = Path.Combine(uploads +  file);

        //        _dbContext.BIZ_COMPANY.Add(company);
        //        _dbContext.SaveChanges(HttpContext);
        //    }
        //    response.Message = ReturnMessageHelper.SUCCESSSAVE;
        //    response.Success = true;

        //    return response;
        //}

            /// <summary>
            /// 
            /// </summary>
            /// <param name="company"></param>
            /// <returns></returns>

        [HttpPost]
        //[Authorize(Policy = "BizApiUser")]
        [AllowAnonymous]
        public async Task<ResponseClient> Insert([FromBody]BIZ_COMPANY company)
        {
            ResponseClient response = new ResponseClient();
            company.COMID = Convert.ToInt32(_dbContext.GetTableID("BIZ_COMPANY"));
            try
            {
                if (ModelState.IsValid)
                {
                    _dbContext.BIZ_COMPANY.Add(company);
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




        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("AllStores")]
        [Authorize(Policy = ("BizApiUser"))]
        public ResponseClient GetAllStores()
        {
            int currentCompanyID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
            var allcompanyID = _dbContext.BIZ_STORE_BUSINESS.Where(x => x.BUSINESSID == 3).Select( x => new {x.BUSINESSID}).ToList();
            int[] array = UsefulHelpers.toListArray(allcompanyID);

            var companys = from company in _dbContext.BIZ_COMPANY
                            where array.Contains(company.COMID)
                            select company;
            return ReturnResponce.ListReturnResponce(companys.ToList());
        }
    }


}
