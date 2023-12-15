using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class ImageDto
    {
        public int ID { get; set; }
        public int IMAGETYPE { get; set; }
        public string URL { get; set; }
        public int STATE { get; set; }
    }
}
