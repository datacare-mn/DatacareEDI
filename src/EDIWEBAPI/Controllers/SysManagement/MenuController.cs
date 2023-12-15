using EDIWEBAPI.Attributes;
using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/[controller]")]
    public class MenuController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MenuController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MenuController(OracleDbContext context, ILogger<MenuController> log)
        {
            _dbContext = context;
            _log = log;
        }



        #endregion

        #region Get
        /// <summary>
        /// API to get all values
        /// </summary>
        /// <remarks>
        /// This API will get all values
        /// </remarks>
        /// <returns>All values</returns>
        [HttpGet]
        [Route("gettest/{daynum}")]
        [AllowAnonymous]
        public async Task<ResponseClient> GetTestData(DayOfWeekMN daynum)
        {
            ResponseClient rs = new  ResponseClient();
            rs.Message = "Success";
            rs.Success = true;
            return rs;
        }

     



    /// <summary>
    /// Цор ганц меню буцаана
    /// </summary>
    /// <remarks>
    /// This API will get all values
    /// </remarks>
    /// <returns>All values</returns>
    [HttpGet]
        [Route("{id}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> Get(int id)
        {
            var currentmenu = _dbContext.SYSTEM_MENU.FirstOrDefault(x => x.MENUID == id);
            if (currentmenu != null)
            {
                return ReturnResponce.ListReturnResponce(currentmenu);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetAll()
        {
            var menus = _dbContext.SYSTEM_MENU.ToList();
            if (menus != null)
            {
                return ReturnResponce.ListReturnResponce(menus);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }


        /// <summary>
        /// Role -д хамааралтай цэсүүд дээр нэмж болох цэсийн жагсаалт  
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : Л.Идэр
        /// Үүсгэсэн огноо : 2017.11.30 
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 


        // role нэмж болох цэсийн жагсаалт API      

        [HttpGet]
        [Route("addrolemenus/{roleId}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> AddMenuRoles(int roleId)
        {
            if (!ModelState.IsValid) return ReturnResponce.ModelIsNotValudResponce();

            var result = _dbContext.GET_ADD_MENUS(roleId);
            if (result != null)
            {
                return ReturnResponce.ListReturnResponce(result);
            }

            return ReturnResponce.NotFoundResponce();
        }


        #endregion

            #region Post
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> Post([FromBody]List<SYSTEM_MENU> param)
        {
            List<SYSTEM_MENU> uData = new List<SYSTEM_MENU>();
            foreach (SYSTEM_MENU vdata in param)
            {
                vdata.MENUID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_MENU"));
                if (ModelState.IsValid)
                {
                    uData.Add(new SYSTEM_MENU()
                    {
                        MENUID = vdata.MENUID,
                        PARENTID = vdata.PARENTID,
                        MENUNAME = vdata.MENUNAME,
                        MENUCAPTION = vdata.MENUCAPTION,
                        MENUURL = vdata.MENUURL,
                        MENUICON = vdata.MENUICON,
                    });
                }
                else
                    return ReturnResponce.ModelIsNotValudResponce();
            }
            _dbContext.SYSTEM_MENU.AddRange(uData);
            _dbContext.SaveChanges();
            return ReturnResponce.SaveSucessResponce();
        }

        #endregion

        #region Put

        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> Put([FromBody]SYSTEM_MENU param)
        {
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.SYSTEM_MENU.FirstOrDefault(x => x.MENUID == param.MENUID);
                if (currentdata != null)
                {
                    currentdata.MENUID = param.MENUID;
                    currentdata.PARENTID = param.PARENTID;
                    currentdata.MENUNAME = param.MENUNAME;
                    currentdata.MENUCAPTION = param.MENUCAPTION;
                    currentdata.MENUURL = param.MENUURL;
                    currentdata.MENUICON = param.MENUICON;
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
