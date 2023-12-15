﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.MasterData
{
    public class MST_SUBCLASS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public string SUBCLASSNAME { get; set; }

        public string SUBCATCD { get; set; }
    }
}
