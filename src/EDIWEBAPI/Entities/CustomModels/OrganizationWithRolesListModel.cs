using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class OrganizationWithRolesListModel
    {

        public OrganizationWithRolesListModel() {
        }


        public OrganizationWithRolesListModel(int _totalCount, IList<OrganizationWithRoles> _resultList)
        {
            this.totalCount = _totalCount;
            this.resultList = _resultList;
        }

        [DefaultValue(0)]
        public int totalCount
        {
            get; set;
        }

        public IList<OrganizationWithRoles> resultList
        {
            get;
            set;
        }
    }
}
