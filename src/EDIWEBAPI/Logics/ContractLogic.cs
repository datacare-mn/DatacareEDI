using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.APIModel;
using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel.MasterSku;
using EDIWEBAPI.Entities.DBModel.Order;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using EDIWEBAPI.Entities.FilterViews;
using EDIWEBAPI.Entities.ResultModels;
using EDIWEBAPI.Enums;
using EDIWEBAPI.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Logics
{
    public class ContractLogic : BaseLogic
    {
        public static MST_CONTRACT GetContract(OracleDbContext _context, string contractNo)
        {
            return _context.MST_CONTRACT.FirstOrDefault(x => x.CONTRACTNO == contractNo);
        }

        public static List<MST_CONTRACT_DEPARTMENT> GetDepartments(OracleDbContext _context, ILogger _logger, int businessId)
        {
            return (from c in _context.MST_CONTRACT
                    join d in _context.MST_CONTRACT_DEPARTMENT on c.CONTRACTID equals d.CONTRACTID
                    where c.BUSINESSID == businessId
                    select d).ToList();
        }
        public static List<MST_DEPARTMENT_MAPPING> GetDepartmentMapping(OracleDbContext _context, ILogger _logger, int userId)
        {
            return (from c in _context.SYSTEM_USER_DEPARTMENT
                    join d in _context.MST_DEPARTMENT_MAPPING on c.DEPARTMENTID equals d.DEPARTMENTID
                    where c.USERID == userId
                    select d).ToList();
        }

        public static List<MST_CONTRACT> GetContracts(OracleDbContext _context, ILogger _logger, int businessId)
        {
            return _context.MST_CONTRACT.Where(x => x.BUSINESSID == businessId).ToList();
        }

        public static List<MST_CONTRACT> GetContracts(OracleDbContext _context, ILogger _logger, int storeId, int businessId, bool active = false)
        {
            return _context.MST_CONTRACT.Where(x => x.BUSINESSID == businessId && x.STOREID == storeId 
                && (active ? x.ISACTIVE == 1 : 1 == 1)).ToList();
        }

        public static List<MST_CONTRACT> GetStoreContracts(OracleDbContext _context, ILogger _logger, int storeId)
        {
            return _context.MST_CONTRACT.Where(a => a.STOREID == storeId).ToList();
        }

        public static ResponseClient Modify(OracleDbContext _context, ILogger _logger, int storeId, string regNo)
        {
            try
            {
                var currentCompany = ManagementLogic.GetOrganization(_context, regNo);
                if (currentCompany == null)
                    return ReturnResponce.NotFoundResponce();

                var restUtils = new HttpRestUtils(storeId, _context);
                if (!restUtils.StoreServerConnected)
                    return ReturnResponce.FailedRemoteServerNotConnectedResponce("No connected server");

                var response = restUtils.Get($"/api/masterdata/contracts/{regNo}").Result;
                if (!response.Success)
                    return response;

                var contracts = JsonConvert.DeserializeObject<List<StoreContract>>(Convert.ToString(response.Value));
                if (contracts == null || !contracts.Any())
                    return ReturnResponce.NotFoundResponce();

                var ediContracts = GetContracts(_context, _logger, currentCompany.ID);
                var ediDepartments = GetDepartments(_context, _logger, currentCompany.ID);

                var distinctContracts = contracts.Select(a => a.contractcode).Distinct();
                foreach (var contractNo in distinctContracts)
                {
                    var currentContract = ediContracts.FirstOrDefault(a => a.CONTRACTNO == contractNo);
                    if (currentContract == null)
                    {
                        var found = contracts.FirstOrDefault(a => a.contractcode == contractNo);
                        currentContract = new MST_CONTRACT()
                        {
                            CONTRACTID = Convert.ToInt32(GetNewId(_context, typeof(MST_CONTRACT).Name)),
                            BUSINESSID = currentCompany.ID,
                            STOREID = storeId,
                            ISACTIVE = 1,
                            CONTRACTTYPE = found.contracttype,
                            CONTRACTNO = contractNo,
                            CONTRACDESC = found.contractname
                        };
                        Insert(_context, currentContract, false);
                    }
                    var emartDepartments = contracts
                        .Where(a => a.contractcode == contractNo && !string.IsNullOrEmpty(a.departmentcode))
                        .Select(a => a.departmentcode).Distinct();
                    foreach (var departmentCode in emartDepartments)
                    {
                        var current = ediDepartments.FirstOrDefault(a => a.CONTRACTID == currentContract.CONTRACTID && a.DEPARTMENTCODE == departmentCode);
                        if (current != null) continue;

                        var newDepartment = new MST_CONTRACT_DEPARTMENT()
                        {
                            ID = GetNewId(_context, typeof(MST_CONTRACT_DEPARTMENT).Name),
                            CONTRACTID = currentContract.CONTRACTID,
                            DEPARTMENTCODE = departmentCode
                        };
                        Insert(_context, newDepartment, false);
                    }
                }
                _context.SaveChanges();
                return ReturnResponce.SuccessMessageResponce("");
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                _logger.LogError(1, ex, string.Format("{0}.{1}", methodBase.DeclaringType, methodBase.Name));
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }

        public static ResponseClient GetVendorList(OracleDbContext _context, ILogger _logger, int orgtype, int comid, VendorFilterView filterview)
        {
            try
            {
                var list = new List<VendorCompany>();
                var value = new ReturnVendorList();
                var organizations = new List<SYSTEM_ORGANIZATION>();

                if ((ORGTYPE)orgtype == ORGTYPE.Бизнес)
                {
                    if (filterview != null)
                    {
                        filterview.COMID = comid;
                        var result = _context.GetBusinessVendorList(filterview);
                        //value.TotalCount = result.totalCount;
                        organizations = result.resultList.ToList();
                    }
                    else
                    {
                        organizations = _context.SYSTEM_ORGANIZATION.Where(x => x.ORGTYPE == ORGTYPE.Дэлгүүр).ToList();
                    }
                }
                else if ((ORGTYPE)orgtype == ORGTYPE.Дэлгүүр)
                {
                    if (filterview != null)
                    {
                        filterview.COMID = comid;
                        var result = _context.GetStoreVendorList(filterview);
                        //value.TotalCount = result.totalCount;
                        organizations = result.resultList.ToList();
                    }
                    else
                    {
                        organizations = _context.SYSTEM_ORGANIZATION.Where(x => x.ORGTYPE == ORGTYPE.Бизнес).ToList();
                    }
                }
                else
                {
                    if (filterview != null)
                    {
                        filterview.COMID = comid;
                        var result = _context.GetSystemVendorList(filterview);
                        //value.TotalCount = result.totalCount;
                        organizations = result.resultList.ToList();
                    }
                    else
                    {
                        organizations = _context.SYSTEM_ORGANIZATION.Where(x => x.ORGTYPE != ORGTYPE.Менежмент).ToList();
                    }
                }
                
                if (organizations.Count == 0)
                    return ReturnResponce.NotFoundResponce();

                foreach (SYSTEM_ORGANIZATION vend in organizations)
                {
                    var company = new VendorCompany(vend);

                    if ((SystemEnums.ORGTYPE)orgtype == SystemEnums.ORGTYPE.Бизнес)
                        company.Contracts = GetContracts(_context, _logger, vend.ID, comid);
                    else if ((SystemEnums.ORGTYPE)orgtype == SystemEnums.ORGTYPE.Дэлгүүр)
                        company.Contracts = GetContracts(_context, _logger, comid, vend.ID);
                    else
                    {
                        if (vend.ORGTYPE == SystemEnums.ORGTYPE.Бизнес)
                            company.Contracts = GetContracts(_context, _logger, vend.ID);
                        else if (vend.ORGTYPE == SystemEnums.ORGTYPE.Дэлгүүр)
                            company.Contracts = GetStoreContracts(_context, _logger, vend.ID);
                    }

                    list.Add(company);
                }

                value.RetVendorList = list;
                value.TotalCount = organizations.Count;
                return ReturnResponce.ListReturnResponce(value);
            }
            catch (Exception ex)
            {
                return ReturnResponce.GetExceptionResponce(ex);
            }
        }



        public static MST_CONTRACT AddContract(OracleDbContext _context, ILogger _log,
            int storeId, int organizationId, ReciveOrder order)
        {
            var contract = new MST_CONTRACT()
            {
                BUSINESSID = organizationId,
                CONTRACTNO = order.ContractNo,
                ISACTIVE = 1,
                STOREID = storeId,
                CONTRACDESC = order.ContractDesc,
                CONTRACTTYPE = order.ContractType
            };
            try
            {
                contract.CONTRACTID = Convert.ToInt16(GetNewId(_context, "MST_CONTRACT"));
                Insert(_context, contract);
            }
            catch (Exception ex)
            {
                _log.LogError($"OrderLogic.AddContract {order.ContractNo} : {ex.Message}");
            }
            return contract;
        }
    }
}
