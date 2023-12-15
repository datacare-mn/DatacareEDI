using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using WebAPI.Helpers;
using System.IO;

namespace WebAPI.Controllers.SystemControllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly OracleDbContext _dbContext;

        public ValuesController(OracleDbContext context)
        {
            _dbContext = context;
        }

        // GET api/values
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<string> Get()
        {

            //  var sdsds = _dbContext.ToList();

            // var sdfsdf = _dbContext.AAA_TESTS.Find("{key values}"); /// key-r shuud hailt hiij baigaa gesen.
            // var ewrwe = _dbConbtext.AAA_TESTS.ToList(); /// table buhleer n
            // var sdfsdfds = _dbConbtext.AAA_TESTS.Where(x => x.col1 == "werwer" && x.col2 == 5).SingleOrDefault(); /// Where select hiigeed. oldvol gantsiig av. oldku bol null
            //if (dfsfsdd != null)
            //{

            //}

          var SDATAS = _dbContext.TEST_DATAS().SingleOrDefault();
           
            



            return new string[] { "value1", "value2" };
        }


        [HttpPost]
        [Route("upload")]
        public void PostFile(IFormFile uploadedFile)
        {
            //TODO: Save file
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public ObjectResult Get(int id)
        {
            return Ok( new { davhar = "fsdf", treter = 5 });
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
