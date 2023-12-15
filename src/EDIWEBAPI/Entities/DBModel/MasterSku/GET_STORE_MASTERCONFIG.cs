using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.MasterSku
{
    public class GET_STORE_MASTERCONFIG
    {
        public int? ID { get; set; }

        public string CLASSCODE { get; set; }

        public string CLASSNAME { get; set; }

        public string FIRSTNAME { get; set; }

        public string INSEMP { get; set; }

        public DateTime? INSYMD { get; set; }

        public int? DEPID { get; set; }
    }
}
