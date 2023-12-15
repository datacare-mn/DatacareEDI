using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class LicenseUser
    {
        public int ID { get; set; }
        public int ORGID { get; set; }
        public int? ROLEID { get; set; }
        public int? OLDROLEID { get; set; }
        public DateTime? ROLECHANGEDATE { get; set; }
        public DateTime? AGREEMENTDATE { get; set; }
        public int? RESTORED { get; set; }
        public int? DELETED { get; set; }
        public DateTime? RESTOREDDATE { get; set; }
        public DateTime? DELETEDDATE { get; set; }
        public int? LOGCOUNT { get; set; }
        public int? REQUESTCOUNT { get; set; }
    }
}
