using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI.Models
{
    public class BIZ_COM_USER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }

        [Required]
        [MaxLength(50)]
        public string FULLNAME { get; set; }

        [Required]
        public string PASSWORD { get; set; }

        [Required]
        [MaxLength(50)]

        public string EMAIL { get; set; }

        public int COMID { get; set; }

        public int USERTYPE { get; set; }

        public int USERSTATUS { get; set; }

        public string PIC { get; set; }

    }
}
