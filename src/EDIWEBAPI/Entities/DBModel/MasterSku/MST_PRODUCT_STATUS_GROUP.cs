using EDIWEBAPI.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDIWEBAPI.Entities.DBModel.MasterSku
{
    public class MST_PRODUCT_STATUS_GROUP : IBasicEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public string NAME { get; set; }
        public string COLOR { get; set; }
        public int ENABLED { get; set; }
        public int VIEWORDER { get; set; }
    }
}
