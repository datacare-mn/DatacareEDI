using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.ResultModels
{
    public class AnnouncementListModel : IResultModel
    {
        public AnnouncementListModel(int _totalCount, IList<AnnouncementRequestDto> _resultList)
        {
            this.totalCount = _totalCount;
            this.resultList = _resultList;
        }

        public IList<AnnouncementRequestDto> resultList
        {
            get;
            set;
        }
    }
}
