using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.Interfaces;
using EDIWEBAPI.Entities.CustomModels;

namespace EDIWEBAPI.Entities.ResultModels
{
    public class ProductRequestListModel : IResultModel
    {
        public ProductRequestListModel(int _totalCount, IList<ProductRequestDto> _resultList)
        {
            this.totalCount = _totalCount;
            this.resultList = _resultList;
        }

        public IList<ProductRequestDto> resultList
        {
            get;
            set;
        }
    }
}
