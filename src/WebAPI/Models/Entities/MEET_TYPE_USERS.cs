﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    [Table("MEET_TYPE_USERS")]
    public class MEET_TYPE_USERS : AuditableEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int TYPEUSERID { get; set; }
        [Required]
        public int USERID { get; set; }
        [Required]
        public int MEETTYPEID { get; set; }
        [Required]
        public int STOREID { get; set; }
    }
}
