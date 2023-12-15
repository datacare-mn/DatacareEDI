using System;
using System.Collections.Generic;
using System.Linq;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Entities.Interfaces
{
    public interface IProductRequest
    {
        int ID { get; set; }
        int STOREID { get; set; }
        int DEPARTMENTID { get; set; }
        int REQUESTID { get; set; }
        int ENABLED { get; set; }
        int STATUS { get; set; }
        int SEEN { get; set; }
        int ATTACHMENT { get; set; }
        string NOTE { get; set; }
        DateTime REQUESTDATE { get; set; }
        int? RECEIVEDBY { get; set; }
        DateTime? RECEIVEDDATE { get; set; }
        int? CONFIRMEDBY { get; set; }
        DateTime? CONFIRMEDDATE { get; set; }
        DateTime? EXPIREDATE { get; set; }
    }
}
