using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Entities.Interfaces;
using EDIWEBAPI.Entities.CustomModels;

namespace EDIWEBAPI.Entities.ResultModels
{
    public class UserListModel 
    {
        public UserListModel(int _totalCount, IList<User> _resultList ) {
            this.totalCount = _totalCount;
            this.resultList = _resultList;  
        }

        [DefaultValue(0)]
        public int totalCount {
            get; set;
        }

        public IList<User> resultList {
            get;
            set;
        }
    }
}
