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
    public class REQ_PAYMENT_DISABLED
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public decimal ID { get; set; }
        public decimal PAYMENTID { get; set; }
        public int QUANTITY { get; set; }
        public string DESCRIPTION { get; set; }
        public int? DISABLEDBY { get; set; }
        public DateTime? DISABLEDDATE { get; set; }
    }
}
