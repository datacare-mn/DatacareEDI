using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreAPI.Controllers
{
    [ApiVersion("1.0")]
    //   [Authorize(Policy = "StoreApiUser")]
    [Route("api/[controller]")]
    public class StoreController : Controller
    {
        // GET: /<controller>/
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {

            Process.Start(@"AnvizDoorOpen.exe");
            return new string[] { "Амжилттай!"};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
