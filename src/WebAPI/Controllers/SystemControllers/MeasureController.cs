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
    public class MeasureController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MeasureController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MeasureController(OracleDbContext context, ILogger<MeasureController> log)
        {
            _dbContext = context;
            _log = log;
        }


        /// <summary>
        /// Системийн барааны үйлдвэрлэгдсэн улсын лавлах
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        //[Authorize(Policy = "EdiApiUser")]
        public ResponseClient Get()
        {
            return ReturnResponce.ListReturnResponce(_dbContext.SYS_MEASURE.ToList());
        }

        /// <summary>
        /// Улс гэсэн json
        /// </summary>
        /// <param name="origins">Лист</param>
        /// <returns></returns>
        [HttpPost]
       // [Authorize(Policy = "EdiApiUser")]
        public ResponseClient InsertOrigins([FromBody] List<SYS_MEASURE> origins)
        {
            List<SYS_MEASURE> uorigin = new List<SYS_MEASURE>();
            foreach (SYS_MEASURE vdata in origins)
            {

                vdata.MEASUREID = Convert.ToInt32(_dbContext.GetTableID("SYS_MEASURE"));
                if (ModelState.IsValid)
                {
                    uorigin.Add(new SYS_MEASURE() { MEASUREID = vdata.MEASUREID, MEASURENAME = vdata.MEASURENAME});
                }
                else
                {
                    return ReturnResponce.ModelIsNotValudResponce();
                }
            }

            _dbContext.SYS_MEASURE.AddRange(uorigin);
            _dbContext.SaveChanges(HttpContext);
            return ReturnResponce.SaveSucessResponce();


        }

        [HttpPut]
        [AllowAnonymous]
        public ResponseClient UpdateOrigin([FromBody]SYS_MEASURE origin)
        {
            if (ModelState.IsValid)
            {
                var currentOrigin = _dbContext.SYS_MEASURE.FirstOrDefault(x => x.MEASUREID == origin.MEASUREID);
                if (currentOrigin != null)
                {
                    currentOrigin.MEASURENAME = origin.MEASURENAME;
                    _dbContext.Entry(_dbContext.SYS_ORIGIN).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges(HttpContext);
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                {
                    return ReturnResponce.NotFoundResponce();
                }
            }
            else
            {
                return ReturnResponce.ModelIsNotValudResponce();
            }

        }
    }
}
