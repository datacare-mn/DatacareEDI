using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_CONFIGDATA
    {
        [Key]
        public string KEYDATA { get; set; }
        public string KEYVALUE { get; set; }
    }
}
