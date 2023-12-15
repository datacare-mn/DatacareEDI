using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.Entities;

namespace WebAPI.Models.CustomModels
{
    public class RegisterUser
    {
        [Key]
        public decimal USERID { get; set; }

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



        [ForeignKey("COMID")]
        public virtual BIZ_COMPANY BIZ_COMPANY { get; set; }
    }
}
