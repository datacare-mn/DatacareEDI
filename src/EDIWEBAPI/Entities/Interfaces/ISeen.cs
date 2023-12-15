using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.Interfaces
{
    public interface ISeen
    {
        int? ISSEEN { get; set; }
        int? SEENUSER { get; set; }
        DateTime? SEENDATE { get; set; }
        int? APPROVEDUSER { get; set; }
        DateTime? APPROVEDDATE { get; set; }
    }
}
