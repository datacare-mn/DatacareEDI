using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.DBModel.MasterData;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.MasterData
{
    [Route("api/[controller]")]
    public class SystemRolesController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<SystemRolesController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public SystemRolesController(OracleDbContext context, ILogger<SystemRolesController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        #region Get
        /// <summary>
        /// Өнгө
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public ResponseClient Get(int id)
        {
            var currentroles = _dbContext.SYSTEM_ROLES.FirstOrDefault(x => x.ID == id);
            if (currentroles != null)
            {
                return ReturnResponce.ListReturnResponce(currentroles);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ResponseClient GetAll()
        {
            var systemroles = _dbContext.SYSTEM_ROLES.ToList();
            if (systemroles != null)
            {
                return ReturnResponce.ListReturnResponce(systemroles);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }
        #endregion

        #region Post

        [HttpPost]
        public ResponseClient Post([FromBody]List<SYSTEM_ROLES> param)
        {
            List<SYSTEM_ROLES> uData = new List<SYSTEM_ROLES>();
            foreach (SYSTEM_ROLES vdata in param)
            {
                vdata.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ROLES"));
                if (ModelState.IsValid)
                {
                    uData.Add(new SYSTEM_ROLES()
                    {
                        ID = vdata.ID,
                        ROLENAME = vdata.ROLENAME,
                    });
                }
                else
                    return ReturnResponce.ModelIsNotValudResponce();
            }
            _dbContext.SYSTEM_ROLES.AddRange(uData);
            _dbContext.SaveChanges();
            return ReturnResponce.SaveSucessResponce();
        }

        #endregion

        #region Put

        [HttpPut]
        public ResponseClient Put([FromBody]SYSTEM_ROLES param)
        {
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.SYSTEM_ROLES.FirstOrDefault(x => x.ID == param.ID);
                if (currentdata != null)
                {
                    currentdata.ID = param.ID;
                    currentdata.ROLENAME = param.ROLENAME;
                    _dbContext.Entry(currentdata).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges();
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                    return ReturnResponce.NotFoundResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }


        #endregion

    }
}
