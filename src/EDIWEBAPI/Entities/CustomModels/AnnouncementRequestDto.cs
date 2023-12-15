using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class AnnouncementRequestDto
    {
        public decimal ID { get; set; }
        public DateTime BEGINDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public int TYPE { get; set; }
        public decimal RECEIVER { get; set; }
        public string RECEIVERQTY { get; set; }
        public string TITLE { get; set; }
        public string MESSAGE { get; set; }
        public string IMAGEURL { get; set; }
        public decimal CREATEDBY { get; set; }
        public string CREATEDUSER { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public List<decimal> USERS { get; set; }

        public AnnouncementRequestDto()
        {

        }

        public AnnouncementRequestDto(SYSTEM_ANNOUNCEMENT source)
        {
            ID = source.ID;
            BEGINDATE = source.BEGINDATE;
            ENDDATE = source.ENDDATE;
            TYPE = source.TYPE;
            RECEIVER = source.RECEIVER;
            TITLE = source.TITLE;
            MESSAGE = source.MESSAGE;
            IMAGEURL = source.IMAGEURL;
            CREATEDBY = source.CREATEDBY;
            CREATEDDATE = source.CREATEDDATE;
            USERS = new List<decimal>();
        }
    }
}
