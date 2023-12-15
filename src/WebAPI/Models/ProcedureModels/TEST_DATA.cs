using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.ProcedureModels
{
    public class TEST_DATA
    {
        [Key]
        public string Name { get; set; }

        public string Address { get; set; }
    }
}
