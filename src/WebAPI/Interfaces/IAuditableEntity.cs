using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime? INSYMD { get; set; }

        int? INSEMP { get; set; }

        DateTime? UPDYMD { get; set; }

        int? UPDEMP { get; set; }
    }
}
