using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.DBModel.SystemManagement;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class OrganizationWithRoles
    {
        public int id { get; set; }
        public string organizationName { get; set;}
        public string logo { get; set; } 

        public List<SystemOrganizationRoles> roles { get; set; }

    }
}
