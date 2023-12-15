using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.MasterSku;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.Storeapi
{
    [Route("api/masterdata")]
    public class MasterDataController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MasterDataController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MasterDataController(OracleDbContext context, ILogger<MasterDataController> log)
        {
            _dbContext = context;
            _log = log;
        }

        #endregion

        [HttpGet]
        [Authorize]
        [Route("contractmodify/{regno}/{comid}")]
        public async Task<ResponseClient> ContractModify(string regno, int comid)
        {
            return Logics.ContractLogic.Modify(_dbContext, _log, comid, regno);
        }
        
        [HttpPost]
        [Authorize]
        [Route("storemastercategory/{storeid}")]
        public async Task<ResponseClient> GetStoreMasterData(int storeid)
        {
            var curretdiv = _dbContext.MST_MASTER_DIVISION.Where(x=> x.STOREID == storeid).Select(x=>x.ID).ToList<int>();
            var currentdivision = _dbContext.MST_MASTER_DIVISION.Where(x => x.STOREID == storeid).ToList();
            if (curretdiv != null)
            {
                var currentdepart =  _dbContext.MST_MASTER_DEPART.Where(x => curretdiv.Contains(x.DIVID)).Select(x=> x.ID).ToList<int>();
                var currentdepartments = _dbContext.MST_MASTER_DEPART.Where(x => curretdiv.Contains(x.DIVID)).ToList();
                if (currentdepart != null)
                {
                    var currentclass = _dbContext.MST_MASTER_CLASS.Where(x => currentdepart.Contains(x.DEPID)).ToList();
                    List<MasterCategory> category = new List<MasterCategory>();
                    foreach (MST_MASTER_DIVISION div in currentdivision)
                    {
                        MasterCategory cat = new MasterCategory();
                        cat.DivsionName = div.DIVNAME;
                        cat.DivCode = div.DIVCODE;
                        cat.Departs = new List<MaterDepart>();
                        foreach (MST_MASTER_DEPART dep in currentdepartments.Where(x=> x.DIVID == div.ID))
                        {

                            MaterDepart deps = new MaterDepart();
                            deps.DepCode = dep.DEPCODE;
                            deps.DepName = dep.DEPNAME;
                            deps.Classes = new List<MasterClass>();
                            foreach (MST_MASTER_CLASS classes in currentclass.Where(x => x.DEPID == dep.ID))
                            {
                                MasterClass mclass = new MasterClass();
                                mclass.ClassCode = classes.CLASSCODE;
                                mclass.ClassName = classes.CLASSNAME;
                                deps.Classes.Add(mclass);
                            }
                            cat.Departs.Add(deps);

                        }
                        category.Add(cat);
                    }
                    return ReturnResponce.ListReturnResponce(category);


                }
                return ReturnResponce.NotFoundResponce();

            }
            return ReturnResponce.NotFoundResponce();
        }


        [HttpPost]
        [Authorize]
        [Route("storemastercategoryconfig/{storeid}")]
        public async Task<ResponseClient> GetStoreMasterDataConfig(int storeid)
        {
            var curretdiv = _dbContext.MST_MASTER_DIVISION.Where(x => x.STOREID == storeid).Select(x => x.ID).ToList<int>();
            var currentdivision = _dbContext.MST_MASTER_DIVISION.Where(x => x.STOREID == storeid).ToList();
            if (curretdiv != null)
            {
                var currentdepart = _dbContext.MST_MASTER_DEPART.Where(x => curretdiv.Contains(x.DIVID)).Select(x => x.ID).ToList<int>();
                var currentdepartments = _dbContext.MST_MASTER_DEPART.Where(x => curretdiv.Contains(x.DIVID)).ToList();
                if (currentdepart != null)
                {
                    var currclasses = _dbContext.GET_STORE_MASTERCONFIG(storeid).ToList();
                    var currentclass = currclasses.Where(x => currentdepart.Contains(x.DEPID ?? 0)).ToList();
                    List<MasterConfigCategory> category = new List<MasterConfigCategory>();
                    foreach (MST_MASTER_DIVISION div in currentdivision)
                    {
                        MasterConfigCategory cat = new MasterConfigCategory();
                        cat.DivsionName = div.DIVNAME;
                        cat.DivCode = div.DIVCODE;
                        cat.Departs = new List<MasterConfigtDepart>();
                        foreach (MST_MASTER_DEPART dep in currentdepartments.Where(x => x.DIVID == div.ID))
                        {

                            MasterConfigtDepart deps = new MasterConfigtDepart();
                            deps.DepCode = dep.DEPCODE;
                            deps.DepName = dep.DEPNAME;
                            deps.Classes = new List<MasterConfigClass>();
                            foreach (GET_STORE_MASTERCONFIG classes in currentclass.Where(x => x.DEPID == dep.ID))
                            {

                                MasterConfigClass mclass = new MasterConfigClass();
                                mclass.ClassCode = classes.CLASSCODE;
                                mclass.ClassName = classes.CLASSNAME;
                                mclass.Classid = classes.ID;
                                mclass.UserName = classes.FIRSTNAME;
                                mclass.InsEmp = classes.INSEMP;
                                mclass.InsYmd = classes.INSYMD;
                                deps.Classes.Add(mclass);
                            }
                            cat.Departs.Add(deps);

                        }
                        category.Add(cat);
                    }
                    return ReturnResponce.ListReturnResponce(category);


                }
                return ReturnResponce.NotFoundResponce();

            }
            return ReturnResponce.NotFoundResponce();
        }




        [HttpPost]
        [Route("catuserpost")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient Post([FromBody]List<MST_CATUSER_CONFIG> param)
        {
            List<MST_CATUSER_CONFIG> uData = new List<MST_CATUSER_CONFIG>();
            int userid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
            foreach (MST_CATUSER_CONFIG vdata in param)
            {
                vdata.ID = Convert.ToInt32(_dbContext.GetTableID("MST_CATUSER_CONFIG"));
                if (ModelState.IsValid)
                {
                    uData.Add(new MST_CATUSER_CONFIG()
                    {
                        ID = vdata.ID,
                        USERID = vdata.USERID,
                        CLASSID = vdata.CLASSID,
                        INSEMP = userid,
                        INSYMD = DateTime.Now,
                    });
                }
                else
                    return ReturnResponce.ModelIsNotValudResponce();
            }
            _dbContext.MST_CATUSER_CONFIG.AddRange(uData);
            _dbContext.SaveChanges();
            return ReturnResponce.ListReturnResponce(uData);
        }






        [HttpGet]
        [AllowAnonymous]
       // [Authorize]
        [Route("divsion/{comid}")]
        public async Task<ResponseClient> GetDivsion(int comid)
        {
            HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
            if (restUtils.StoreServerConnected)
            {
                var division = JsonConvert.DeserializeObject<List<MST_MASTER_DIVISION>>(Convert.ToString(restUtils.Get($"/api/masterdata/divsion").Result.Value));
                return ReturnResponce.ListReturnResponce(division);
            }

            return ReturnResponce.NotFoundResponce();
        }

        [HttpGet]
        [AllowAnonymous]
        // [Authorize]
        [Route("depart/{comid}")]
        public async Task<ResponseClient> GetDepart(int comid)
        {
            HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
            if (restUtils.StoreServerConnected)
            {
                var division = JsonConvert.DeserializeObject<List<MST_MASTER_DEPART>>(Convert.ToString(restUtils.Get($"/api/masterdata/depart").Result.Value));
                return ReturnResponce.ListReturnResponce(division);
            }

            return ReturnResponce.NotFoundResponce();
        }

        [HttpGet]
        [AllowAnonymous]
        // [Authorize]
        [Route("class/{comid}")]
        public async Task<ResponseClient> GeClass(int comid)
        {
            HttpRestUtils restUtils = new HttpRestUtils(comid, _dbContext);
            if (restUtils.StoreServerConnected)
            {
                var division = JsonConvert.DeserializeObject<List<MST_MASTER_CLASS>>(Convert.ToString(restUtils.Get($"/api/masterdata/class").Result.Value));
                return ReturnResponce.ListReturnResponce(division);
            }

            return ReturnResponce.NotFoundResponce();
        }

    }
}
