using EDIWEBAPI.Entities.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.ResultModels{
    public class RoleListModel{
        public RoleListModel() {
        }
        public RoleListModel(int _totalCount, List<SystemOrganizationRoles> _roles) {
            this.roles = _roles;
            this.totalCount = _totalCount;
        }

        public int totalCount { get; set; }
        public List<SystemOrganizationRoles> roles { get; set; }
    } 
}
