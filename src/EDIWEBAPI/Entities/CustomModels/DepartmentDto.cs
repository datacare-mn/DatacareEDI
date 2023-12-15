using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class DepartmentDto
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string NOTE { get; set; }
        public int VIEWORDER { get; set; }
        public int USERQTY { get; set; }
        public string MAPPINGIDS { get; set; }
        public List<string> USERS { get; set; }
    }
}
