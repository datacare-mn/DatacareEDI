using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.Interfaces;


namespace EDIWEBAPI.Entities.FilterViews
{
    public class OganizationWithRolesFilterView:IFilterEntity{
        public string organizationName{get;set;}
    }
}
