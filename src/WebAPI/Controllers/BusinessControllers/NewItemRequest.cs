using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;

namespace WebAPI.Controllers.BusinessControllers
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]


    public class NewItemRequest : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<NewItemRequest> _log;


        /// <summary>
        /// Функцын тайлбар
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 


        public NewItemRequest(OracleDbContext context, ILogger<NewItemRequest> log)
        {
            _dbContext = context;
            _log = log;
        }

        /// <summary>
        /// Функцын тайлбар
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 
        /// Зассан огноо : 
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 



        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient Get()
        {
            try
            {

                return ReturnResponce.SaveFailureResponce();
                //code here
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient Post()
        {
            try
            {

                return ReturnResponce.SaveFailureResponce();
                //code here
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }








    }
}
