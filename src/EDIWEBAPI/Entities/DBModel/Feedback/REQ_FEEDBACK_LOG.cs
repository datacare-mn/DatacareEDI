﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EDIWEBAPI.Enums.SystemEnums;
using EDIWEBAPI.Entities.Interfaces;

namespace EDIWEBAPI.Entities.DBModel.Feedback
{
    public class REQ_FEEDBACK_LOG : IProductLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public int HEADERID { get; set; }
        public int USERID { get; set; }
        public string NOTE { get; set; }
        public int SEEN { get; set; } = 0;
        public int STATUS { get; set; }
        public ORGTYPE ORGTYPE { get; set; }
        public RequestLogType TYPE { get; set; }
        public DateTime? ACTIONDATE { get; set; }
    }
}
