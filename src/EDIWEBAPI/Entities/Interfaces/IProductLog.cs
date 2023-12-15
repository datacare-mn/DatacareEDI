using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Entities.Interfaces
{
    public interface IProductLog
    {
        int ID { get; set; }
        int HEADERID { get; set; }
        int USERID { get; set; }
        string NOTE { get; set; }
        int SEEN { get; set; }
        int STATUS { get; set; }
        ORGTYPE ORGTYPE { get; set; }
        RequestLogType TYPE { get; set; }
        DateTime? ACTIONDATE { get; set; }
    }
}
