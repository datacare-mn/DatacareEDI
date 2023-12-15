using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{

    [Table("MEET_TYPE")]
    public class MEET_TYPE : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int MEETTYPEID { get; set; }
        [Required]
        public string TYPENAME { get; set; }
        [Required]
        public int USEREQUEST { get; set; }

        public int USECLASS { get; set; }
        [Required]
        public int STOREID { get; set; }

    }
}
