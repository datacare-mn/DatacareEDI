using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class MenuDto
    {
        public int ID { get; set; }
        public int? PARENTID { get; set; }
        public string TITLE { get; set; }
        public string ROUTE { get; set; }
        public string ICON { get; set; }
        public int TYPE { get; set; }
        public int VIEWORDER { get; set; }
        public string SORTEDORDER { get; set; }
        public bool HASROLE { get; set; }
    }
}
