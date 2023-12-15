﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class LicenseUserDto
    {
        public int BUSINESSID { get; set; }
        public int USERID { get; set; }
        public int LICENSEID { get; set; }
        public string USERMAIL { get; set; }
        public string FIRSTNAME { get; set; }
        public decimal? AMOUNT { get; set; }
    }
}
