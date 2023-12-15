using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.Entities;

namespace WebAPI.Controllers.SystemControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ColorsController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<ColorsController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public ColorsController(OracleDbContext context, ILogger<ColorsController> log)
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

            return ReturnResponce.ListReturnResponce(_dbContext.SYS_COLORS.ToList());
        }


        [HttpPost]
        [AllowAnonymous]
        public ResponseClient InsertColors([FromBody] List<SYS_COLORS> colors)
        {
            List<SYS_COLORS> ucolor = new List<SYS_COLORS>();
            foreach (SYS_COLORS vdata in colors)
            {
                vdata.COLORID = Convert.ToInt32(_dbContext.GetTableID("SYS_COLORS"));
                if (ModelState.IsValid)
                {
                    ucolor.Add(new SYS_COLORS()
                    {
                        COLORID = vdata.COLORID,
                        COLORNAME = vdata.COLORNAME
                    });

                }
                else
                {
                    return ReturnResponce.ModelIsNotValudResponce();
                }
            }

            _dbContext.SYS_COLORS.AddRange(ucolor);
            _dbContext.SaveChanges(HttpContext, 1);
            return ReturnResponce.SaveSucessResponce();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <param name="Color"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpPut]
        [AllowAnonymous]
        public ResponseClient UpdateColors([FromBody] SYS_COLORS color)
        {
            var currentColor = _dbContext.SYS_COLORS.FirstOrDefault(x => x.COLORID == color.COLORID);
            if (currentColor != null)
            {
                currentColor.COLORNAME = color.COLORNAME;
                _dbContext.Entry(currentColor).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges(HttpContext, 1);
                return ReturnResponce.SaveSucessResponce();
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

    }
}
