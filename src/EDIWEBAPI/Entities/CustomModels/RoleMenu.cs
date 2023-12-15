using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class RoleMenu
    {
        public long menuId { get; set; }
        public long? parentId { get; set; }
        public string menuName { get; set; }
        public string menuCaption { get; set; }
        public string menuUrl { get; set; }
        public int order { get; set; }
        public IList<RoleMenu> childMenus { get; set; }

        public RoleMenu()
        {

        }

    }
}
