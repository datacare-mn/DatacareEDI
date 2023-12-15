using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("MEET_SCHEDULE_REQ")]
    public class MEET_SCHEDULE_REQ : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int REQID { get; set; }
        [Required]
        public int BIZUSERID { get; set; }
        [Required]
        public int MEETTYPEID { get; set; }
        [Required]
        public DateTime SCHEDULEDATE { get; set; }

        public string DESCRIPTION { get; set; }
        [Required]
        public int MEMBERREQCOUNT { get; set; }
    }
}
