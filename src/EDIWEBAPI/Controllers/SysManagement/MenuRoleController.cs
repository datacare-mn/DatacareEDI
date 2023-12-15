using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.FilterViews;
using static EDIWEBAPI.Enums.SystemEnums;
using EDIWEBAPI.Attributes;
using EDIWEBAPI.Entities.LicenseConfig;

namespace EDIWEBAPI.Controllers.SysManagement
{
    [Route("api/[controller]")]
    public class MenuRoleController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MenuRoleController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MenuRoleController(OracleDbContext context, ILoggerFactory log)
        {
            //ILoggerFactory loggerFactory
            _dbContext = context;
            _log = log.CreateLogger<MenuRoleController>();
        }
        #endregion

        #region Get


        [HttpGet]
        [Route("storeroles")]
        [Authorize]
        public ResponseClient GetStoreRoles()
        {
            var menus = (from m in _dbContext.SYSTEM_NEW_MENU
                         where m.ENABLED == 1 && m.SHOP == 1 && m.HASROLE == 1
                         orderby m.SORTEDORDER
                         select new
                         {
                             m.ID,
                             m.TITLE,
                             m.TYPE
                         }).ToList();

            return ReturnResponce.ListReturnResponce(menus);
        }

        [HttpGet]
        [Route("menus")]
        [Authorize]
        public ResponseClient GetMenus()
        {
            var userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            var orgtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.OrgType));
            try
            {
                var allMenus = new List<SYSTEM_NEW_MENU>();
                var menus = new List<MenuDto>();
                var showParent = true;

                if ((ORGTYPE)orgtype == ORGTYPE.Бизнес)
                {
                    allMenus = (from m in _dbContext.SYSTEM_NEW_MENU
                                where m.BUSINESS == 1 && m.ENABLED == 1
                                orderby m.SORTEDORDER
                                select m).ToList();

                    var month = DateTime.Today.ToString("MM");

                    var licenseMenus = (from l in _dbContext.SYSTEM_LICENSE_PRICE
                                        where l.ENABLED == 1 && l.TYPE == 2
                                        select new { l.ID, l.MENUID }).ToList();

                    var licenseUsers = (from u in _dbContext.SYSTEM_LICENSE_USER
                                        where u.USERID == userid && u.MONTH == month && u.YEAR == DateTime.Today.Year && u.ENABLED == 1
                                        group u by u.LICENSEID into g
                                        select new { LICENSEID = g.Key }).ToList();

                    var reports = (from m in licenseMenus
                                   join u in licenseUsers on m.ID equals u.LICENSEID into mu
                                   from lj in mu.DefaultIfEmpty()
                                   select new { m.MENUID, LICENSEID = lj == null ? 0 : lj.LICENSEID }).ToList();

                    menus = (from m in allMenus
                             join l in reports on m.ID equals l.MENUID into lj
                             from j in lj.DefaultIfEmpty()
                                 //where j == null || j.MENUID == null || j.LICENSEID != 0
                             orderby m.SORTEDORDER
                             select new MenuDto()
                             {
                                 ID = m.ID,
                                 PARENTID = m.PARENTID,
                                 VIEWORDER = m.VIEWORDER,
                                 TITLE = m.TITLE,
                                 ROUTE = m.ROUTE,
                                 ICON = m.ICON,
                                 HASROLE = j == null || j.MENUID == null || j.LICENSEID != 0
                             }).ToList();
                }
                else if ((ORGTYPE)orgtype == ORGTYPE.Дэлгүүр)
                {
                    showParent = false;
                    allMenus = (from m in _dbContext.SYSTEM_NEW_MENU
                                where m.SHOP == 1 && m.ENABLED == 1 
                                orderby m.SORTEDORDER
                                select m).ToList();

                    var roles = (from r in _dbContext.SYSTEM_USER_ROLES
                                 where r.USERID == userid
                                 select new { r.ROLEID }).ToList();

                    menus = (from m in allMenus
                             join l in roles on m.ID equals l.ROLEID //into lj
                             //from j in lj.DefaultIfEmpty()
                             orderby m.SORTEDORDER
                             select new MenuDto()
                             {
                                 ID = m.ID,
                                 PARENTID = m.PARENTID,
                                 VIEWORDER = m.VIEWORDER,
                                 TITLE = m.TITLE,
                                 ROUTE = m.ROUTE,
                                 ICON = m.ICON,
                                 HASROLE = true, //m.PARENTID == null || j != null
                             }).ToList();

                    // HASROLE ТАВИГДААГҮЙ ЭСВЭЛ ЭРХ НЬ ТАВИГДСАН PARENT - УУД
                    allMenus = (from m in allMenus
                                join r in roles on m.ID equals r.ROLEID into lj
                                from l in lj.DefaultIfEmpty()
                                where m.PARENTID == null && (m.HASROLE != 1 || l != null)
                                orderby m.SORTEDORDER
                                select m).ToList();
                }
                else if ((ORGTYPE)orgtype == ORGTYPE.Менежмент)
                {
                    allMenus = (from m in _dbContext.SYSTEM_NEW_MENU
                                where m.SYS == 1 && m.ENABLED == 1
                                orderby m.SORTEDORDER
                                select m).ToList();

                    //menus = allMenus.Select(a => a).ToList();
                }

                var parentMenus = (from m in allMenus //menus
                                   where m.PARENTID == null
                                   orderby m.VIEWORDER
                                   select new UserMenuDto()
                                   {
                                       ID = m.ID,
                                       TITLE = m.TITLE,
                                       ROUTE = m.ROUTE,
                                       ICON = m.ICON,
                                       ITEMS = new List<UserSubMenuDto>()
                                   }).ToList();

                var parents = new List<UserMenuDto>();
                foreach (var parent in parentMenus)
                {
                    var items = from i in menus
                                where i.PARENTID == parent.ID
                                select new UserSubMenuDto()
                                {
                                    ID = i.ID,
                                    TITLE = i.TITLE,
                                    ROUTE = i.ROUTE,
                                    HASROLE = i.HASROLE
                                };

                    if (items.Any()) 
                        parent.ITEMS = items.ToList();

                    if (!showParent && !parent.ITEMS.Any() && string.IsNullOrEmpty(parent.ROUTE)) continue;
                    parents.Add(parent);
                }

                return ReturnResponce.ListReturnResponce(parents);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        ///	#
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        [HttpGet]
        [Route("{id}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Get(int id)
        {
            var currentuser = _dbContext.SYSTEM_MENU_ROLE.FirstOrDefault(x => x.ID == id);
            if (currentuser != null)
            {
                return ReturnResponce.ListReturnResponce(currentuser);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        [HttpGet]
        [Route("orgrole/{id}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient GetOrgRole(int id)
        {
            
            var currentuser = _dbContext.SYSTEM_ORGANIZATION_ROLES.FirstOrDefault(x => x.ID == id);
            if (currentuser != null)
            {
                return ReturnResponce.ListReturnResponce(currentuser);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }


        /// <summary>
        ///	#Системд нэвтэрсэн хэрэглэгчийн 
        ///	Role-д хамаарах цэсийг буцаагдаг функц
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]

        public ResponseClient GetUserRoles()
        {
            int userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var currentUser = _dbContext.SYSTEM_USERS.FirstOrDefault(x => x.ID == userid);
            if (currentUser.ISADMIN == -1)
            {
                var currentcompany = _dbContext.SYSTEM_ORGANIZATION.FirstOrDefault(x => x.ID == comid);
                if (currentcompany.ORGTYPE == ORGTYPE.Дэлгүүр)
                {
                    var parentmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 1 && menu.PARENTID == null) select new UserMenu { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                    List<UserMenu> mainmenu = parentmenus.Where(x => x.PARENTID == null).Distinct().OrderBy(c => c.ORDER).ToList<UserMenu>();
                    foreach (var parent in mainmenu)
                    {
                        var childmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 1 && menu.PARENTID != null) select new UserDetailMenu {  MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL,  MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                        List<UserDetailMenu> data = childmenus.Where(x => x.PARENTID == parent.MENUID).Distinct().OrderBy(c => c.ORDER) .ToList<UserDetailMenu>();
                        if (data.Count > 0)
                        {
                            parent.DETAILMENU = data;
                        }

                    }
                    return ReturnResponce.ListReturnResponce(mainmenu);
                }
                if (currentcompany.ORGTYPE == ORGTYPE.Бизнес)
                {
                    var parentmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 2 && menu.PARENTID == null) select new UserMenu { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                    List<UserMenu> mainmenu = parentmenus.Where(x => x.PARENTID == null).Distinct().OrderBy(c => c.ORDER).ToList<UserMenu>();
                    foreach (var parent in mainmenu)
                    {
                        var childmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 2 && menu.PARENTID != null) select new UserDetailMenu { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                        List<UserDetailMenu> data = childmenus.Where(x => x.PARENTID == parent.MENUID).Distinct().OrderBy(c => c.ORDER).ToList<UserDetailMenu>();
                        if (data.Count > 0)
                        {
                            parent.DETAILMENU = data;
                        }

                    }
                    return ReturnResponce.ListReturnResponce(mainmenu);
                }
                if (currentcompany.ORGTYPE == ORGTYPE.Менежмент)
                {
                    var parentmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 3 && menu.PARENTID == null) select new UserMenu { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                    List<UserMenu> mainmenu = parentmenus.Where(x => x.PARENTID == null).Distinct().OrderBy(c => c.ORDER).ToList<UserMenu>();
                    foreach (var parent in mainmenu)
                    {
                        var childmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 3 && menu.PARENTID != null) select new UserDetailMenu { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                        List<UserDetailMenu> data = childmenus.Where(x => x.PARENTID == parent.MENUID).Distinct().OrderBy(c => c.ORDER).ToList<UserDetailMenu>();
                        if (data.Count > 0)
                        {
                            parent.DETAILMENU = data;
                        }
                    }
                    return ReturnResponce.ListReturnResponce(mainmenu);
                }
                else
                {
                    return ReturnResponce.NotFoundResponce();
                }

            }
            else
            {
                if (currentUser.ROLEID != null)
                {
                    var organizationRoles = _dbContext.SYSTEM_ORGMENUROLES.Where(x => x.ROLEID == currentUser.ROLEID);
                    if (organizationRoles != null)
                    {
                        var menuIDs = organizationRoles.Select(x => x.MENUID).ToArray();
                        if (menuIDs != null)
                        {
                            var query = from item in _dbContext.SYSTEM_MENU
                                        where menuIDs.Contains(item.MENUID)
                                        select item.MENUID;

                            var parentmenus = from menu in _dbContext.SYSTEM_MENU where query.Contains(menu.MENUID) select new UserMenu { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                            List<UserMenu> mainmenu = parentmenus.Where(x => x.PARENTID == null).Distinct().OrderBy(c => c.ORDER).ToList<UserMenu>();
                            foreach (var parent in mainmenu)
                            {
                                var childmenus = from menu in _dbContext.SYSTEM_MENU where (menu.PARENTID == parent.MENUID && query.AsEnumerable().Contains(menu.MENUID)) select new UserDetailMenu { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                                List<UserDetailMenu> data = childmenus.Where(x => x.PARENTID == parent.MENUID).Distinct().OrderBy(c => c.ORDER).ToList<UserDetailMenu>();
                                if (data.Count > 0)
                                {
                                    parent.DETAILMENU = data;
                                }
                            }
                            return ReturnResponce.ListReturnResponce(mainmenu);
                        }
                    }
                    return ReturnResponce.NotFoundResponce();
                }
                else
                {
                  return  ReturnResponce.FailedMessageResponce("Хэрэглэгч дээр эрхийн тохиргоо хийгдээгүй байна.");
                }
            }
        }

        [HttpGet]
        [Authorize]
        [Route("userroledata")]

        public ResponseClient GetUserRolesData()
        {
            int userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var currentUser = _dbContext.SYSTEM_USERS.FirstOrDefault(x => x.ID == userid);
            if (currentUser.ISADMIN == -1)
            {
                var currentcompany = _dbContext.SYSTEM_ORGANIZATION.FirstOrDefault(x => x.ID == comid);
                if (currentcompany.ORGTYPE == ORGTYPE.Дэлгүүр)
                {
                    var parentmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 1 && menu.PARENTID == null) select new UserMenuData { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                    List<UserMenuData> mainmenu = parentmenus.Where(x => x.PARENTID == null).Distinct().OrderBy(c => c.ORDER).ToList<UserMenuData>();
                    foreach (var parent in mainmenu)
                    {
                        var childmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 1 && menu.PARENTID != null) select new UserDetailMenuData { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                        List<UserDetailMenuData> data = childmenus.Where(x => x.PARENTID == parent.MENUID).Distinct().OrderBy(c => c.ORDER).ToList<UserDetailMenuData>();
                        if (data.Count > 0)
                        {
                            parent.DETAILMENU = data;
                        }

                    }
                    return ReturnResponce.ListReturnResponce(mainmenu);
                }
                if (currentcompany.ORGTYPE == ORGTYPE.Бизнес)
                {
                    var parentmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 2 && menu.PARENTID == null) select new UserMenuData { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                    List<UserMenuData> mainmenu = parentmenus.Where(x => x.PARENTID == null).Distinct().OrderBy(c => c.ORDER).ToList<UserMenuData>();
                    foreach (var parent in mainmenu)
                    {
                        var childmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 2 && menu.PARENTID != null) select new UserDetailMenuData { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                        List<UserDetailMenuData> data = childmenus.Where(x => x.PARENTID == parent.MENUID).Distinct().OrderBy(c => c.ORDER).ToList<UserDetailMenuData>();
                        if (data.Count > 0)
                        {
                            parent.DETAILMENU = data;
                        }

                    }
                    return ReturnResponce.ListReturnResponce(mainmenu);
                }
                if (currentcompany.ORGTYPE == ORGTYPE.Менежмент)
                {
                    var parentmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 3 && menu.PARENTID == null) select new UserMenuData { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                    List<UserMenuData> mainmenu = parentmenus.Where(x => x.PARENTID == null).Distinct().OrderBy(c => c.ORDER).ToList<UserMenuData>();
                    foreach (var parent in mainmenu)
                    {
                        var childmenus = from menu in _dbContext.SYSTEM_MENU where (menu.MODULETYPE == 3 && menu.PARENTID != null) select new UserDetailMenuData { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                        List<UserDetailMenuData> data = childmenus.Where(x => x.PARENTID == parent.MENUID).Distinct().OrderBy(c => c.ORDER).ToList<UserDetailMenuData>();
                        if (data.Count > 0)
                        {
                            parent.DETAILMENU = data;
                        }
                    }
                    return ReturnResponce.ListReturnResponce(mainmenu);
                }
                else
                {
                    return ReturnResponce.NotFoundResponce();
                }

            }
            else
            {
                if (currentUser.ROLEID != null)
                {
                    var organizationRoles = _dbContext.SYSTEM_ORGMENUROLES.Where(x => x.ROLEID == currentUser.ROLEID);
                    if (organizationRoles != null)
                    {
                        var menuIDs = organizationRoles.Select(x => x.MENUID).ToArray();
                        if (menuIDs != null)
                        {
                            var query = from item in _dbContext.SYSTEM_MENU
                                        where menuIDs.Contains(item.MENUID)
                                        select item.MENUID;

                            var parentmenus = from menu in _dbContext.SYSTEM_MENU where query.Contains(menu.MENUID) select new UserMenuData { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                            List<UserMenuData> mainmenu = parentmenus.Where(x => x.PARENTID == null).Distinct().OrderBy(c => c.ORDER).ToList<UserMenuData>();

                            foreach (var parent in mainmenu)
                            {
                                var childmenus = from menu in _dbContext.SYSTEM_MENU where (menu.PARENTID == parent.MENUID && query.AsEnumerable().Contains(menu.MENUID)) select new UserDetailMenuData { MENUID = menu.MENUID, MENUNAME = menu.MENUNAME, MENUURL = menu.MENUURL, PARENTID = menu.PARENTID, MENUCAPTION = menu.MENUCAPTION, MENUICON = menu.MENUICON, ORDER = menu.ORDER };
                                List<UserDetailMenuData> data = childmenus.Where(x => x.PARENTID == parent.MENUID).Distinct().OrderBy(c => c.ORDER).ToList<UserDetailMenuData>();
                                if (data.Count > 0)
                                {
                                    parent.DETAILMENU = data;
                                }
                            }
                            return ReturnResponce.ListReturnResponce(mainmenu);
                        }
                    }
                    return ReturnResponce.NotFoundResponce();
                }
                else
                {
                    return ReturnResponce.FailedMessageResponce("Хэрэглэгч дээр эрхийн тохиргоо хийгдээгүй байна.");
                }
            }
        }




        #region OrganizationRoles

        /// <summary>
        ///	#Хандах эрхийн мэдээллийг филтертэй авчрана
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : Л.Идэр
        /// Үүсгэсэн огноо : 2018-1-26
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>



        [HttpPost]
        [Route("getrolelist")]
        [Authorize]
        public ResponseClient GetRoles([FromBody]MenuRoleFilterView filter) {
            if (!ModelState.IsValid) return ReturnResponce.ModelIsNotValudResponce();
            var roleList = _dbContext.GET_ROLE_LIST(filter);
            if (roleList != null){
                return ReturnResponce.ListReturnResponce(roleList);
            }

            return ReturnResponce.NotFoundResponce();

        }


        /// <summary>
        ///	#Нэвтэрсэн хэрэглэгчийн байгууллагын Role
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-15
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>



        [HttpGet]
        [Route("getorganizationroles")]
        [Authorize]
        public ResponseClient GetOrganizationRoles()
        {

            int orgtype =Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));

            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));

            OrganizationDefaultRolesAdd(orgtype, comid);

            var systemRoles = _dbContext.SYSTEM_ROLES.ToList();
            List<SystemRoleMenu> menuList = new List<SystemRoleMenu>();
            var currentRolelist = _dbContext.SYSTEM_ORGANIZATION_ROLES.Where(x => x.ORGID == comid);
            if (currentRolelist != null)
            {
                foreach (SYSTEM_ORGANIZATION_ROLES role in currentRolelist)
                {
                    SystemRoleMenu menu = new SystemRoleMenu();
                    menu.ID = role.ID;
                    menu.ROLENAME = role.ROLENAME;

                    var organizationRoles = _dbContext.SYSTEM_ORGMENUROLES.Where(x => x.ROLEID == role.ID);
                    var menuIDs = organizationRoles.Select(x => x.MENUID).ToArray();

                    var query = from item in _dbContext.SYSTEM_MENU
                                where menuIDs.Contains(item.MENUID)
                                select item.MENUID;
                    var menus = _dbContext.SYSTEM_MENU.Where(x => query.Contains(x.MENUID)).ToList();
                    menu.MENUS = menus;
                    menuList.Add(menu);
                }
                return ReturnResponce.ListReturnResponce(menuList);
            }
            else
                return ReturnResponce.NotFoundResponce();
        }





        private bool OrganizationDefaultRolesAdd(int orgtype, int comid)
        {
                var currentsystemroles = _dbContext.SYSTEM_ROLES.Where(x => x.MODULETYPE == orgtype);
            if (currentsystemroles != null)
            {
                foreach (SYSTEM_ROLES roles in currentsystemroles)
                {
                    SYSTEM_ORGANIZATION_ROLES OrgRole = new SYSTEM_ORGANIZATION_ROLES();
                    int roleID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ORGANIZATION_ROLES"));
                    OrgRole.ID = roleID;
                    OrgRole.ISSYSTEM = 1;
                    OrgRole.ORGID = comid;
                    OrgRole.ROLENAME = roles.ROLENAME;
                    _dbContext.SYSTEM_ORGANIZATION_ROLES.Add(OrgRole);

                    var currentSystemMenuRoles = _dbContext.SYSTEM_MENU_ROLE.Where(x => x.ROLEID == roles.ID);
                    if (currentsystemroles != null)
                    {
                        foreach (SYSTEM_MENU_ROLE systemmenurole in currentSystemMenuRoles)
                        {
                            SYSTEM_ORGMENUROLES OrgMenuRole = new SYSTEM_ORGMENUROLES();
                            OrgMenuRole.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ORGMENUROLES"));
                            OrgMenuRole.MENUID = systemmenurole.MENUID;
                            OrgMenuRole.ROLEID = roleID;
                            _dbContext.SYSTEM_ORGMENUROLES.Add(OrgMenuRole);
                        }
                    }
                }
                _dbContext.SaveChanges();
                return true;

            }
            else
                return false;
        }


        [HttpPost]
        [Authorize]
        public ResponseClient Post([FromBody]SYSTEM_ORGANIZATION_ROLES param)
        {
            param.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ORGANIZATION_ROLES"));
            param.ORGID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            param.ISSYSTEM = 0;
            if (ModelState.IsValid)
            {
                _dbContext.SYSTEM_ORGANIZATION_ROLES.Add(param);
                _dbContext.SaveChanges();
                return ReturnResponce.SaveSucessResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }


        [HttpPut]
        [Authorize]
        [Route("getorganizationroles")]
        public ResponseClient Put([FromBody]SYSTEM_ORGANIZATION_ROLES param)
        {
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.SYSTEM_ORGANIZATION_ROLES.FirstOrDefault(x => x.ID == param.ID);
                if (currentdata != null)
                {
                    currentdata.ROLENAME = param.ROLENAME;
                    currentdata.ISSYSTEM = 0;
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





        [HttpPost]
        [Authorize]
        [Route("organizationmenuroles")]
        public ResponseClient Post([FromBody]SYSTEM_ORGMENUROLES param)
        {
            param.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ORGMENUROLES"));
          
            if (ModelState.IsValid)
            {
                _dbContext.SYSTEM_ORGMENUROLES.Add(param);
                _dbContext.SaveChanges();
                return ReturnResponce.SaveSucessResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }

        [HttpPost]
        [Authorize]
        [Route("organizationmenuroleslist")]
        public ResponseClient Post([FromBody]List<SYSTEM_ORGMENUROLES> param)
        {
            List<SYSTEM_ORGMENUROLES> uData = new List<SYSTEM_ORGMENUROLES>();
            foreach (SYSTEM_ORGMENUROLES vdata in param)
            {
                vdata.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ORGMENUROLES"));
                if (ModelState.IsValid)
                {
                    uData.Add(new SYSTEM_ORGMENUROLES()
                    {
                        ID = vdata.ID,
                        MENUID = vdata.MENUID,
                        ROLEID = vdata.ROLEID,
                    });
                }
                else
                    return ReturnResponce.ModelIsNotValudResponce();
            }
            _dbContext.SYSTEM_ORGMENUROLES.AddRange(uData);
            _dbContext.SaveChanges();
            return ReturnResponce.SaveSucessResponce();
        }

        [HttpPut]
        [Authorize]
        [Route("organizationmenuroles")]
        public ResponseClient Put([FromBody]SYSTEM_ORGMENUROLES param)
        {
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.SYSTEM_ORGMENUROLES.FirstOrDefault(x => x.ID == param.ID);
                if (currentdata != null)
                {
                    currentdata.MENUID = param.MENUID;
                    currentdata.ROLEID = param.ROLEID;
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



        [HttpGet]
        [Route("getmenudata")]
        public ResponseClient GetMenuData()
        {
            return  ReturnResponce.ListReturnResponce(_dbContext.SYSTEM_MENU.ToList());
        }

        #endregion

        #region Системийн үндсэн Role


        [HttpGet]
        [Authorize]
        [Route("systemroles")]
        public ResponseClient Get()
        {

            
            try
            {
                var systemRoles = _dbContext.SYSTEM_ROLES.ToList();
                List<SystemRoleMenu> menuList = new List<SystemRoleMenu>();
                foreach (SYSTEM_ROLES role in systemRoles)
                {
                    SystemRoleMenu menu = new SystemRoleMenu();
                    menu.ID = role.ID;
                    menu.MODULETYPE = role.MODULETYPE;
                    menu.ROLENAME = role.ROLENAME;
                    var organizationRoles = _dbContext.SYSTEM_MENU_ROLE.Where(x => x.ROLEID == role.ID);
                    var menuIDs = organizationRoles.Select(x => x.MENUID).ToArray();
                    var query = from item in _dbContext.SYSTEM_MENU
                                where menuIDs.Contains(item.MENUID)
                                select item.MENUID;
                    var menus = _dbContext.SYSTEM_MENU.Where(x => query.Contains(x.MENUID)).ToList();
                    menu.MENUS = menus;
                    menuList.Add(menu);
                }
                return ReturnResponce.ListReturnResponce(menuList);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("systemroles")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Post([FromBody]SYSTEM_ROLES param)
        {
            param.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ROLES"));
            if (ModelState.IsValid)
            {
                _dbContext.SYSTEM_ROLES.Add(param);
                _dbContext.SaveChanges();
                return ReturnResponce.SaveSucessResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }


        /// <summary>
        ///	#SYSTEM_MENU_ROLE ХҮСНЭГТИЙН POST
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpPut]
        [Authorize]
        [Route("systemroles")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Put([FromBody]SYSTEM_ROLES param)
        {
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.SYSTEM_ROLES.FirstOrDefault(x => x.ID == param.ID);
                if (currentdata != null)
                {
                    currentdata.ID = param.ID;
                    currentdata.ROLENAME = param.ROLENAME;
                    currentdata.MODULETYPE = param.MODULETYPE;
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
        [HttpPost]
        [Authorize]
        [Route("systemmenurole")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Post([FromBody]SYSTEM_MENU_ROLE param)
        {
            param.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_MENU_ROLE"));
            if (ModelState.IsValid)
            {
                _dbContext.SYSTEM_MENU_ROLE.Add(param);
                _dbContext.SaveChanges();
                return ReturnResponce.SaveSucessResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }


        [HttpDelete]
        [Authorize]
        [Route("systemmenurole/{id}")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Delete(int id)
        {
            var currentmenu = _dbContext.SYSTEM_MENU_ROLE.FirstOrDefault(x=> x.ID == id);
            if (currentmenu != null)
            {
                _dbContext.Entry(currentmenu).State = System.Data.Entity.EntityState.Deleted;
                _dbContext.SaveChanges();
                return ReturnResponce.SuccessMessageResponce("Мэдээллийг устгалаа!");
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }



        /// <summary>
        ///	#SYSTEM_MENU_ROLE ХҮСНЭГТИЙН PUT 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-20
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>



        [HttpPut]
        [Authorize]
        [Route("systemmenurole")]
        [ServiceFilter(typeof(LogFilter))]
        public ResponseClient Put([FromBody]SYSTEM_MENU_ROLE param)
        {
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.SYSTEM_MENU_ROLE.FirstOrDefault(x => x.ID == param.ID);
                if (currentdata != null)
                {
                    currentdata.MENUID = param.MENUID;
                    currentdata.ROLEID = param.ROLEID;
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

        [HttpDelete]
        [Authorize]
        [Route("deleteorganizationmenurole/{roleid}")]
        [ServiceFilter(typeof(LogFilter))]

        public ResponseClient DeleteOrganizationRole(int roleid)
        {
            if (ModelState.IsValid)
            {
                var orgtype = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.OrgType));
                var comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, UserProperties.CompanyId));
                if (orgtype == 3)
                {

                    var currenorganizationrole = _dbContext.SYSTEM_ORGANIZATION_ROLES.FirstOrDefault(x => x.ID == roleid);
                    if (currenorganizationrole != null)
                    {
                        var empcount = _dbContext.SYSTEM_USERS.Where(x => x.ROLEID == roleid).Count();
                        if (empcount == 0)
                        {
                            var currentmenus = _dbContext.SYSTEM_ORGMENUROLES.Where(x => x.ROLEID == roleid);
                            if (currentmenus.Count() > 0)
                            {
                                foreach (SYSTEM_ORGMENUROLES menu in currentmenus)
                                {
                                    _dbContext.Entry(menu).State = System.Data.Entity.EntityState.Deleted;
                                }         
                            }
                            _dbContext.Entry(currenorganizationrole).State = System.Data.Entity.EntityState.Deleted;
                            _dbContext.SaveChanges();
                            return ReturnResponce.SuccessMessageResponce("Мэдээллийг устгалаа!");
                        }
                        else
                        {
                            return ReturnResponce.FailedMessageResponce("Энэ мэдээлэл дээр хэрэглэгч бүртгэлттэй байна.!");
                        }
                    }
                    else
                        return ReturnResponce.NotFoundResponce();
                }
                else
                {
                    var currenorganizationrole = _dbContext.SYSTEM_ORGANIZATION_ROLES.FirstOrDefault(x => x.ID == roleid);
                    if (currenorganizationrole != null)
                    {
                        if (currenorganizationrole.ISSYSTEM == 0)
                        {
                            var empcount = _dbContext.SYSTEM_USERS.Where(x => x.ROLEID == roleid).Count();
                            if (empcount == 0)
                            {
                                var currentmenus = _dbContext.SYSTEM_ORGMENUROLES.Where(x => x.ROLEID == roleid);
                                if (currentmenus.Count() > 0)
                                {
                                        foreach (SYSTEM_ORGMENUROLES menu in currentmenus)
                                        {
                                            _dbContext.Entry(menu).State = System.Data.Entity.EntityState.Deleted;
                                        }
                                }
                                _dbContext.Entry(currenorganizationrole).State = System.Data.Entity.EntityState.Deleted;
                                _dbContext.SaveChanges();
                                return ReturnResponce.SuccessMessageResponce("Мэдээллийг устгалаа!");
                            }
                            else
                            {
                                return ReturnResponce.FailedMessageResponce("Энэ мэдээлэл дээр хэрэглэгч бүртгэлттэй байна.!");
                            }
                        }
                        else
                        {
                            return ReturnResponce.FailedMessageResponce("Энэ мэдээллийг устгах эрх хүрэхгүй байна.!");
                        }
                    }
                    else
                        return ReturnResponce.NotFoundResponce();
                }

            }
            return ReturnResponce.ModelIsNotValudResponce();
        }





        /// <summary>
        /// Role жагсаалтыг байгууллагаар нь нэгтгэж харуулах api 
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


        // role нь үүсгэгдсэн байгууллагуудын жагсаалтыг role уудтай нь компанийн нэрээр шүүж авчрах API      
        [HttpPost]
        [Route("organizationswithroles")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetOrganizationsWithRoles([FromBody]OganizationWithRolesFilterView filter)
        {
            if (!ModelState.IsValid) return ReturnResponce.ModelIsNotValudResponce();
            var organizationList = _dbContext.GET_ORGANIZAION_WITH_ROLES(filter);
            if (organizationList != null)
            {
                return ReturnResponce.ListReturnResponce(organizationList);
            }

            return ReturnResponce.NotFoundResponce();

        }

        /// <summary>
        /// Role -н Menu жагсаалтыг  харуулах api 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : Л.Идэр
        /// Үүсгэсэн огноо : 2017.12.05 
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        // Тухайн role дээрх хандах цэсийн жагсаалт roleId-р шүүж авчрах API      
        [HttpGet]
        [Route("menulistbyroletype/{type}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetMenusByOrganizationType(int type)
        {
            if (!ModelState.IsValid) return ReturnResponce.ModelIsNotValudResponce();
            var menus = _dbContext.GET_MENUS_BY_ORGANIZATION_TYPE(type);
            if (menus != null)
            {
                return ReturnResponce.ListReturnResponce(menus);
            }

            return ReturnResponce.NotFoundResponce();

        }

        /// <summary>
        /// Байгууллагын Menu жагсаалтыг  харуулах api 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : Л.Идэр
        /// Үүсгэсэн огноо : 2017.1.30 
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        // Тухайн role дээрх хандах цэсийн жагсаалт roleId-р шүүж авчрах API      
        [HttpGet]
        [Route("menulistbyorganization/{id}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> GetMenusByRoleId(long id)
        {
            if (!ModelState.IsValid) return ReturnResponce.ModelIsNotValudResponce();
            var menus = _dbContext.GET_MENUS_BY_ROLE(id);
            if (menus != null)
            {
                return ReturnResponce.ListReturnResponce(menus);
            }

            return ReturnResponce.NotFoundResponce();

        }

        /// <summary>
        /// Хандах эрхийн нэрийг хадгалах api 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : Л.Идэр
        /// Үүсгэсэн огноо : 2017.12.07 
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        // Хандах эрхийн нэрийг хадгалах api     
        [HttpPost]
        [Route("saverole")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> SaveRole([FromBody]SYSTEM_ORGANIZATION_ROLES role)
        {
            if (!ModelState.IsValid) return ReturnResponce.ModelIsNotValudResponce();
            if (role.ID == 0) {
                role.ID = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ORGANIZATION_ROLES"));
            }
            var result = _dbContext.SAVE_ROLE(role);
            if (result>0)
            {
                return ReturnResponce.ListReturnResponce(result);
            }

            return ReturnResponce.NotFoundResponce();

        }

        /// <summary>
        /// Хандах эрхийн цэсний жагсаалтаас цэс устгах api 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : Л.Идэр
        /// Үүсгэсэн огноо : 2017.12.07 
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        // Хандах эрхийн цэс хасах api     
        [HttpGet]
        [Route("removerolemenu/{roleId}/{menuId}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> RemoveMenuRole(int roleId, int menuId)
        {
            if (!ModelState.IsValid) return ReturnResponce.ModelIsNotValudResponce();
            var result = _dbContext.REMOVE_ROLE(roleId,menuId);
            if (result > 0){
                return ReturnResponce.ListReturnResponce(result);
            }
            return ReturnResponce.NotFoundResponce();
        }

        /// <summary>
        /// Хандах эрхийн цэсүүдийг нэмэх api 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : Л.Идэр
        /// Үүсгэсэн огноо : 2017.12.07 
        /// Зассан огноо : 
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 
        // Хандах эрхийн цэсийн жагсаалт нэмэх api     
        [HttpPost]
        [Route("addrolemenus/{roleId}")]
        [Authorize]
        [ServiceFilter(typeof(LogFilter))]
        public async Task<ResponseClient> AddRoleMenus([FromBody]List<int> menuIds, int roleId)
        {
            if (!ModelState.IsValid) return ReturnResponce.ModelIsNotValudResponce();
            var result = 0;

            foreach (int menuId in menuIds) {
                   var id = Convert.ToInt32(_dbContext.GetTableID("SYSTEM_ORGMENUROLES"));
                   result = _dbContext.ADD_ROLE_MENU(id, menuId,roleId);
                if (result == 0) { break; } 
            }

            if (result > 0){
                return ReturnResponce.ListReturnResponce(result);
            }
            return ReturnResponce.NotFoundResponce();
        }

        #endregion
    }
}
