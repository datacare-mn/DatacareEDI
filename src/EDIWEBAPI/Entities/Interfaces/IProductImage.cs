using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.Interfaces
{
    public interface IProductImage
    {
        int ID { get; set; }
        int HEADERID { get; set; }
        int? LOGID { get; set; }
        int IMAGETYPE { get; set; }
        int VIEWORDER { get; set; }
        string URL { get; set; }
        int ENABLED { get; set; }
        int SEEN { get; set; }
        int? SEENBY { get; set; }
        DateTime? SEENDATE { get; set; }
        DateTime? EXPIREDATE { get; set; }
        DateTime? CREATEDDATE { get; set; }
    }
}
