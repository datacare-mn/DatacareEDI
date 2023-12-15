﻿using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.MasterData
{
    public class MST_DEPART
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public string DEPARTNAME { get; set; }
        [Required]
        public int STOREID { get; set; }

        public string DEPCD { get; set; }
        [ForeignKey("STOREID")]
        public virtual SYSTEM_ORGANIZATION SYSTEM_ORGANIZATION { get; set; }
    }
}
