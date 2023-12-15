using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Controllers.BusinessControllers;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.CustomModels;
using WebAPI.Models.Entities;

namespace WebAPI.Controllers.SystemControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SkuController : Controller
    {
        private readonly OracleDbContext _dbContext;
        readonly ILogger<SkuController> _log;
        readonly ILogger<BusinessSkuController> _skulog;
        readonly ILogger<SkuPicturesController> _skupic;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public SkuController(OracleDbContext context, ILogger<SkuController> log)
        {
            _dbContext = context;
            _log = log;
        }

        /// <summary>
        /// Барааны жагсаалт
        /// </summary>  
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        public ResponseClient Get() 
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnumTypes.UserProperties.CompanyId));
            return ReturnResponce.ListReturnResponce(_dbContext.SkuListCompany(comid));
            //  return ReturnResponce.ListReturnResponce(_dbContext.GetSkuList(comid));
        }


        /// <summary>
        /// Бараа бүртгэх
        /// </summary>
        /// <param name="SKUpic"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy ="BizApiUser")]
        
        public ResponseClient Post([FromBody]SkuList SKUpic)
        {
            try
            {
                SYS_SKU SKU = new SYS_SKU();
                SKU = SKUpic.sku;
                SKU.SKUID = Convert.ToInt32(_dbContext.GetTableID("SYS_SKU"));
                if (ModelState.IsValid)
                {
                    var currentData = _dbContext.SYS_SKU.FirstOrDefault(x => x.SKUCD == SKU.SKUCD);
                    if (currentData == null)
                    {
                        _dbContext.SYS_SKU.Add(SKU);
                        _dbContext.SaveChanges(HttpContext, 1);
                        List <SYS_SKU_PUCTURES> SkPic = new List<SYS_SKU_PUCTURES>();
                       SkPic = SKUpic.pic;
                        foreach (SYS_SKU_PUCTURES pic in SkPic)
                        {
                            pic.SKUID = SKU.SKUID;
                            pic.PICURL = pic.PICURL;
                            pic.LETTERIMAGE = pic.LETTERIMAGE;
                        }


                        SkuPicturesController skuPic = new SkuPicturesController(_dbContext, _skupic);
                        skuPic.Post(SkPic);


                        List<SKU_BUSINESS> lst = new List<SKU_BUSINESS>();

                        SKU_BUSINESS sku = new SKU_BUSINESS()
                        {
                            SKUID = SKU.SKUID,
                            ISOWNER = 1,
                            ISACTIVE = 1,
                            BALANCE = 1,
                            COMID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnumTypes.UserProperties.CompanyId))
                        };
                        lst.Add(sku);
                        BusinessSkuController controller = new BusinessSkuController(_dbContext, _skulog);
                        ResponseClient rs = new ResponseClient();
                        rs = controller.Post(lst);
                        if (rs.Success)
                        {
                            _dbContext.SaveChanges(HttpContext, 1);
                            return ReturnResponce.SaveSucessResponce();
                        }
                        else
                            return ReturnResponce.SaveFailureResponce();

                    }

                    else
                    {
                        int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnumTypes.UserProperties.CompanyId));
                        if (_dbContext.SKU_BUSINESS.Where(x => x.COMID == comid && x.SKUID == SKU.SKUID).Select(x => x.SKUID).First() != 0)
                        {
                            ResponseClient rs = new ResponseClient();
                            rs = Put(SKU);
                            return rs;
                        }
                        else
                        {
                            BusinessSkuController controller = new BusinessSkuController(_dbContext, _skulog);
                            ResponseClient rs = new ResponseClient();
                            List<SKU_BUSINESS> lst = new List<SKU_BUSINESS>();
                            SKU_BUSINESS sku = new SKU_BUSINESS()
                            {
                                SKUID = SKU.SKUID,
                                ISOWNER = 1,
                                ISACTIVE = 1,
                                BALANCE = 1,
                                COMID = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnumTypes.UserProperties.CompanyId))
                            };
                            rs = controller.Post(lst);
                            _dbContext.SaveChanges(HttpContext, 1);
                            return ReturnResponce.SaveSucessResponce();
                        }
                    }
                }
                else
                {
                    return ReturnResponce.ModelIsNotValudResponce();
                }
        }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
}





        /// <summary>
        /// Баркодоор бараа хайх
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "BizApiUser")]
        [Route("Get/{barcode}")]
        public ResponseClient GetData(string barcode)
        {
            if (barcode != null)
            {
                int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnumTypes.UserProperties.CompanyId));
                int skuid =_dbContext.SYS_SKU.Where(x => x.SKUCD == barcode).Select(x => x.SKUID).First();
                var currentItem = _dbContext.SKU_BUSINESS.FirstOrDefault(x=>x.SKUID == skuid);
                    if (currentItem != null && currentItem.COMID == comid)
                    {

                    var pictures = _dbContext.SYS_SKU_PUCTURES.Where(x => x.SKUID == skuid).ToList().OrderByDescending(x=> x.PICTUREID).OrderBy(x=> x.LETTERIMAGE);
                    var data = _dbContext.GetSkuInfo(comid, barcode);
                    SkuInfoVirtual info = new SkuInfoVirtual();
                    info.Sku = data;
                    info.pic = new List<SkuInfoImageData>();
                    foreach (SYS_SKU_PUCTURES pic in pictures)
                    {
                        SkuInfoImageData ImageData = new SkuInfoImageData(pic.LETTERIMAGE, UsefulHelpers.FILEPREVIEWMAINURL + pic.PICURL);
                        info.pic.Add(ImageData);
                    }

            
                    return ReturnResponce.ListReturnResponce(info);
                    }
                    else
                    {
                    var pictures = _dbContext.SYS_SKU_PUCTURES.Where(x => x.SKUID == skuid).ToList();

                    var data = _dbContext.GetSkuInfo(comid, barcode);
                    SkuInfoVirtual info = new SkuInfoVirtual();
                    info.Sku = data;
                    info.pic = new List<SkuInfoImageData>();
                    foreach (SYS_SKU_PUCTURES pic in pictures)
                    {
                        SkuInfoImageData ImageData = new SkuInfoImageData(pic.LETTERIMAGE, UsefulHelpers.FILEPREVIEWMAINURL + pic.PICURL);
                        info.pic.Add(ImageData);
                    }
                        return ReturnResponce.ListReturnResponce(info);
                    }
                }
            else
            {
                return ReturnResponce.NotFoundResponce();
            }
        }




        /// <summary>
        /// Барааны мэдээлэл бүртгэх
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>

        [HttpPut]
        [Authorize(Policy = "BizApiUser")]
        [Route("UpdateSku")]
        public ResponseClient Put([FromBody]SYS_SKU param)
        {
            if (ModelState.IsValid)
            {
                var currentdata = _dbContext.SYS_SKU.FirstOrDefault(x => x.SKUID == param.SKUID);
                if (currentdata != null)
                {
                    currentdata.ISCALVAT = param.ISCALVAT;
                    currentdata.SKUNAME = param.SKUNAME;
                    currentdata.MGLNAME = param.MGLNAME;
                    currentdata.BILLNAME = param.BILLNAME;
                    currentdata.BRANDID = param.BRANDID;
                    currentdata.ORIGINID = param.ORIGINID;
                    currentdata.MODELNO = param.MODELNO;
                    currentdata.COLOR = param.COLOR;
                    currentdata.MAKEDBY = param.MAKEDBY;
                    currentdata.UOM = param.UOM;
                    currentdata.WEIGHT = param.WEIGHT;
                    currentdata.MEASURE = param.MEASURE;
                    currentdata.KEEPUNIT = param.KEEPUNIT;
                    currentdata.KEEPTYPE = param.KEEPTYPE;
                    currentdata.SIZE = param.SIZE;
                    currentdata.BOXCODE = param.BOXCODE;
                    currentdata.BOXWEIGHT = param.BOXWEIGHT;
                    currentdata.BOXQTY = param.BOXQTY;
                    currentdata.BOXCBM = param.BOXCBM;
                    currentdata.DESCRIPTION = param.DESCRIPTION;
                    _dbContext.Entry(currentdata).State = System.Data.Entity.EntityState.Modified;
                    _dbContext.SaveChanges(HttpContext, 1);
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                    return ReturnResponce.NotFoundResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }
        /// <summary>
        /// төлөв идвэхитэй болгох
        /// </summary>
        /// <param name="skuid"></param>
        /// <returns></returns>

        [HttpPut]
        [Authorize(Policy = "BizApiUser")]
        [Route("InactiveSku/{skuid}")]
        public ResponseClient InactiveSku(int skuid)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnumTypes.UserProperties.CompanyId));
            var currentData= _dbContext.SKU_BUSINESS.FirstOrDefault(x => x.COMID == comid && x.SKUID == skuid);
            if (currentData != null)
            {
                currentData.ISACTIVE = 1;
                _dbContext.Entry(currentData).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
                return ReturnResponce.NotFoundResponce();

        }
        /// <summary>
        /// Төлөв идэвхигүй болгох
        /// </summary>
        /// <param name="skuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = "BizApiUser")]
        [Route("DeActiveSku/{skuid}")]
        public ResponseClient DeactiveSku(int skuid)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnumTypes.UserProperties.CompanyId));
            var currentData = _dbContext.SKU_BUSINESS.FirstOrDefault(x => x.COMID == comid && x.SKUID == skuid);
            if (currentData != null)
            {
                currentData.ISACTIVE = 0;
                _dbContext.Entry(currentData).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
                return ReturnResponce.NotFoundResponce();

        }


        /// <summary>
        /// Нөөцийн байгаа төлөвт оруулах 
        /// </summary>
        /// <param name="skuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = "BizApiUser")]
        [Route("BalanceEnable/{skuid}")]
        public ResponseClient BalanceEnable(int skuid)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnumTypes.UserProperties.CompanyId));
            var currentData = _dbContext.SKU_BUSINESS.FirstOrDefault(x => x.COMID == comid && x.SKUID == skuid);
            if (currentData != null)
            {
                currentData.BALANCE = 1;
                _dbContext.Entry(currentData).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
                return ReturnResponce.NotFoundResponce();

        }
        /// <summary>
        /// Нөөц дууссан төлөвт оруулах
        /// </summary>
        /// <param name="skuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = "BizApiUser")]
        [Route("BalanceDisable/{skuid}")]
        public ResponseClient BalanceDisable(int skuid)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnumTypes.UserProperties.CompanyId));
            var currentData = _dbContext.SKU_BUSINESS.FirstOrDefault(x => x.COMID == comid && x.SKUID == skuid);
            if (currentData != null)
            {
                currentData.BALANCE = 0;
                _dbContext.Entry(currentData).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges(HttpContext);
                return ReturnResponce.SaveSucessResponce();
            }
            else
                return ReturnResponce.NotFoundResponce();

        }


        [HttpPost]
        [Authorize(Policy = "BizApiUser")]
        [Route("SkuListInsert")]
        public ResponseClient PostList([FromBody]List<SYS_SKU> param)
        {
            List<SYS_SKU> uData = new List<SYS_SKU>();
            foreach (SYS_SKU vdata in param)
            {
                vdata.SKUID = Convert.ToInt32(_dbContext.GetTableID("SYS_SKU"));
                if (ModelState.IsValid)
                {
                    uData.Add(new SYS_SKU()
                    {
                        ISCALVAT = vdata.ISCALVAT,
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
                        UOM = vdata.UOM,
                        WEIGHT = vdata.WEIGHT,
                        MEASURE = vdata.MEASURE,
                        KEEPUNIT = vdata.KEEPUNIT,
                        KEEPTYPE = vdata.KEEPTYPE,
                        SIZE = vdata.SIZE,
                        BOXCODE = vdata.BOXCODE,
                        BOXWEIGHT = vdata.BOXWEIGHT,
                        BOXQTY = vdata.BOXQTY,
                        BOXCBM = vdata.BOXCBM,
                        DESCRIPTION = vdata.DESCRIPTION
                    });
                }
                else
                    return ReturnResponce.ModelIsNotValudResponce();
            }
            _dbContext.SYS_SKU.AddRange(uData);
            _dbContext.SaveChanges(HttpContext);
            return ReturnResponce.SaveSucessResponce();
        }
    }
}
