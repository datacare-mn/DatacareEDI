using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class UserSubMenuDto
    {
        public int ID { get; set; }
        public string TITLE { get; set; }
        public string ROUTE { get; set; }
        public bool HASROLE { get; set; }
    }
}
