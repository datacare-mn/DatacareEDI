using EDIWEBAPI.Entities.DBModel.SendData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class NewItemSend
    {
        public int ID { get; set; }

        public int? INFOTYPE { get; set; }

        public int? CONTRACTID { get; set; }

        public DateTime? SENDDATE { get; set; }

        public int? SENDUSER { get; set; }

        public DateTime? APPLYDATE { get; set; }

        public int? APPLYUSER { get; set; }

        public DateTime? SENDTEMPDATE { get; set; }

        public int? SENDTEMPUSER { get; set; }

        public int? STATUS { get; set; }

        public  List<REQ_NEWITEM> REQ_NEWITEMS { get; set;}
    }
}
