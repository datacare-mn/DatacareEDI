using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    public class PackageList
    {
        public SYSTEM_LICENSE_PACKAGE package { get; set; }

        public List<SYSTEM_LICENSE_FUNCTION> functions  { get; set; }
    }
}
