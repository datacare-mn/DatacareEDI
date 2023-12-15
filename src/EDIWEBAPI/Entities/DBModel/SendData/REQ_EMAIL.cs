using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDIWEBAPI.Entities.DBModel.SendData
{
    public class REQ_EMAIL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public string EMAIL { get; set; }
        public string PASSCODE { get; set; }
        public int TYPE { get; set; }
        public int CONFIRMED { get; set; }
        public DateTime EXPIREDATE { get; set; }
        public DateTime REQUESTDATE { get; set; }
        public DateTime? CONFIRMEDDATE { get; set; }
    }
}
