using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDIWEBAPI.Entities.DBModel.Test
{
    public class SYSTEM_TEST_ORGANIZATION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public int ORGANIZATIONID { get; set; }
        public int CONTRACTID { get; set; }
        public string DESCRIPTION { get; set; }
        public int VIEWORDER { get; set; }
        public int STOREID { get; set; }
    }
}
