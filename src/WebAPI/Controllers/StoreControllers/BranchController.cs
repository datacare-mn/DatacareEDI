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

namespace WebAPI.Controllers.StoreControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Policy = "StoreApiUser")]
    public class BranchController : Controller
    {
            private readonly OracleDbContext _dbContext;
            readonly ILogger<BranchController> _log;

            public BranchController(OracleDbContext context, ILogger<BranchController> log)
            {
                _dbContext = context;
                _log = log;
            }


        [HttpGet("{id}")]
        public async Task<ResponseClient> GetBranch(int id)
        {
            var currentbranch = _dbContext.BIZ_COM_BRANCH.Where(x => x.ID == id).SingleOrDefault();
            if (currentbranch != null)
            {
                return ReturnResponce.ListReturnResponce(currentbranch);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        [HttpPost]
        public async Task<ResponseClient> NewBranch([FromBody] BIZ_COM_BRANCH branch)
        {
                branch.ID = _dbContext.GetTableID("BIZ_COM_BRANCH");
            if (ModelState.IsValid)
            {
                _dbContext.BIZ_COM_BRANCH.Add(branch);
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
            {
                return ReturnResponce.SaveFailureResponce();
            }
        }

        [HttpPut]
        public async Task<ResponseClient> UpdateBranch([FromBody] BIZ_COM_BRANCH branch)
        {
            ResponseClient response = new ResponseClient();
            var currentbranch = _dbContext.BIZ_COM_BRANCH.SingleOrDefault(x => x.ID == branch.ID);
            if (currentbranch != null)
            {
                currentbranch.LOCATION = branch.LOCATION;
                currentbranch.ADDRESS = branch.ADDRESS;
                currentbranch.BRANCHIMAGE = branch.BRANCHIMAGE;
                currentbranch.COMID = branch.COMID;
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        [HttpGet]
       
        public async Task<ResponseClient> GetAllBranch(int storeid)
        {
            var allbranch = _dbContext.BIZ_COM_BRANCH.Where(x => x.COMID == storeid).ToList();
            if (allbranch.Count == 0)
            {
                return ReturnResponce.NotFoundResponce();
            }
            else
            {
                return ReturnResponce.ListReturnResponce(allbranch);

            }
        }
        
    }
}
