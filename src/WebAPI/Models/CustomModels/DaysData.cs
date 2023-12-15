using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.CustomModels
{
    public class DaysData
    {
        [Key]
        public int DayIndex { get; set; }

        public string DayName { get; set; }

        public string DayShortName { get; set; }
    }
}
