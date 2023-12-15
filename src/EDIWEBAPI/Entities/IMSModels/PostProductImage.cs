using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.IMSModels
{
    public class PostProductImage
    {
        public IList<IFormFile> file { get; set; }
        public List<string> imgnm { get; set; } = new List<string>();
        public List<int> ismain { get; set; } = new List<int>();
        public List<string> skucds { get; set; } = new List<string>();
    }
}
