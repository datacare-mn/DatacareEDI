using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using EDIWEBAPI.Entities.Interfaces;
using EDIWEBAPI.Entities.CustomModels;


namespace EDIWEBAPI.Entities.ResultModels
{
    public class FeedbackListModel : IResultModel
    {
        public FeedbackListModel(int _totalCount, IList<FeedbackListDto> _resultList)
        {
            this.totalCount = _totalCount;
            this.resultList = _resultList;
        }

        public IList<FeedbackListDto> resultList
        {
            get;
            set;
        }

    }
}
