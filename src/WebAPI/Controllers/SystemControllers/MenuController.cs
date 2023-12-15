using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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
    [Route("api/v{version:apiVersion}/menus")]
    public class MenuController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MenuController> _log;

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

        [HttpGet("{id}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetMenu(int userid)
        {
            ResponseClient response = new ResponseClient();
           var currentCompany = _dbContext.BIZ_COMPANY.Where(x => x.COMID == userid).SingleOrDefault();
            int curComID = currentCompany.COMTYPE;
            var MenuList = from menu in _dbContext.SYS_MENU
                           where menu.MODULEID == curComID
                           orderby menu.ORDERINDEX select menu;
            List<SYS_MENU> menu1 = MenuList.ToList();           
            if (MenuList != null)
            {
                response.Success = true;
                return ReturnResponce.ListReturnResponce(menu1);
            }
            else
            {
               return ReturnResponce.NotFoundResponce();
            }
        }

    }
}
