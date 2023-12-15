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
    public class KeepUnitTypeController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<KeepUnitTypeController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public KeepUnitTypeController(OracleDbContext context, ILogger<KeepUnitTypeController> log)
        {
            _dbContext = context;
            _log = log;
        }
        
        /// <summary>
        /// Системийн хадгалах хугацааны лавлах
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [AllowAnonymous]
        public ResponseClient Get()
        {

            return ReturnResponce.ListReturnResponce(_dbContext.SYS_KEEP_UNITTYPE.ToList());
        }


        [HttpPost]
        [AllowAnonymous]
        public ResponseClient InsertKeepUnitType([FromBody] List<SYS_KEEP_UNITTYPE> unitypes)
        {
            List<SYS_KEEP_UNITTYPE> utype = new List<SYS_KEEP_UNITTYPE>();
            ResponseClient response = new ResponseClient();
            foreach (SYS_KEEP_UNITTYPE vdata in unitypes)
            {
                vdata.TYPEID = Convert.ToInt32(_dbContext.GetTableID("SYS_KEEP_UNITTYPE"));
                if (ModelState.IsValid)
                {
                    utype.Add(new SYS_KEEP_UNITTYPE() { TYPEID = vdata.TYPEID, TYPENAME = vdata.TYPENAME });

                }
                else
                {
                    return ReturnResponce.ModelIsNotValudResponce();
                }
            }

            _dbContext.SYS_KEEP_UNITTYPE.AddRange(utype);
            _dbContext.SaveChanges(HttpContext);
            return ReturnResponce.SaveSucessResponce();


        }




    }
}
