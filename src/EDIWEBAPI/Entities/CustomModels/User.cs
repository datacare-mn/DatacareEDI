using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Enums;
using static EDIWEBAPI.Enums.SystemEnums;
using EDIWEBAPI.Entities.DBModel.SystemManagement;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class User
    {
        public long? id { get; set; }
        public string userPassword { get; set; }

        public string usermail { get; set; }

        public string userpic { get; set; }

        public string lastname { get; set; }
        public string firstname { get; set; }

        public  ENABLED enabled { get; set; }
        public int? orgid { get; set; }
            
        public int? isadmin { get; set; }
        public string phone { get; set; }
        public DateTime? regdate { get; set; }

        public int? roleId { get; set; }
        public string rolename { get; set; }

        public IEnumerable<MST_CONTRACT> Contracts { get; set; }

    }
}
