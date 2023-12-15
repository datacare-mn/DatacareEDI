﻿using EDIWEBAPI.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.MasterSku
{
    public class MST_DEPARTMENT : IBasicEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public string NAME { get; set; }
        public string NOTE { get; set; }
        public int ENABLED { get; set; }
        public int VIEWORDER { get; set; }
    }
}
