using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.CustomModels
{
    public class DataPermissionValue
    {
        public int PermissionTypeID { get; set; }

        public int UserID { get; set; }

        public int ISChecked { get; set; }
    }
}
