namespace WebAPI.Models
{
    using Controllers.StoreControllers.Meet;
    using CustomModels;
    using Entities;
    using Helpers;
    using Interfaces;
    using Microsoft.AspNetCore.Http;
    using Oracle.ManagedDataAccess.Client;
    using ProcedureModels;
    using QueryModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading;

    [DbConfigurationType(typeof(ModelConfiguration))]
    public class OracleDbContext : DbContext
    {
        public virtual DbSet<AAA_TEST> AAA_TESTS { get; set; }

        public virtual DbSet<BIZ_COM_USER> BIZ_COM_USER { get; set; }

        public virtual DbSet<BIZ_COMPANY> BIZ_COMPANY { get; set; }

        public virtual DbSet<MEET_ROOM> MEET_ROOM { get; set; }

        public virtual DbSet<BIZ_COM_BRANCH> BIZ_COM_BRANCH { get; set; }

        public virtual DbSet<LoginUser> LoginUser { get; set; }

        public virtual DbSet<RegisterUser> RegisterUser { get; set; }

        public virtual DbSet<SYS_MENU> SYS_MENU { get; set; }

        public virtual DbSet<SYS_MODULE> SYS_MODULE { get; set; }


        public virtual DbSet<MEET_CLASS> MEET_CLASS { get; set; }

        public virtual DbSet<MEET_TYPE> MEET_TYPE { get; set; }



        public virtual DbSet<MEET_CLASS_USERS> MEET_CLASS_USERS { get; set; }

        public virtual DbSet<MEET_TYPE_USERS> MEET_TYPE_USERS { get; set; }

        public virtual DbSet<MEET_BUSINESS_DAYS> MEET_BUSINESS_DAYS { get; set; }

        public virtual DbSet<MEET_SCHEREQ_ATTACH> MEET_SCHEREQ_ATTACH { get; set; }


        public virtual DbSet<MEET_SCHEDULE_REQ> MEET_SCHEDULE_REQ { get; set; }

        public virtual DbSet<SYS_TEST_LOREM> SYS_TEST_LOREM { get; set; }

        public virtual DbSet<BIZ_STORE_BUSINESS> BIZ_STORE_BUSINESS { get; set; }


        public virtual DbSet<BIZ_DEPART> BIZ_DEPART { get; set; }

        public virtual DbSet<BIZ_CONTRACT_TYPE> BIZ_CONTRACT_TYPE { get; set; }

        public virtual DbSet<BIZ_CONTRACT> BIZ_CONTRACT { get; set; }

        public virtual DbSet<BIZ_USER_CONT> BIZ_USER_CONT { get; set; }

        public virtual DbSet<BIZ_USER_DEPART> BIZ_USER_DEPART { get; set; }

        public virtual DbSet<SYSTEM_AUDIT_LOG> SYSTEM_AUDIT_LOG { get; set; }

        public virtual DbSet<SYS_KEEP_UNITTYPE> SYS_KEEP_UNITTYPE { get; set; }

        public virtual DbSet<SYS_ORIGIN> SYS_ORIGIN { get; set; }

        public virtual DbSet<SYS_MEASURE> SYS_MEASURE { get; set; }

        public virtual DbSet<SYS_COLORS> SYS_COLORS { get; set; }

        public virtual DbSet<SYS_BRAND> SYS_BRAND { get; set; }

        public virtual DbSet<SYS_SKU> SYS_SKU { get; set; }

        public virtual DbSet<BIZ_STORE_COMPANY> BIZ_STORE_COMPANY { get; set; }

        public virtual DbSet<SYS_SKU_PUCTURES> SYS_SKU_PUCTURES { get; set; }

        public virtual DbSet<BIZ_STORE_SKU> BIZ_STORE_SKU { get; set; }

        public virtual DbSet<SKU_BUSINESS> SKU_BUSINESS { get; set; }

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


        public IEnumerable<TEST_DATA> TEST_DATAS()
        {

            var value = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<TEST_DATA>("BEGIN TEST_DATA(:RETURN_VALUE); END;",  value).ToList();
        }

        public IEnumerable<SkuListCompany> SkuListCompany(int comid)
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var pcomid = new OracleParameter("P_COMID", comid);
            var param3 = new OracleParameter("P_COMID", OracleDbType.Int32, comid, ParameterDirection.Input);
            return Database.SqlQuery<SkuListCompany>("BEGIN ST_BUSINESS_SKU_LIST(:P_COMID, :RETURN_VALUE); END;", param3, retvalue);

        }


