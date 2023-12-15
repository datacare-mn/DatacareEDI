using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class FeedbackDto
    {
        public int ID { get; set; }
        public int ORGID { get; set; }
        public int TYPEID { get; set; }
        public string SUBJECT { get; set; }
        public string NOTE { get; set; }
    }
}
