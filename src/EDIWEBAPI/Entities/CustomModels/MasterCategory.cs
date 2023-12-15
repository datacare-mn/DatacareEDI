using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class MasterCategory
    {
        public string DivCode { get; set; }
        public string DivsionName { get; set; }

        public List<MaterDepart> Departs { get; set; }


    }

    public class MaterDepart
    {
        public string DepName { get; set; }

        public string DepCode { get; set; }

        public List<MasterClass> Classes { get; set; }
    }

    public class MasterClass {
        public string ClassCode { get; set; }
        public string ClassName { get; set; }
    }

    public class MasterConfigCategory
    {
        public string DivCode { get; set; }
        public string DivsionName { get; set; }

        public List<MasterConfigtDepart> Departs { get; set; }


    }


    public class MasterConfigtDepart
    {
        public string DepName { get; set; }

        public string DepCode { get; set; }

        public List<MasterConfigClass> Classes { get; set; }
    }

    public class MasterConfigClass
    {
        public int? Classid { get; set; }
        public string ClassCode { get; set; }
        public string ClassName { get; set; }

        public string  UserName { get; set; }

        public DateTime? InsYmd { get; set; }

        public string InsEmp { get; set; }

    }


}