        public IEnumerable<SkuInfo> GetSkuInfo(int comid, string barcode)
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var param3 = new OracleParameter("P_COMID", OracleDbType.Int32, comid, ParameterDirection.Input);
            var param4 = new OracleParameter("P_SKUCD", OracleDbType.Varchar2, barcode, ParameterDirection.Input);
            return Database.SqlQuery<SkuInfo>("BEGIN ST_BUSINESS_SKU_INFO(:P_COMID, :P_SKUCD, :RETURN_VALUE); END;", param3, param4, retvalue);

        }





        public decimal GetTableSequeenceNextValue(string tablename)
        {
            try
            {
                var tableID = Convert.ToDecimal(Database.ExecuteSqlCommand($"SELECT {tablename}_seq.nextval FROM dual", 0));
                return tableID;
            }
            catch (Exception ex)
            {
                return -1;
            }
            
        }

        public object GetSkuList(int comid)
        {
            OracleParameter param = new OracleParameter("P_COMID", comid);
            var value = Database.SqlQuery<SkuListCompany>(QueryList.BusinessSkuList, param).ToList();
            return value;
        }
        


        /// <summary>
        /// Табле-н нэрийг дамжуулахад түүнд хамаарах секюнсе утгыг буцаадаг функц <para />
        /// Хэрэв SEQUENCE үүсээгүй бол -1 гэсэн утга буцаана
        /// Syntax TABLE_NAME_SEQ гэсэн sequence үүссэн байх шаардлагатай 
        /// </summary>
        /// <param name="tablename">TableName</param>
        /// <returns>SEQUENCE_NEXTVAL</returns>
        public decimal GetTableID(string tablename)
        {
            try
            {
                return Convert.ToDecimal(Database.SqlQuery<decimal>($"SELECT {tablename}_seq.nextval nextval FROM dual").Single());
            }
            catch (Exception)
            {
                return -1;
            }
        }


        public int SaveChanges(HttpContext context, int UseAutitLog)
        {
            int userId =Convert.ToInt32(UsefulHelpers.GetIdendityValue(context, Enums.SystemEnumTypes.UserProperties.UserId));
            foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                IAuditableEntity entity = ent.Entity as IAuditableEntity;
                if (ent.State == EntityState.Added)
                {
                    
                    entity.INSYMD = DateTime.Now;
                    entity.INSEMP = userId;
                }
                if (ent.State == EntityState.Modified)
                {
                    entity.UPDYMD = DateTime.Now;
                    entity.UPDEMP = userId;
                }
                   
                foreach (SYSTEM_AUDIT_LOG x in GetAuditRecordsForChange(ent, userId))
                {
                    this.SYSTEM_AUDIT_LOG.Add(x);
                }

            }

            return base.SaveChanges();
        }

        #region AuditLog
        private List<SYSTEM_AUDIT_LOG> GetAuditRecordsForChange(DbEntityEntry dbEntry, int userId)
        {
            List<SYSTEM_AUDIT_LOG> result = new List<SYSTEM_AUDIT_LOG>();

            DateTime changeTime = DateTime.Now;

            TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

            string keyName = dbEntry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;

            if (dbEntry.State == EntityState.Added)
            {

             
                result.Add(new SYSTEM_AUDIT_LOG()
                {
                    LOGID =Convert.ToString(Guid.NewGuid()),
                    USERNAME = Convert.ToString(userId),
                    EVENTDATE= changeTime,
                    EVENTTYPE = "A", // ШИНЭ
                    TABLENAME = tableName,
                    RECORDID = dbEntry.CurrentValues.GetValue<object>(keyName).ToString(), 
                    COLUMNNAME = "*ALL", 
                    NEWVALUE = (dbEntry.CurrentValues.ToObject() is IDescribableEntity) ? (dbEntry.CurrentValues.ToObject() as IDescribableEntity).Describe() : dbEntry.CurrentValues.ToObject().ToString()
                }
                    );
            }
            else if (dbEntry.State == EntityState.Deleted)
            {
                result.Add(new SYSTEM_AUDIT_LOG()
                {
                    LOGID =Convert.ToString(Guid.NewGuid()),
                    USERNAME = Convert.ToString(userId),
                    EVENTDATE = changeTime,
                    EVENTTYPE = "D", // УСТГАГДСАН
                    TABLENAME = tableName,
                    RECORDID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                    COLUMNNAME = "*ALL",
                    NEWVALUE = (dbEntry.OriginalValues.ToObject() is IDescribableEntity) ? (dbEntry.OriginalValues.ToObject() as IDescribableEntity).Describe() : dbEntry.OriginalValues.ToObject().ToString()
                }
                    );
            }
            else if (dbEntry.State == EntityState.Modified)
            {
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                    {
                        result.Add(new SYSTEM_AUDIT_LOG()
                        {
                            LOGID =Convert.ToString(Guid.NewGuid()),
                            USERNAME =Convert.ToString(userId),
                            EVENTDATE = changeTime,
                            EVENTTYPE = "M",    // ӨӨРЧЛӨЛТ ОРСОН
                            TABLENAME = tableName,
                            RECORDID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                            COLUMNNAME = propertyName,
                            ORIGINALVALUE = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(),
                            NEWVALUE = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
                        }
                            );
                    }
                }
            }

            return result;
        }

        #endregion

        #region  AuditColumn
        public int SaveChanges(HttpContext context)
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity
                    && (x.State == System.Data.Entity.EntityState.Added || x.State == System.Data.Entity.EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                IAuditableEntity entity = entry.Entity as IAuditableEntity;
                if (entity != null)
                {
                    //Thread.CurrentPrincipal
                   

                    int UserID =Convert.ToInt32(UsefulHelpers.GetIdendityValue(context, Enums.SystemEnumTypes.UserProperties.UserId));
                    DateTime now = DateTime.UtcNow;

                    if (entry.State == System.Data.Entity.EntityState.Added)
                    {
                        entity.INSEMP = UserID;
                        entity.INSYMD = now;
                    }
                    else
                    {
                        base.Entry(entity).Property(x => x.INSEMP).IsModified = false;
                        base.Entry(entity).Property(x => x.INSYMD).IsModified = false;
                        entity.UPDEMP = UserID;
                        entity.UPDYMD = now;
                    }

                }
            }

            return base.SaveChanges();
        }
        #endregion


    }

    public partial class dbDatacare
    {

    }
}
