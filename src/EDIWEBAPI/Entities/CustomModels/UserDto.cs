using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class UserDto
    {
        public int ID { get; set; }
        public int ORGID { get; set; }
        public string USERMAIL { get; set; }
        public string USERPIC { get; set; }
        public string LASTNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string PHONE { get; set; }
        public int? ROLEID { get; set; }
        public int? COOPERATION { get; set; }
        public DateTime REGDATE { get; set; }
        public string ROLENAME { get; set; }
        public List<int> departments { get; set; }
        public List<int> roles { get; set; }
    }
}
