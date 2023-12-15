using Faker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.Entities;

namespace WebAPI.Controllers.SystemControllers
{
    public class LoremTestController : Controller
    {

        private readonly OracleDbContext _dbContext;
        readonly ILogger<LoremTestController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public LoremTestController(OracleDbContext context, ILogger<LoremTestController> log)
        {
            _dbContext = context;
            _log = log;
        }


        /// <summary>
        /// Уулзалтын ажлын өдрүүд
        /// </summary>
        /// <returns>Уулзалтын төрлийн лист</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ResponseClient> GetAllRows()
        {
            ResponseClient response = new ResponseClient();
            response.Success = true;
            response.Value = _dbContext.SYS_TEST_LOREM.ToList();
            return response;

        }

        /// <summary>
        /// Insert rows
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseClient> InsertRows()
        {
            ResponseClient response = new ResponseClient();
            response.Success = true;

            List<SYS_TEST_LOREM> fakeData = new List<SYS_TEST_LOREM>();
            for (int i = 0; i < 1000; i++)
            {
                fakeData.Add(new SYS_TEST_LOREM() { ID =Convert.ToInt32(_dbContext.GetTableID("SYS_TEST_LOREM")), LOREMDATA = StringFaker.Randomize("asdas"), LOREMDATE = Faker.DateTimeFaker.DateTime(DateTime.Now, DateTime.Now.AddDays(130)), LOREMNAME = Faker.NameFaker.FirstName() });
            }

            _dbContext.SYS_TEST_LOREM.AddRange(fakeData);
            _dbContext.SaveChanges(HttpContext);
            response.Value = _dbContext.SYS_TEST_LOREM.ToList(); 
            return response;

        }
        [HttpPost]
        [AllowAnonymous]
        [Route("InsertUser")]
        public async Task<ResponseClient> InsertTestUsers()
        {
            ResponseClient response = new ResponseClient();
            List<BIZ_COM_USER> fakeData = new List<BIZ_COM_USER>();
            for (int i = 0; i < 1000; i++)
            {
                fakeData.Add(new BIZ_COM_USER() { COMID = 1, EMAIL = InternetFaker.Email(), FULLNAME = Faker.NameFaker.Name(), PASSWORD = Faker.StringFaker.AlphaNumeric(10), USERID = Convert.ToInt32(_dbContext.GetTableID("SYS_TEST_LOREM")), USERSTATUS = 1, USERTYPE = 1});
            }
            _dbContext.BIZ_COM_USER.AddRange(fakeData);
            _dbContext.SaveChanges(HttpContext);
            response.Value = _dbContext.BIZ_COM_USER.ToList();
            return response;

            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("InsertTestSku")]
        public ResponseClient InsertItemRows()
        {
            List<SYS_SKU> fakeData = new List<SYS_SKU>();
            for (int i = 0; i < 10; i++)
            {
                fakeData.Add(new SYS_SKU() { BILLNAME = $"Бараа {i}", BOXCBM = 1, BOXCODE = $"code{i}", BOXQTY = i, BOXWEIGHT = 1250, BRANDID = 1, COLOR = 5, DESCRIPTION = Faker.TextFaker.Sentence(), INSEMP = 1, INSYMD = Faker.DateTimeFaker.DateTime(), KEEPTYPE = 1, KEEPUNIT = 1, MAKEDBY = Faker.StringFaker.Alpha(10), MEASURE = 1, MGLNAME = Faker.StringFaker.Alpha(20), MODELNO = Faker.StringFaker.Alpha(20), ORIGINID = 1, SIZE = 42, SKUCD = Convert.ToString(Faker.NumberFaker.Number(1000, 50000)), SKUID = Convert.ToInt32(_dbContext.GetTableID("SYS_SKU")), SKUNAME = Faker.StringFaker.Alpha(30), UOM = 1, WEIGHT = Faker.NumberFaker.Number(1000, 5000)});
            }
            _dbContext.SYS_SKU.AddRange(fakeData);
            _dbContext.SaveChanges(HttpContext);
             var a = _dbContext.SYS_SKU.ToList();
            return ReturnResponce.ListReturnResponce(a);

        }


        [HttpGet]
        [AllowAnonymous]
        [Route("SearchUser/{Value}")]
        public async Task<ResponseClient> SearchUSers(string Value)
        {
            ResponseClient response = new ResponseClient();
            response.Success = true;

            var users = from u in _dbContext.BIZ_COM_USER select u;
            if (!string.IsNullOrWhiteSpace(Value))
                users = users.Where(u => u.FULLNAME.ToLower().Contains(Value.ToLower()));
            response.Value = users.ToList();
            return response;

        }



    }
}
