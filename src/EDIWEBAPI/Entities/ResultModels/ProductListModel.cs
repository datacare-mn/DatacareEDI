using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.Interfaces;
using EDIWEBAPI.Entities.CustomModels;

namespace EDIWEBAPI.Entities.ResultModels
{
    public class ProductListModel : IResultModel
    {
        public ProductListModel(int _totalCount, IList<ProductDto> _resultList)
        {
            this.totalCount = _totalCount;
            this.resultList = _resultList;
        }

        public IList<ProductDto> resultList
        {
            get;
            set;
        }
    }
}
