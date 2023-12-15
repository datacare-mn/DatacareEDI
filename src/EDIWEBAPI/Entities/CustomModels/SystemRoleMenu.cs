using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class SystemRoleMenu
    {
        public int ID { get; set; }

        public int MODULETYPE { get; set; }

        public string ROLENAME { get; set; }

        public List<SYSTEM_MENU> MENUS { get; set; }
    }
}
