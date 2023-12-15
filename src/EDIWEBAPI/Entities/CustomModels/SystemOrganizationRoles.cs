using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class SystemOrganizationRoles
    {
        public int? ID { get; set; }

        public virtual int? STATUS { get; set; }

        public string ROLENAME { get; set; }

        public int? ISSYSTEM { get; set; }
        [Required]
        public int ORGID { get; set; }

    }
}
