using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.IMSModels
{
    public class PostAttributeModel
    {
        public string attrnm { get; set; }
        public string description { get; set; }
        public int measureid { get; set; }
        public int isenable { get; set; }
        public List<AttrDetail> attrvalues { get; set; }
    }
    public class AttrDetail
    {
        public int? id { get; set; }
        public string attrvalue { get; set; }
    }
}
