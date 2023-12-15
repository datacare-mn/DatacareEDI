
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.ResultModels
{
    public class StoreVendorListModel : IResultModel
    {
        public StoreVendorListModel(int _totalCount, IList<SYSTEM_ORGANIZATION> _resultList)
        {
            this.totalCount = _totalCount;
            this.resultList = _resultList;
        }

        public IList<SYSTEM_ORGANIZATION> resultList
        {
            get;
            set;
        }

    }
}
