using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    public class LicenseData
    {
       public SYSTEM_LICENSE license { get; set; }

       public List<int> funcionid { get; set; } 


    }
}
