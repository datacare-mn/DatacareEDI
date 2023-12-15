using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDIWEBAPI.Entities.DBModel.Test
{
    public class SYSTEM_TEST_DETAIL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public int TESTID { get; set; }
        public int CASEID { get; set; }
        public int STATUS { get; set; }
    }
}
