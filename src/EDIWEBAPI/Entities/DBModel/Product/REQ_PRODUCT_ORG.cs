﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EDIWEBAPI.Entities.Interfaces;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Entities.DBModel.Product
{
    public class REQ_PRODUCT_ORG : IProductRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public int STOREID { get; set; }
        public string REGNO { get; set; }
        public string ORGNAME { get; set; }
        public string CEONAME { get; set; }
        public string EMAIL { get; set; }
        public string MOBILE { get; set; }
        [Required]
        public int DEPARTMENTID { get; set; }
        [Required]
        public int REQUESTID { get; set; }
        public int ENABLED { get; set; }
        public int STATUS { get; set; }
        public int SEEN { get; set; }
        public int ATTACHMENT { get; set; }
        public string NOTE { get; set; }
        public string REQUESTBY { get; set; }
        public DateTime REQUESTDATE { get; set; }
        public int? RECEIVEDBY { get; set; }
        public DateTime? RECEIVEDDATE { get; set; }
        public int? CONFIRMEDBY { get; set; }
        public DateTime? CONFIRMEDDATE { get; set; }
        public DateTime? EXPIREDATE { get; set; }

    }
}
