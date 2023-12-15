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
    public class SysOriginController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<SysOriginController> _log;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public SysOriginController(OracleDbContext context, ILogger<SysOriginController> log)
        {
            _dbContext = context;
            
            _log = log;
        }

        /// <summary>
        /// Системийн барааны үйлдвэрлэгдсэн улсын лавлах
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [AllowAnonymous]
        public ResponseClient Get()
        {
            return ReturnResponce.ListReturnResponce(_dbContext.SYS_ORIGIN.ToList());
        }

        /// <summary>
        /// Улс гэсэн json
        /// </summary>
        /// <param name="origins">Лист</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "EdiApiUser")]
        public ResponseClient InsertOrigins([FromBody] List<SYS_ORIGIN> origins)
        {
            List<SYS_ORIGIN> uorigin = new List<SYS_ORIGIN>();
            foreach (SYS_ORIGIN vdata in origins)
            {

                vdata.ORIGINID = Convert.ToInt32(_dbContext.GetTableID("SYS_ORIGIN"));
                if (ModelState.IsValid)
                {
                    uorigin.Add(new SYS_ORIGIN() {  ORIGINID = vdata.ORIGINID, ORIGINNAME = vdata.ORIGINNAME, SHORTNAME = vdata.SHORTNAME });
                }
                else
                {
                    return ReturnResponce.ModelIsNotValudResponce();
                }
            }

            _dbContext.SYS_ORIGIN.AddRange(uorigin);
            _dbContext.SaveChanges(HttpContext);
            return ReturnResponce.SaveSucessResponce();


        }
        /// <summary>
        /// Улс объект
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = "EdiApiUser")]
        public ResponseClient UpdateOrigin([FromBody]SYS_ORIGIN origin)
        {
            if (ModelState.IsValid)
            {
                var currentOrigin = _dbContext.SYS_ORIGIN.FirstOrDefault(x => x.ORIGINID == origin.ORIGINID);
                if (currentOrigin != null)
                {
                    currentOrigin.ORIGINNAME = origin.ORIGINNAME;
                    currentOrigin.SHORTNAME = origin.SHORTNAME;
                    _dbContext.Entry(currentOrigin).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges(HttpContext);
                 return   ReturnResponce.SaveSucessResponce();
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
