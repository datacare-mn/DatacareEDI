using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.MasterSku;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Controllers.MasterSku
{
    [Route("api/mastersku")]
    public class MasterSkuController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<MasterSkuController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public MasterSkuController(OracleDbContext context, ILogger<MasterSkuController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion

        [HttpGet]
        [Route("measures")]
        [Authorize]
        public ResponseClient GetMeasures()
        {
            return ReturnResponce.ListReturnResponce(_dbContext.MST_MEASURE.ToList());
        }

        [HttpGet]
        [Route("departments")]
        [Authorize]
        public ResponseClient GetDepartments()
        {
            return ReturnResponce.ListReturnResponce(_dbContext.MST_DEPARTMENT.Where(d => d.ENABLED == 1).OrderBy(d => d.VIEWORDER).ToList());
        }


        [HttpGet]
        [Route("alldepartments")]
        [AllowAnonymous]
        public ResponseClient GetAllDepartments()
        {
            return ReturnResponce.ListReturnResponce(_dbContext.MST_DEPARTMENT.Where(d => d.ENABLED == 1).OrderBy(d => d.VIEWORDER).ToList());
        }

        [HttpGet]
        [Route("requesttypes")]
        [Authorize]
        public ResponseClient GetRequestTypes()
        {
            var types = (from r in _dbContext.MST_PRODUCT_REQUEST
                         where r.ENABLED == 1
                         orderby r.VIEWORDER
                         select new ProductRequestTypeDto()
                         {
                             ID = r.ID,
                             CODE = r.CODE,
                             NAME = r.NAME,
                             NOTE = r.NOTE
                         }).ToList();

            return ReturnResponce.ListReturnResponce(types);
        }

        #region DEPARTMENT

        [HttpGet]
        [Route("storedepartments")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetStoreDepartments()
        {
            var users = (from u in _dbContext.SYSTEM_USER_DEPARTMENT
                         group u by u.DEPARTMENTID into g
                         select new
                         {
                             DEPARTMENTID = g.Key,
                             QTY = g.Count()
                         }).ToList();
            var mapping = _dbContext.MST_DEPARTMENT_MAPPING.OrderBy(x => x.VIEWORDER).ToList();

            var departments = (from d in _dbContext.MST_DEPARTMENT.ToList()
                               join u in users on d.ID equals u.DEPARTMENTID into lj
                               from l in lj.DefaultIfEmpty()
                               where d.ENABLED == 1
                               orderby d.ID descending
                               select new DepartmentDto()
                               {
                                   ID = d.ID,
                                   NAME = d.NAME,
                                   NOTE = d.NOTE,
                                   VIEWORDER = d.VIEWORDER,
                                   USERQTY = l == null ? 0 : l.QTY,
                                   MAPPINGIDS = String.Join(", ", mapping.Where(x => x.DEPARTMENTID == d.ID).Select(x => x.DEPARTMENTCODE).ToList())
                               }).ToList();

            return ReturnResponce.ListReturnResponce(departments);
        }
        [HttpGet]
        [Route("checkmapping")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient CheckMapping()
        {
            var mapping = _dbContext.MST_DEPARTMENT_MAPPING.Select(x => x.DEPARTMENTCODE).ToList();
            return ReturnResponce.ListReturnResponce(mapping);
        }
        [HttpPut]
        [Route("editmapping/{id}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient EditMapping([FromBody] List<string> MAPPINGIDS, int id)
        {
            try
            {
                var department = _dbContext.MST_DEPARTMENT.FirstOrDefault(x => x.ID == id);
                if (department == null)
                    return ReturnResponce.NotFoundResponce();
                var mapping = _dbContext.MST_DEPARTMENT_MAPPING.Where(x => x.DEPARTMENTID == id).ToList();
                _dbContext.MST_DEPARTMENT_MAPPING.RemoveRange(mapping);
                foreach (var d in MAPPINGIDS)
                {
                    var newMapping = new MST_DEPARTMENT_MAPPING()
                    {
                        DEPARTMENTCODE = d,
                        DEPARTMENTID = id,
                        ENABLED = 1,
                        ID = Convert.ToInt32(Logics.BaseLogic.GetNewId(_dbContext, "MST_DEPARTMENT_MAPPING")),
                        VIEWORDER = 1
                    };
                    _dbContext.MST_DEPARTMENT_MAPPING.Add(newMapping);
                }
                _dbContext.SaveChanges();
                return ReturnResponce.ListReturnResponce(MAPPINGIDS);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        /// Брэнд
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("department/{id}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient GetDepartmentInfo(int id)
        {
            var currentbrand = _dbContext.MST_DEPARTMENT.FirstOrDefault(x => x.ID == id);
            if (currentbrand == null)
                return ReturnResponce.NotFoundResponce();

            var response = new DepartmentDto()
            {
                ID = currentbrand.ID,
                NAME = currentbrand.NAME,
                NOTE = currentbrand.NOTE,
                VIEWORDER = currentbrand.VIEWORDER
            };

            var users = (from d in _dbContext.SYSTEM_USER_DEPARTMENT
                         join u in _dbContext.SYSTEM_USERS on d.USERID equals u.ID
                         where d.DEPARTMENTID == id
                         select u);

            if (users.Any())
            {
                response.USERS = users.ToList().Select(a => UsefulHelpers.GetUserName(a)).ToList();
                response.USERQTY = response.USERS.Count();
            }

            return ReturnResponce.ListReturnResponce(response);
        }

        [HttpPost]
        [Route("department")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient PostDepartment([FromBody] MST_DEPARTMENT param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            try
            {
                param.ID = Convert.ToInt32(Logics.BaseLogic.GetNewId(_dbContext, "MST_DEPARTMENT"));
                param.ENABLED = 1;
                param.VIEWORDER = param.ID;

                Logics.BaseLogic.Insert(_dbContext, param);
                return ReturnResponce.SaveSucessWithIdResponce(param.ID);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }
        
        [HttpPut]
        [Route("department")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient PutDepartment([FromBody] MST_DEPARTMENT param)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            try
            {
                var currentdata = _dbContext.MST_DEPARTMENT.FirstOrDefault(x => x.ID == param.ID);
                if (currentdata == null)
                    return ReturnResponce.NotFoundResponce();
                
                currentdata.NAME = param.NAME;
                currentdata.NOTE = param.NOTE;

                Logics.BaseLogic.Update(_dbContext, currentdata);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        [HttpDelete]
        [Route("department/{id}")]
        [Authorize(Policy = "StoreApiUser")]
        public ResponseClient DeleteDepartment(int id)
        {
            if (!ModelState.IsValid)
                return ReturnResponce.ModelIsNotValudResponce();
            try
            {
                var currentdata = _dbContext.MST_DEPARTMENT.FirstOrDefault(x => x.ID == id);
                if (currentdata == null)
                    return ReturnResponce.NotFoundResponce();

                currentdata.ENABLED = 0;

                Logics.BaseLogic.Update(_dbContext, currentdata);
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        #endregion

        #region Brand

        /// <summary>
        /// Брэнд
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("brand/{id}")]
        [AllowAnonymous]
        public ResponseClient GetBrand(int id)
        {
            var currentbrand = _dbContext.MST_BRAND.FirstOrDefault(x => x.ID == id);
            if (currentbrand != null)
            {
                return ReturnResponce.ListReturnResponce(currentbrand);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        [HttpGet]
        [Route("brand")]
        [Authorize]
        public ResponseClient GetBrandList()
        {
            int comid =Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var currentbrandlist = _dbContext.MST_BRAND.Where(x=> x.ORGID == comid).ToList();
            if (currentbrandlist != null)
            {
                return ReturnResponce.ListReturnResponce(currentbrandlist);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }


        [HttpPost]
        [Route("brand")]
        [Authorize]

        public ResponseClient Post([FromBody]List<MST_BRAND> param)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            List<MST_BRAND> uData = new List<MST_BRAND>();
            foreach (MST_BRAND vdata in param)
            {
                vdata.ID = Convert.ToInt32(_dbContext.GetTableID("MST_BRAND"));
                if (ModelState.IsValid)
                {
                    uData.Add(new MST_BRAND()
                    {
                        ID = vdata.ID,
                        BRANDNAME = vdata.BRANDNAME,
                        ORGID =comid,
                        LOGO = vdata.LOGO
                    });
                }
                else
                    return ReturnResponce.ModelIsNotValudResponce();
            }
            _dbContext.MST_BRAND.AddRange(uData);
            _dbContext.SaveChanges();
            return ReturnResponce.SaveSucessResponce();
        }



        [HttpPut]
        [Route("brand")]
        public ResponseClient Put([FromBody]MST_BRAND param)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.MST_BRAND.FirstOrDefault(x => x.ID == param.ID);
                if (currentdata != null)
                {
                    currentdata.ID = param.ID;
                    currentdata.BRANDNAME = param.BRANDNAME;
                    currentdata.LOGO = param.LOGO;
                    currentdata.ORGID = comid;
                    _dbContext.Entry(currentdata).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges();
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                    return ReturnResponce.NotFoundResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }
        #endregion

        #region Origin


        /// <summary>
        ///	#Гарал үүсэл
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-10-24
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpPost]
        [Route("origin")]
        public ResponseClient Post([FromBody]List<MST_ORIGIN> param)
        {
            List<MST_ORIGIN> uData = new List<MST_ORIGIN>();
            foreach (MST_ORIGIN vdata in param)
            {
                vdata.ID = Convert.ToInt32(_dbContext.GetTableID("MST_ORIGIN"));
                if (ModelState.IsValid)
                {
                    uData.Add(new MST_ORIGIN()
                    {
                        ID = vdata.ID,
                        NAME = vdata.NAME,
                        SHORTNAME = vdata.SHORTNAME,
                    });
                }
                else
                    return ReturnResponce.ModelIsNotValudResponce();
            }
            _dbContext.MST_ORIGIN.AddRange(uData);
            _dbContext.SaveChanges();
            return ReturnResponce.SaveSucessResponce();
        }


        /// <summary>
        /// Гарал үүслийн бүртгэл
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpPut]
        [Route("origin")]
        public ResponseClient Put([FromBody]MST_ORIGIN param)
        {
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.MST_ORIGIN.FirstOrDefault(x => x.ID == param.ID);
                if (currentdata != null)
                {
                    currentdata.ID = param.ID;
                    currentdata.NAME = param.NAME;
                    currentdata.SHORTNAME = param.SHORTNAME;
                    _dbContext.Entry(currentdata).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges();
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                    return ReturnResponce.NotFoundResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }



        /// <summary>
        /// Улсын бүртгэл
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("origin/{id}")]
        [AllowAnonymous]
        public ResponseClient GetOrigin(int id)
        {
            var currentcolor = _dbContext.MST_BRAND.FirstOrDefault(x => x.ID == id);
            if (currentcolor != null)
            {
                return ReturnResponce.ListReturnResponce(currentcolor);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }



        /// <summary>
        /// Гарал үүсэл жагсаалт
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-10-24
        /// Зассан огноо : 
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        /// 


        [HttpGet]
        [Route("origin")]
        [AllowAnonymous]
        public ResponseClient GetOriginList()
        {
            var currentorigin = _dbContext.MST_ORIGIN.ToList();
            if (currentorigin != null)
            {
                return ReturnResponce.ListReturnResponce(currentorigin);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }
        #endregion

        #region uom

        /// <summary>
        ///	#Хэмжих нэгж
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        ///	<author>Pieter Muller</author>



        [HttpGet]
        [Route("uom")]
        [AllowAnonymous]
        public ResponseClient GetOUMList()
        {
            var currentoumlist = _dbContext.MST_UOM.ToList();
            if (currentoumlist != null)
            {
                return ReturnResponce.ListReturnResponce(currentoumlist);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }




        /// <summary>
        ///	Нэгжийн бүртгэл
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Route("uom/{id}")]
        [AllowAnonymous]
        public ResponseClient GetOUM(int id)
        {
            var currentuom = _dbContext.MST_UOM.FirstOrDefault(x => x.ID == id);
            if (currentuom != null)
            {
                return ReturnResponce.ListReturnResponce(currentuom);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }



        /// <summary>
        ///	Нэгжийн мэдээлэл бүртгэх
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpPost]
        [Route("uom")]
        [Authorize]
        public ResponseClient UOMPost([FromBody]List<MST_UOM> param)
        {
            List<MST_UOM> uData = new List<MST_UOM>();
            foreach (MST_UOM vdata in param)
            {
                vdata.ID = Convert.ToInt32(_dbContext.GetTableID("MST_UOM"));
                if (ModelState.IsValid)
                {
                    uData.Add(new MST_UOM()
                    {
                        ID = vdata.ID,
                        NAME = vdata.NAME,
                        SHORTNAME = vdata.SHORTNAME
                    });
                }
                else
                    return ReturnResponce.ModelIsNotValudResponce();
            }
            _dbContext.MST_UOM.AddRange(uData);
            _dbContext.SaveChanges();
            return ReturnResponce.SaveSucessResponce();
        }




        #endregion

        #region sku

        /// <summary>
        ///	Барааны жагсаалт
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpGet]
        [Route("sku/{skuid}")]
        [Authorize]
        public ResponseClient GetByID(int skuid)
        {

            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
            var skuinfos = _dbContext.MST_SKU.FirstOrDefault(x => x.ORGID == comid && x.SKUID == skuid);
            var images = _dbContext.MST_SKU_IMAGES.Where(x => x.SKUID == skuid).ToList();

            if (skuinfos != null)
            {
                SkuInfo info = new SkuInfo();
                info.BALANCE = skuinfos.BALANCE;
                info.BILLNAME = skuinfos.BILLNAME;
                info.BOXCBM = skuinfos.BOXCBM;
                info.BOXCODE = skuinfos.BOXCODE;
                info.BOXQTY = skuinfos.BOXQTY;
                info.BOXWEIGHT = skuinfos.BOXWEIGHT;
                info.BRANDID = skuinfos.BRANDID;
                info.COLOR = skuinfos.COLOR;
                info.DESCRIPTION = skuinfos.DESCRIPTION;
                info.INSEMP = skuinfos.INSEMP;
                info.INSYMD = skuinfos.INSYMD;
                info.ISACTIVE = skuinfos.ISACTIVE;
                info.KEEPUNIT = skuinfos.KEEPUNIT;
                info.KEEPUNITVALUE = skuinfos.KEEPUNITVALUE;
                info.MAKEDBY = skuinfos.MAKEDBY;
                info.MEASURE = skuinfos.MEASURE.Value;
                info.MEASUREVALUE = skuinfos.MEASUREVALUE;
                info.MGLNAME = skuinfos.MGLNAME;
                info.MODELNO = skuinfos.MODELNO;
                info.ORGID = skuinfos.ORGID;
                info.ORIGINID = skuinfos.ORIGINID;
                info.SKUCD = skuinfos.SKUCD;
                info.SKUID = skuinfos.SKUID;
                info.SKUNAME = skuinfos.SKUNAME;
                info.SKUSIZE = skuinfos.SKUSIZE;
                info.UOMID = skuinfos.UOMID;
                info.UOMVALUE = skuinfos.UOMVALUE;
                info.UPDEMP = skuinfos.UPDEMP;
                info.UPDYMD = skuinfos.UPDYMD;
                info.WEIGHT = skuinfos.WEIGHT;
                info.ISCALVAT = skuinfos.ISCALVAT;
                info.IMAGES = images;
                return ReturnResponce.ListReturnResponce(info);
            }
            return ReturnResponce.NotFoundResponce();
        }





        /// <summary>
        ///	Барааны жагсаалт
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpGet]
        [Route("sku")]
        [Authorize]
        public ResponseClient Get()
        {
           int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
           var skulist =  _dbContext.MST_SKU_SELECT(comid).ToList();
            var json = JsonConvert.SerializeObject(skulist);
            //var datalenght = UsefulHelpers.ObjectGetByteLength(skulist);
            return ReturnResponce.ListReturnResponce(skulist);
        }



        /// <summary>
        ///	Барааны бүртгэл
        /// </summary>
        /// <remarks>
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-10-24
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpPost]
        [Authorize]
        [Route("sku")]
        public ResponseClient Post([FromBody]List<MST_SKU> param)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return ReturnResponce.ModelIsNotValudResponce();
                }

                List<MST_SKU> uData = new List<MST_SKU>();
                foreach (MST_SKU vdata in param)
                {
                    vdata.SKUID = Convert.ToInt32(_dbContext.GetTableID("MST_SKU"));
                    if (ModelState.IsValid)
                    {
                        uData.Add(new MST_SKU()
                        {
                            SKUID = vdata.SKUID,
                            SKUCD = vdata.SKUCD,
                            SKUNAME = vdata.SKUNAME,
                            MGLNAME = vdata.MGLNAME,
                            BILLNAME = vdata.BILLNAME,
                            BRANDID = vdata.BRANDID,
                            ORIGINID = vdata.ORIGINID,
                            MODELNO = vdata.MODELNO,
                            COLOR = vdata.COLOR,
                            MAKEDBY = vdata.MAKEDBY,
                            UOMID = vdata.UOMID,
                            WEIGHT = vdata.WEIGHT,
                            MEASURE = vdata.MEASURE,
                            KEEPUNIT = vdata.KEEPUNIT,
                            KEEPUNITVALUE = vdata.KEEPUNITVALUE,
                            SKUSIZE = vdata.SKUSIZE,
                            BOXCODE = vdata.BOXCODE,
                            BOXWEIGHT = vdata.BOXWEIGHT,
                            BOXQTY = vdata.BOXQTY,
                            BOXCBM = vdata.BOXCBM,
                            DESCRIPTION = vdata.DESCRIPTION,
                            INSYMD = vdata.INSYMD,
                            INSEMP = vdata.INSEMP,
                            UPDYMD = vdata.UPDYMD,
                            UPDEMP = vdata.UPDEMP,
                            ISCALVAT = vdata.ISCALVAT,
                            UOMVALUE = vdata.UOMVALUE,
                            MEASUREVALUE = vdata.MEASUREVALUE,
                            BALANCE = vdata.BALANCE,
                            ISACTIVE = vdata.ISACTIVE,
                            ORGID = vdata.ORGID
                        });
                    }
                    else
                        return ReturnResponce.ModelIsNotValudResponce();
                }
                _dbContext.MST_SKU.AddRange(uData);
                _dbContext.SaveChanges();
                return ReturnResponce.SaveSucessResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }



        /// <summary>
        ///	Барааны мэдээлэл засах
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
        ///	<author>Pieter Muller</author>


        [HttpPut]
        [Authorize]
        [Route("sku")]
        public ResponseClient Put([FromBody]MST_SKU param)
        {
            try
            {
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.MST_SKU.FirstOrDefault(x => x.SKUID == param.SKUID);
                if (currentdata != null)
                {
                    currentdata.SKUID = param.SKUID;
                    currentdata.SKUCD = param.SKUCD;
                    currentdata.SKUNAME = param.SKUNAME;
                    currentdata.MGLNAME = param.MGLNAME;
                    currentdata.BILLNAME = param.BILLNAME;
                    currentdata.BRANDID = param.BRANDID;
                    currentdata.ORIGINID = param.ORIGINID;
                    currentdata.MODELNO = param.MODELNO;
                    currentdata.COLOR = param.COLOR;
                    currentdata.MAKEDBY = param.MAKEDBY;
                    currentdata.UOMID = param.UOMID;
                    currentdata.WEIGHT = param.WEIGHT;
                    currentdata.MEASURE = param.MEASURE;
                    currentdata.KEEPUNIT = param.KEEPUNIT;
                    currentdata.KEEPUNITVALUE = param.KEEPUNITVALUE;
                    currentdata.SKUSIZE = param.SKUSIZE;
                    currentdata.BOXCODE = param.BOXCODE;
                    currentdata.BOXWEIGHT = param.BOXWEIGHT;
                    currentdata.BOXQTY = param.BOXQTY;
                    currentdata.BOXCBM = param.BOXCBM;
                    currentdata.DESCRIPTION = param.DESCRIPTION;
                    currentdata.INSYMD = param.INSYMD;
                    currentdata.INSEMP = param.INSEMP;
                    currentdata.UPDYMD = param.UPDYMD;
                    currentdata.UPDEMP = param.UPDEMP;
                    currentdata.ISCALVAT = param.ISCALVAT;
                    currentdata.UOMVALUE = param.UOMVALUE;
                    currentdata.MEASUREVALUE = param.MEASUREVALUE;
                    currentdata.ISACTIVE = param.ISACTIVE;
                    currentdata.BALANCE = param.BALANCE;
                    currentdata.ORGID = param.ORGID;
                    _dbContext.Entry(currentdata).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges();
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                    return ReturnResponce.NotFoundResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        #endregion

        #region SkuImages

        /// <summary>
        ///	#Барааны зураг хадгалах 
        ///	SKUID - барааны ID
        ///	IMAGEURL - зурагны хаяг
        ///	LETTERIMAGE - Шинжилгээнийн бичиг эсэх
        ///	INDEXID - Эрэмбэ
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="барааны зураг"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpPost]
        [Authorize]
        [Route("skuimage")]
        public ResponseClient Post([FromBody]List<MST_SKU_IMAGES> param)
        {
            List<MST_SKU_IMAGES> uData = new List<MST_SKU_IMAGES>();
            foreach (MST_SKU_IMAGES vdata in param)
            {
                vdata.ID = Convert.ToInt32(_dbContext.GetTableID("MST_SKU_IMAGES"));
                if (ModelState.IsValid)
                {
                    uData.Add(new MST_SKU_IMAGES()
                    {
                        ID = vdata.ID,
                        SKUID = vdata.SKUID,
                        IMAGEURL = vdata.IMAGEURL,
                        LETTERIMAGE = vdata.LETTERIMAGE,
                        INDEXID = vdata.INDEXID
                    });
                }
                else
                    return ReturnResponce.ModelIsNotValudResponce();
            }
            _dbContext.MST_SKU_IMAGES.AddRange(uData);
            _dbContext.SaveChanges();
            return ReturnResponce.SaveSucessResponce();

        }


        /// <summary>
        ///	#Зурагийн мэдээлэл засах
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <param name="param"></param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpPut]
        [Authorize]
        [Route("skuimage")]

        public ResponseClient Put([FromBody]MST_SKU_IMAGES param)
        {
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.MST_SKU_IMAGES.FirstOrDefault(x => x.ID == param.ID);
                if (currentdata != null)
                {
                    currentdata.ID = param.ID;
                    currentdata.SKUID = param.SKUID;
                    currentdata.IMAGEURL = param.IMAGEURL;
                    currentdata.LETTERIMAGE = param.LETTERIMAGE;
                    currentdata.INDEXID = param.INDEXID;
                    _dbContext.Entry(currentdata).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges();
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                    return ReturnResponce.NotFoundResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }

        #endregion

        #region ChangeBalance
        /// <summary>
        ///	#Барааны нөөц өөрчлөх функц 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-02
        /// </remarks>
        /// <param name="skuid">Барааны ID</param>
        /// <param name="balance">Барааны нөөц /1, 0/</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>



        [HttpPost]
        [Authorize]
        [Route("skubalancechange/{skuid}/{balance}")]
        public ResponseClient ChangeBalance(int skuid, int balance)
        {
            try
            {
                int comid =Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                var item = _dbContext.MST_SKU.FirstOrDefault(x => x.ORGID == comid && x.SKUID == skuid);
                if (item != null)
                {
                    item.BALANCE = balance;
                    _dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges();
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                    return ReturnResponce.NotFoundResponce();

            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        /// <summary>
        ///	#Барааны төлөв өөрчлөх функц 
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : 2017-11-02
        /// </remarks>
        /// <param name="skuid">Барааны ID</param>
        /// <param name="status">Барааны төлөв /1, 0/</param>
        /// <returns>буцаах утга</returns>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>
       
        [HttpPost]
        [Authorize]
        [Route("skustatuschange/{skuid}/{status}")]
        public ResponseClient ChangeStatus(int skuid, int status)
        {
            try
            {
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.CompanyId));
                var item = _dbContext.MST_SKU.FirstOrDefault(x => x.ORGID == comid && x.SKUID == skuid);
                if (item != null)
                {
                    item.ISACTIVE= status;
                    _dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges();
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                    return ReturnResponce.NotFoundResponce();
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }


        #endregion

        #region Department

        /// <summary>
        ///	#Department
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpGet]
        [Route("depart/{storeid}")]
        [Authorize]
        public ResponseClient GetDepartment(int storeid)
        {
            var currenrtdeparts = _dbContext.MST_DEPART.Where(x=> x.STOREID == storeid).ToList();
            if (currenrtdeparts != null)
            {
                return ReturnResponce.ListReturnResponce(currenrtdeparts);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        /// <summary>
        ///	#Class
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>

        [HttpGet]
        [Route("class/{depcode}")]
        [Authorize]
        public ResponseClient GetClasses(string depcode)
        {
            var currenrtclass = _dbContext.MST_CLASS.Where(x => x.CATCD.Contains(depcode)).ToList();
            if (currenrtclass != null)
            {
                return ReturnResponce.ListReturnResponce(currenrtclass);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }

        /// <summary>
        ///	#SubClass
        /// </summary>
        /// <remarks>
        /// Тайлбар
        /// Програмист : М.Давхарбаяр
        /// Үүсгэсэн огноо : DateTimes
        /// </remarks>
        /// <response code="ResponceClient = true">Ажилттай биелэсэн</response>
        /// <response code="ResponceClient = false">Амжилтгүй</response>


        [HttpGet]
        [Route("subclass/{classcode}")]
        [Authorize]
        public ResponseClient GetSubClasses(string classcode)
        {
            var currenrtclass = _dbContext.MST_SUBCLASS.Where(x => x.SUBCATCD.Contains(classcode)).ToList();
            if (currenrtclass != null)
            {
                return ReturnResponce.ListReturnResponce(currenrtclass);
            }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }




        #endregion





    }
}
