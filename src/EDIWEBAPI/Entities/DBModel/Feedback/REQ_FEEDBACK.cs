using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EDIWEBAPI.Entities.Interfaces;

namespace EDIWEBAPI.Entities.DBModel.Feedback
{
    public class REQ_FEEDBACK : IProductRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public int STOREID { get; set; }
        [Required]
        public int ORGID { get; set; }
        public string SUBJECT { get; set; }
        [Required]
        [Column("TYPEID")]
        public int DEPARTMENTID { get; set; }
        [Required]
        public int REQUESTID { get; set; }
        public int ENABLED { get; set; }
        public int STATUS { get; set; }
        public int SEEN { get; set; }
        public int ATTACHMENT { get; set; }
        public string NOTE { get; set; }
        public int REQUESTBY { get; set; }
        public DateTime REQUESTDATE { get; set; }
        public int? RECEIVEDBY { get; set; }
        public DateTime? RECEIVEDDATE { get; set; }
        public int? CONFIRMEDBY { get; set; }
        public DateTime? CONFIRMEDDATE { get; set; }
        public DateTime? EXPIREDATE { get; set; }
    }
}
