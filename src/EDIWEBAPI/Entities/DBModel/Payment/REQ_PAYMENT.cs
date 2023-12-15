using EDIWEBAPI.Entities.DBModel.SystemManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.Payment
{
    public class REQ_PAYMENT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string STPYMD { get; set; }
        public string CONTRACTNO { get; set; }
        public string DESCRIPTION { get; set; }
        public int BUSINESSID { get; set; }
        public int? STORESEEN { get; set; }
        public int? STOREPRINT { get; set; }
        public string ATTACHFILE { get; set; }
        public DateTime? ATTACHDATE { get; set; }
        public int? APPROVEDUSER { get; set; }
        public DateTime? APPROVEDDATE { get; set; }
    }
}
