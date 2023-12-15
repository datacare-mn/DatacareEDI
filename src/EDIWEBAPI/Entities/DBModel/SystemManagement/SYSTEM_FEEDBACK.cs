using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_FEEDBACK
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public string USERNAME { get; set; }

        public int? COMPANYID { get; set; }

        public string FEEDBACKNAME { get; set; }

        public string FEEDBACKDESC { get; set; }


        public DateTime? REGDATE { get; set; }

        public int RATEINDEX { get; set; }
    }
}
