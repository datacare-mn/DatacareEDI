using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class ProductRequestLog
    {
        public int USERID { get; set; }
        public string USERNAME { get; set; }
        public string IMAGEURL { get; set; }
        public DateTime? ACTIONDATE { get; set; }
        public Enums.SystemEnums.RequestLogType Type { get; set; }
        public string Note { get; set; }
        public List<ImageDto> Images { get; set; }
    }
}
