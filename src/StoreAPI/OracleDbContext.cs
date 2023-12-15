using Oracle.ManagedDataAccess.Client;
using StoreAPI.Helpers;
using StoreAPI.Models;
using StoreAPI.Models.ProcedureModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI
{
    [DbConfigurationType(typeof(ModelConfiguration))]
    public class OracleDbContext : DbContext
    {
        #region Model
        public virtual DbSet<BIZ_COM_USER> BIZ_COM_USER { get; set; }

        public object GetSkuList(int comid)
        {
            OracleParameter param = new OracleParameter("P_COMID", comid);
            var value = Database.SqlQuery<SkuListCompany>(QueryList.BusinessSkuList, param).ToList();
            return value;
        }
        #endregion


        #region Base
        public OracleDbContext(string cString) : base(cString)
        {
            Configuration.LazyLoadingEnabled = false;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //For Oracle is neccesary
            Database.SetInitializer<OracleDbContext>(null);
            modelBuilder.HasDefaultSchema("EDI");
        }
        #endregion


    }
}
