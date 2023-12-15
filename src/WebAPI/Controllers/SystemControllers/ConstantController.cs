using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.CustomModels;
using static WebAPI.Enums.SystemEnumTypes;

namespace WebAPI.Controllers.SystemControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ConstantController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<ConstantController> _log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public ConstantController(OracleDbContext context, ILogger<ConstantController> log)
        {
            _dbContext = context;
            _log = log;
        }


        /// <summary>
        /// 7 хоногийн өдөрүүдийг буцаадаг функц
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ResponseClient GetDays()
        {
            DaysData days = new DaysData();
            var enumdays = Enum.GetValues(typeof(DayOfWeekMN));
            List<DaysData> daysData = new List<DaysData>();

            foreach (DayOfWeekMN day in enumdays)
            {
                daysData.Add(new DaysData() { DayIndex = Convert.ToInt32(day), DayName = Enum.GetName(typeof(DayOfWeekMN), day), DayShortName = day.DayOfWeekShortName() });
            }

          return  ReturnResponce.ListReturnResponce(daysData);
        }




    }
}
