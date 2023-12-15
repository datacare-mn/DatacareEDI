using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_LICENSE_PRICE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]

        public int ID { get; set; }
        public int? MENUID { get; set; }
        public string NAME { get; set; }
        public int TYPE { get; set; }
        public int VIEWORDER { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? ANNUALPRICE { get; set; }
        public int ENABLED { get; set; }
        public string CONTROLLER { get; set; }
        public string ROUTE { get; set; }
        public string NOTE { get; set; }
        public string IMAGEURL { get; set; }
        public int DAILYLIMIT { get; set; }
    }
}
