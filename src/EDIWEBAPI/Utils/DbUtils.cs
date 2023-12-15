using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.APIModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Utils
{
    public static class DbUtils
    {
        public static string GetBusinessContractList(int comId, OracleDbContext _dbcontext)
        {
            var contraclist = Logics.ContractLogic.GetContracts(_dbcontext, null, comId)
                .Select(x => new { data = x.CONTRACTNO });

            if (contraclist != null)
            {
                var model = contraclist.ToList();
                return JsonConvert.SerializeObject(model);
            }

            return null;
        }
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
        public static string GetBusinessContractList(HttpContext context, OracleDbContext _dbcontext)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(context, UserProperties.CompanyId));
            return GetBusinessContractList(comid, _dbcontext);
        }

        public static string GetStoreContractList(int storeId, OracleDbContext _dbcontext)
        {
            var contraclist = Logics.ContractLogic.GetStoreContracts(_dbcontext, null, storeId)
                .Select(x => new { data = x.CONTRACTNO });

            if (contraclist != null)
            {
                var model = contraclist.ToList();
                return JsonConvert.SerializeObject(model);
            }

            return null;
        }

        public static string GetStoreContractList(HttpContext context, OracleDbContext _dbcontext)
        {
            int comid = Convert.ToInt32(UsefulHelpers.GetIdendityValue(context, UserProperties.CompanyId));
            return GetStoreContractList(comid, _dbcontext);
        }

        public static string GetSystemContractList(HttpContext context, OracleDbContext _dbcontext, int storeid, int businessid)
        {
            var contraclist = Logics.ContractLogic.GetContracts(_dbcontext, null, storeid, businessid)
                .Select(x => new { data = x.CONTRACTNO });

            if (contraclist != null)
            {
                var model = contraclist.ToList();
                return JsonConvert.SerializeObject(model);
            }

            return null;
        }
    }
}
