﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class ProductRequestDto
    {
        public int ID { get; set; }
        public int STOREID { get; set; }
        public string STORENAME { get; set; }
        public int ORGID { get; set; }
        public string ORGNAME { get; set; }
        public int? CONTRACTID { get; set; }
        public string CONTRACTNO { get; set; }
        public string CONTRACTDESC { get; set; }
        public int DEPARTMENTID { get; set; }
        public string DEPARTMENTNAME { get; set; }
        public string BUSINESSNOTE { get; set; }
        public string STORENOTE { get; set; }
        public int REQUESTID { get; set; }
        public string REQUESTCODE { get; set; }
        public string REQUESTNAME { get; set; }
        public int ENABLED { get; set; }
        public int STATUS { get; set; }
        public string STATUSNAME { get; set; }
        public int SEEN { get; set; }
        public int REQUESTBY { get; set; }
        public string REQUESTEDNAME { get; set; }
        public DateTime REQUESTDATE { get; set; }
        public int? RECEIVEDBY { get; set; }
        public string RECEIVEDNAME { get; set; }
        public DateTime? RECEIVEDDATE { get; set; }
        public int? CONFIRMEDBY { get; set; }
        public string CONFIRMEDNAME { get; set; }
        public DateTime? CONFIRMEDDATE { get; set; }

        public int STATUSDECISION { get; set; }
        public int STATUSDURATION { get; set; }
        public DateTime? EXPIREDATE { get; set; }
        public int NOTIFICATION { get; set; }
        public int ATTACHMENT { get; set; }
    }
}
