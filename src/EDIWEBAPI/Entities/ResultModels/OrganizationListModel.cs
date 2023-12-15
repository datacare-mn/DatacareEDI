using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using EDIWEBAPI.Entities.CustomModels;


namespace EDIWEBAPI.Entities.ResultModels
{
    public class OrganizationListModel
    {

        public OrganizationListModel(int _totalCount, IList<TreeCompany> _resultList)
        {
            this.totalCount = _totalCount;
            this.resultList = _resultList;
        }

        [DefaultValue(0)]
        public int totalCount
        {
            get; set;
        }

        public IList<TreeCompany> resultList
        {
            get;
            set;
        }


    }
}
