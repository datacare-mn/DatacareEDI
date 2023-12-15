using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class GET_LOGIN_USER_MENU_SELECT
    {
        public int MENUID { get; set; }

        public int? PARENTID { get; set; }

        public string MENUNAME { get; set; }

        public string TITLE { get; set; }

        public string ROUTE { get; set; }

        public string ICON { get; set; }

        public int MODULETYPE { get; set; }

        public int PERM { get; set; }

        [NotMapped]
        public List<GET_LOGIN_USER_MENU_SELECT> items { get; set; }

    }

    public class LoginUserMenu
    {

    }
}
