using EDIWEBAPI.Entities.DBModel.MasterData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SendData
{
    public class REQ_NEWITEM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int? ID { get; set; }
        [Required]
        public int? SKUID { get; set; }

        public int? SUBCLASSID { get; set; }

        public int? SALEPRICE { get; set; }

        public int? SUPPLYPRICE { get; set; }

        public int? CREATEDUSER { get; set; }

        public DateTime? CREATEDDATE { get; set; }

        public int? SENDUSER { get; set; }

        public DateTime? SENDDATE { get; set; }

        public int? REQUESTSTATUS { get; set; }

        public int? APPLYUSER { get; set; }

        public DateTime? APPLYDATE { get; set; }

        public int? BRANCHID { get; set; }

        public int? HEADERID { get; set; }
        [ForeignKey("HEADERID")]
        public virtual SEND_HEADER SEND_HEADER { get; set; }
        [ForeignKey("SUBCLASSID")]
        public virtual MST_SUBCLASS MST_SUBCLASS { get; set; }
    }
}
