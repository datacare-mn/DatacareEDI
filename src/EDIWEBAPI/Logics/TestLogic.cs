using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Context;
using EDIWEBAPI.Utils;
using EDIWEBAPI.Entities.DBModel.Test;
using EDIWEBAPI.Entities.TestModel;

namespace EDIWEBAPI.Logics
{
    public class TestLogic : BaseLogic
    {
        public static IEnumerable<OrganizationContractModel> GetOrganizations(OracleDbContext _context, int storeid)
        {
            return from sto in _context.SYSTEM_TEST_ORGANIZATION
                                 join so in _context.SYSTEM_ORGANIZATION on sto.ORGANIZATIONID equals so.ID
                                 join mc in _context.MST_CONTRACT on sto.CONTRACTID equals mc.CONTRACTID
                   where sto.STOREID == storeid
                                 orderby sto.VIEWORDER
                                 select new OrganizationContractModel()
                                 {
                                     ID = sto.ID,
                                     ORGANIZATIONID = so.ID,
                                     REGNO = so.REGNO,
                                     CONTRACTID = mc.CONTRACTID,
                                     CONTRACTNO = mc.CONTRACTNO,
                                     DESCRIPTION = sto.DESCRIPTION
                                 };
        }

        public static IEnumerable<OrganizationDetailModel> GetDetails(OracleDbContext _context, int id)
        {
            return from std in _context.SYSTEM_TEST_DETAIL
                   join stc in _context.SYSTEM_TEST_CASE on std.CASEID equals stc.ID
                   where std.TESTID == id && std.STATUS == 1
                   orderby stc.VIEWORDER
                   select new OrganizationDetailModel()
                   {
                       ID = stc.ID,
                       TESTID = stc.ID,
                       DETAILID = std.ID,
                       CONTROLLER = stc.CONTROLLER,
                       ROUTE = stc.ROUTE,
                       DESCRIPTION = stc.DESCRIPTION,
                       RESPONSE = stc.RESPONSE,
                       SUCCESS = stc.SUCCESS,
                       TYPE = stc.TYPE
                   };
        }
    }
}
