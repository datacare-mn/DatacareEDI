using EDIWEBAPI.Entities.CustomModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_ANNOUNCEMENT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal ID { get; set; }
        public DateTime BEGINDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public int TYPE { get; set; }
        public decimal RECEIVER { get; set; }
        public string TITLE { get; set; }
        public string MESSAGE { get; set; }
        public string IMAGEURL { get; set; }
        public int ENABLED { get; set; }
        public decimal CREATEDBY { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public decimal UPDATEDBY { get; set; }
        public DateTime? UPDATEDDATE { get; set; }

        public SYSTEM_ANNOUNCEMENT()
        {

        }

        public SYSTEM_ANNOUNCEMENT(AnnouncementRequestDto source)
        {
            this.ID = source.ID;
            this.BEGINDATE = source.BEGINDATE;
            this.ENDDATE = source.ENDDATE;
            TYPE = source.TYPE;
            RECEIVER = source.RECEIVER;
            TITLE = source.TITLE;
            MESSAGE = source.MESSAGE;
            IMAGEURL = source.IMAGEURL;
            ENABLED = 1;
        }
    }
}
