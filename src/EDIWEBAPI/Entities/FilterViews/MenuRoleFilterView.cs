using EDIWEBAPI.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.FilterViews
{
    public class MenuRoleFilterView : IFilterEntity
    {
        public int organizationId { get; set; }
        public int roleId { get; set; }
        public string roleName { get; set; }

    }
}
