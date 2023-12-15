using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class RequestNoteDto
    {
        public string Type { get; set; }
        public int Id { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
    }
}
