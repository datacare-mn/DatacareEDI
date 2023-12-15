using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.CustomModels
{
    public class BaseEntity
    {
        public DateTime? INSYMD { get; set; }
        public int? INSEMP { get; set; }

        public DateTime? UPDYMD { get; set; }
        public int? UPDEMP { get; set; }
    }
}
