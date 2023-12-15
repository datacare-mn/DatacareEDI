using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class StoreContract
    {
        public string contractcode { get; set; }
        public string contractname { get; set; }
        public int? contracttype { get; set; }
        public string departmentcode { get; set; }
    }
}
