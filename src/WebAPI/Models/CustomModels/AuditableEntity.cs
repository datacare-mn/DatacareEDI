using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Interfaces;

namespace WebAPI.Models.CustomModels
{
    public  class AuditableEntity<T>: IAuditableEntity
    {
        [ScaffoldColumn(false)]
        public DateTime? INSYMD { get; set; }

        [ScaffoldColumn(false)]
        public int? INSEMP { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? UPDYMD { get; set; }

        [ScaffoldColumn(false)]
        public int? UPDEMP { get; set; }
    }
}
