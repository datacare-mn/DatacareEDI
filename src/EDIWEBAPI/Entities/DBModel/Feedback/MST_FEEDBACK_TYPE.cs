using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EDIWEBAPI.Entities.Interfaces;

namespace EDIWEBAPI.Entities.DBModel.Feedback
{
    public class MST_FEEDBACK_TYPE : IBasicEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public string NAME { get; set; }
        public int ENABLED { get; set; }
        public int VIEWORDER { get; set; }
    }
}
