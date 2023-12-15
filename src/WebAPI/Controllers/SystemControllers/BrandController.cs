using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.Entities;

namespace WebAPI.Controllers.SystemControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BrandController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<BrandController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public BrandController(OracleDbContext context, ILogger<BrandController> log)
        {
            _dbContext = context;
            _log = log;
        }

        /// <summary>
        /// Өнгө
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ResponseClient Get()
        {
            return ReturnResponce.ListReturnResponce(_dbContext.SYS_BRAND.ToList());
        }

        /// <summary>
        /// Функцын тайлбар
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 


        [HttpGet]
        [Route("ModelInfo")]
        [AllowAnonymous]

        public ResponseClient GetModel()
        {
       var models =    _dbContext.SYS_BRAND.GetType().GetProperties(BindingFlags.DeclaredOnly |
                                           BindingFlags.Public |
                                           BindingFlags.Instance);
            return ReturnResponce.ListReturnResponce(models);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseClient InsertBrands([FromBody] List<SYS_BRAND> brands)
        {
            List<SYS_BRAND> ubrand = new List<SYS_BRAND>();
            foreach (SYS_BRAND vdata in brands)
            {
                vdata.BRANDID = Convert.ToInt32(_dbContext.GetTableID("SYS_BRAND"));
                if (ModelState.IsValid)
                {
                 //   int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnumTypes.UserProperties.CompanyId));
                    ubrand.Add(new SYS_BRAND() { BRANDID = vdata.BRANDID, BRANDNAME = vdata.BRANDNAME, BRANDIMAGE = vdata.BRANDIMAGE, COMID = vdata.COMID});

                }
                else
                {
                    return ReturnResponce.ModelIsNotValudResponce();
                }
            }

            _dbContext.SYS_BRAND.AddRange(ubrand);
            _dbContext.SaveChanges(HttpContext);
            return ReturnResponce.SaveSucessResponce();
        }


        [HttpPut]
        [AllowAnonymous]
        public ResponseClient UpdateBrand([FromBody] SYS_BRAND brand)
        {
            try
            {
                var currentBrand = _dbContext.SYS_BRAND.FirstOrDefault(x => x.BRANDID == brand.BRANDID);
                if (currentBrand != null)
                {
                    currentBrand.BRANDNAME = brand.BRANDNAME;
                    currentBrand.BRANDIMAGE = brand.BRANDIMAGE;
                }

                _dbContext.Entry(_dbContext.SYS_BRAND).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


    }
}
