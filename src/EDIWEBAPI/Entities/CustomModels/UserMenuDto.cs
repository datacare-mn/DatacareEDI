using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class UserMenuDto
    {
        public int ID { get; set; }
        public string TITLE { get; set; }
        public string ROUTE { get; set; }
        public string ICON { get; set; }
        public List<UserSubMenuDto> ITEMS { get; set; }
    }
}
