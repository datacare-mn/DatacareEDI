using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.SendData;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Controllers.SendData
{
    [Route("api/senddata")]
    public class SendDataController : Controller
    {
        #region Fields
        private readonly OracleDbContext _dbContext;
        readonly ILogger<SendDataController> _log;
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        public SendDataController(OracleDbContext context, ILogger<SendDataController> log)
        {
            _dbContext = context;
            _log = log;
        }
        #endregion


        [HttpPost]
        [Authorize]
        public ResponseClient PostNewItem([FromBody] NewItemSend data)
        {
            data.ID = Convert.ToInt32(_dbContext.GetTableID("SEND_HEADER"));

            if (ModelState.IsValid)
            {
                SEND_HEADER Header = new SEND_HEADER();
                Header.ID = data.ID;
                Header.INFOTYPE = data.INFOTYPE;
                Header.SENDDATE = DateTime.Now;
                Header.SENDUSER = Convert.ToInt32(UsefulHelpers.GetIdendityValue(HttpContext, Enums.SystemEnums.UserProperties.UserId));
                Header.STATUS = 0;
              
                Header.CONTRACTID = data.CONTRACTID;
                _dbContext.SEND_HEADER.Add(Header);
                if (PostReqNewItem(data.REQ_NEWITEMS, data.ID))
                {
                    _dbContext.SaveChanges();
                    return ReturnResponce.SaveSucessResponce();
                }
                else
                    return ReturnResponce.SaveFailureResponce();
            }
            else
                return ReturnResponce.ModelIsNotValudResponce();
        }




        private bool PostReqNewItem([FromBody]List<REQ_NEWITEM> param, int headerid)
        {
            List<REQ_NEWITEM> uData = new List<REQ_NEWITEM>();
            foreach (REQ_NEWITEM vdata in param)
            {
                vdata.ID = Convert.ToInt32(_dbContext.GetTableID("REQ_NEWITEM"));
                if (ModelState.IsValid)
                {
                    uData.Add(new REQ_NEWITEM()
                    {
                        ID = vdata.ID,
                        SKUID = vdata.SKUID,
                        HEADERID = headerid,
                        SUBCLASSID = vdata.SUBCLASSID,
                        SALEPRICE = vdata.SALEPRICE,
                        SUPPLYPRICE = vdata.SUPPLYPRICE,
                        CREATEDUSER = vdata.CREATEDUSER,
                        CREATEDDATE = vdata.CREATEDDATE,
                        SENDUSER = vdata.SENDUSER,
                        SENDDATE = vdata.SENDDATE,
                        REQUESTSTATUS = vdata.REQUESTSTATUS,
                        APPLYUSER = vdata.APPLYUSER,
                        APPLYDATE = vdata.APPLYDATE,
                        BRANCHID = vdata.BRANCHID
                    });
                }
                else
                    return false;
            }
            _dbContext.REQ_NEWITEM.AddRange(uData);
            return true;
        }
    }
}
