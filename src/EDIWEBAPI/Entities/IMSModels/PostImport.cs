using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.IMSModels
{
    public class PostImport
    {
        public string code { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public List<ImportDetail> values { get; set; }
    }
    public class ImportDetail
    {
        public string attribute { get; set; }
        public string value { get; set; }
    }
}
