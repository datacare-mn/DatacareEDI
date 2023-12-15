using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("SYS_TEST_LOREM")]
    public class SYS_TEST_LOREM : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public string LOREMNAME { get; set; }

        public string LOREMDATA { get; set; }

        public DateTime LOREMDATE { get; set; }
    }
}
