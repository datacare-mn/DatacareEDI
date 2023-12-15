using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDIWEBAPI.Entities.DBModel.Test
{
    public class SYSTEM_TEST_CASE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public string CONTROLLER { get; set; }
        public string ROUTE { get; set; }
        public string DESCRIPTION { get; set; }
        public int STATUS { get; set; }
        public int TYPE { get; set; }
        public int SUCCESS { get; set; }
        public int RESPONSE { get; set; }
        public int VIEWORDER { get; set; }
    }
}
