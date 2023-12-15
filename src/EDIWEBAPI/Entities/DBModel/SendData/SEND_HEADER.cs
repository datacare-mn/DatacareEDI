using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SendData
{
    public class SEND_HEADER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
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
    }
}
