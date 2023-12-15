using EDIWEBAPI.Entities.CustomModels;
using EDIWEBAPI.Entities.DBModel;
using EDIWEBAPI.Entities.DBModel.MasterData;
using EDIWEBAPI.Entities.DBModel.MasterSku;
using EDIWEBAPI.Entities.DBModel.SendData;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EDIWEBAPI.Entities.FilterViews;
using EDIWEBAPI.Entities.ResultModels;
using System.Collections;
using System.Reflection.Emit;
using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.DBModel.Payment;
using EDIWEBAPI.Entities.DBModel.Order;
using EDIWEBAPI.Entities.DBModel.Test;
using EDIWEBAPI.Entities.DBModel.Product;
using EDIWEBAPI.Utils;
using Microsoft.AspNetCore.Http;
using EDIWEBAPI.Entities.Dashboard;
using EDIWEBAPI.Entities.License;
using EDIWEBAPI.Entities.LicenseConfig;
using EDIWEBAPI.Entities.DBModel.Feedback;

namespace EDIWEBAPI.Context
{

    [DbConfigurationType(typeof(ModelConfiguration))]
    public class OracleDbContext : DbContext
    {

        #region Entity
        public virtual DbSet<AAA_TEST> AAA_TEST { get; set; }
        public virtual DbSet<SYSTEM_ANNOUNCEMENT> SYSTEM_ANNOUNCEMENT { get; set; }
        public virtual DbSet<SYSTEM_ANNOUNCEMENT_CUSTOMER> SYSTEM_ANNOUNCEMENT_CUSTOMER { get; set; }

        public virtual DbSet<SYSTEM_TEST_CASE> SYSTEM_TEST_CASE { get; set; }
        public virtual DbSet<SYSTEM_TEST_ORGANIZATION> SYSTEM_TEST_ORGANIZATION { get; set; }
        public virtual DbSet<SYSTEM_TEST_DETAIL> SYSTEM_TEST_DETAIL { get; set; }
        public virtual DbSet<SYSTEM_TEST_LOG> SYSTEM_TEST_LOG { get; set; }
        public virtual DbSet<SYSTEM_TEST_LIC_ORG> SYSTEM_TEST_LIC_ORG { get; set; }
        public virtual DbSet<SYSTEM_TEST_LIC_USERS> SYSTEM_TEST_LIC_USERS { get; set; }

        public virtual DbSet<SYSTEM_LICENSE_BUSINESS> SYSTEM_LICENSE_BUSINESS { get; set; }
        public virtual DbSet<SYSTEM_LICENSE_USER> SYSTEM_LICENSE_USER { get; set; }
        public virtual DbSet<SYSTEM_LICENSE_LOG> SYSTEM_LICENSE_LOG { get; set; }
        public virtual DbSet<SYSTEM_BIZ_LICENSE> SYSTEM_BIZ_LICENSE { get; set; }
        public virtual DbSet<SYSTEM_ORGANIZATION> SYSTEM_ORGANIZATION { get; set; }

        public virtual DbSet<SYSTEM_BIZ_LIC_DETAIL> SYSTEM_BIZ_LIC_DETAIL { get; set; }

        public virtual DbSet<SYSTEM_MAIL_LOG> SYSTEM_MAIL_LOG { get; set; }

        public virtual DbSet<SYSTEM_ROLE_CONFIG> SYSTEM_ROLE_CONFIG { get; set; }

        public virtual DbSet<SYSTEM_LIC_CONT_CONFIG> SYSTEM_LIC_CONT_CONFIG { get; set; }

        public virtual DbSet<SYSTEM_LIC_SKU_CONFIG> SYSTEM_LIC_SKU_CONFIG { get; set; }

        public virtual DbSet<SYSTEM_LIC_SALE_CONFIG> SYSTEM_LIC_SALE_CONFIG { get; set; }

        public virtual DbSet<SYSTEM_USERS> SYSTEM_USERS { get; set; }
        public virtual DbSet<SYSTEM_USER_DEVICE> SYSTEM_USER_DEVICE { get; set; }
        public virtual DbSet<SYSTEM_USER_DEPARTMENT> SYSTEM_USER_DEPARTMENT { get; set; }
        public virtual DbSet<SYSTEM_USER_ROLES> SYSTEM_USER_ROLES { get; set; }

        public virtual DbSet<SYSTEM_ROLES> SYSTEM_ROLES { get; set; }

        public virtual DbSet<SYSTEM_NEW_MENU> SYSTEM_NEW_MENU { get; set; }
        public virtual DbSet<SYSTEM_LICENSE_PRICE> SYSTEM_LICENSE_PRICE { get; set; }
        public virtual DbSet<SYSTEM_CONTRACT> SYSTEM_CONTRACT { get; set; }
        public virtual DbSet<SYSTEM_CONTRACT_DETAIL> SYSTEM_CONTRACT_DETAIL { get; set; }

        public virtual DbSet<SYSTEM_MENU> SYSTEM_MENU { get; set; }

        public virtual DbSet<SYSTEM_MENU_ROLE> SYSTEM_MENU_ROLE { get; set; }
        public virtual DbSet<Entities.DBModel.MasterSku.MST_MEASURE> MST_MEASURE { get; set; }

        public virtual DbSet<MST_PRODUCT> MST_PRODUCT { get; set; }
        public virtual DbSet<MST_PRODUCT_STORE> MST_PRODUCT_STORE { get; set; }
        public virtual DbSet<MST_PRODUCT_REQUEST> MST_PRODUCT_REQUEST { get; set; }
        public virtual DbSet<MST_PRODUCT_REQUEST_GROUP> MST_PRODUCT_REQUEST_GROUP { get; set; }
        public virtual DbSet<MST_PRODUCT_STATUS> MST_PRODUCT_STATUS { get; set; }
        public virtual DbSet<MST_PRODUCT_STATUS_GROUP> MST_PRODUCT_STATUS_GROUP { get; set; }
        public virtual DbSet<MST_PRODUCT_STATUS_DETAIL> MST_PRODUCT_STATUS_DETAIL { get; set; }

        public virtual DbSet<REQ_EMAIL> REQ_EMAIL { get; set; }
        public virtual DbSet<REQ_PRODUCT> REQ_PRODUCT { get; set; }
        public virtual DbSet<REQ_PRODUCT_IMAGE> REQ_PRODUCT_IMAGE { get; set; }
        public virtual DbSet<REQ_PRODUCT_LOG> REQ_PRODUCT_LOG { get; set; }
        public virtual DbSet<REQ_PRODUCT_ORG> REQ_PRODUCT_ORG { get; set; }
        public virtual DbSet<REQ_PRODUCT_ORG_IMAGE> REQ_PRODUCT_ORG_IMAGE { get; set; }
        public virtual DbSet<REQ_PRODUCT_ORG_LOG> REQ_PRODUCT_ORG_LOG { get; set; }
        public virtual DbSet<REQ_FEEDBACK> REQ_FEEDBACK { get; set; }
        public virtual DbSet<REQ_FEEDBACK_LOG> REQ_FEEDBACK_LOG { get; set; }

        public virtual DbSet<MST_BRAND> MST_BRAND { get; set; }

        public virtual DbSet<MST_ORIGIN> MST_ORIGIN { get; set; }

        public virtual DbSet<MST_UOM> MST_UOM { get; set; }

        public virtual DbSet<MST_SKU> MST_SKU { get; set; }

        public virtual DbSet<MST_SKU_IMAGES> MST_SKU_IMAGES { get; set; }

        public virtual DbSet<MST_DEPART> MST_DEPART { get; set; }
        public virtual DbSet<MST_DEPARTMENT> MST_DEPARTMENT { get; set; }
        public virtual DbSet<MST_DEPARTMENT_MAPPING> MST_DEPARTMENT_MAPPING { get; set; }
        public virtual DbSet<MST_FEEDBACK_TYPE> MST_FEEDBACK_TYPE { get; set; }
        public virtual DbSet<SYSTEM_ENTITY> SYSTEM_ENTITY { get; set; }


        public virtual DbSet<MST_CLASS> MST_CLASS { get; set; }

        public virtual DbSet<MST_SUBCLASS> MST_SUBCLASS { get; set; }

        public virtual DbSet<SYSTEM_BRANCH> SYSTEM_BRANCH { get; set; }

        public virtual DbSet<REQ_NEWITEM> REQ_NEWITEM { get; set; }

        public virtual DbSet<SEND_HEADER> SEND_HEADER { get; set; }

        public virtual DbSet<SYSTEM_ORGMENUROLES> SYSTEM_ORGMENUROLES { get; set; }

        public virtual DbSet<SYSTEM_ORGANIZATION_ROLES> SYSTEM_ORGANIZATION_ROLES { get; set; }

        public virtual DbSet<MST_CONTRACT> MST_CONTRACT { get; set; }
        public virtual DbSet<MST_CONTRACT_DEPARTMENT> MST_CONTRACT_DEPARTMENT { get; set; }

        public virtual DbSet<MST_CONTRACT_USERS> MST_CONTRACT_USERS { get; set; }
        public virtual DbSet<REQ_PAYMENT> REQ_PAYMENT { get; set; }
        public virtual DbSet<REQ_PAYMENT_DISABLED> REQ_PAYMENT_DISABLED { get; set; }

        public virtual DbSet<SYSTEM_LOGIN_REQUEST> SYSTEM_LOGIN_REQUEST { get; set; }

        public virtual DbSet<SYSTEM_CONFIGDATA> SYSTEM_CONFIGDATA { get; set; }

        public virtual DbSet<SYSTEM_SHORTURL> SYSTEM_SHORTURL { get; set; }


        public virtual DbSet<REQ_ORDER> REQ_ORDER { get; set; }

        public virtual DbSet<REQ_RETURN_ORDER> REQ_RETURN_ORDER { get; set; }

        public virtual DbSet<SYSTEM_FEEDBACK> SYSTEM_FEEDBACK { get; set; }

        public virtual DbSet<SYSTEM_APP_USERS> SYSTEM_APP_USERS { get; set; }

        public virtual  DbSet<REQ_PAYMENT_REPORT> REQ_PAYMENT_REPORT { get; set; }

        public virtual DbSet<MST_MASTER_DEPART> MST_MASTER_DEPART { get; set; }

        public virtual DbSet<MST_MASTER_DIVISION> MST_MASTER_DIVISION { get; set; }

        public virtual DbSet<MST_MASTER_CLASS> MST_MASTER_CLASS { get; set; }

        public virtual DbSet<MST_CATUSER_CONFIG> MST_CATUSER_CONFIG { get; set; }

        public virtual DbSet<SYSTEM_USER_ACTION_LOG> SYSTEM_USER_ACTION_LOG { get; set; }

        public virtual DbSet<SYSTEM_USER_STATUS_LOG> SYSTEM_USER_STATUS_LOG { get; set; }

        public virtual DbSet<SYSTEM_STORECYCLE_CONFIG> SYSTEM_STORECYCLE_CONFIG { get; set; }

        public virtual DbSet<SYSTEM_LICENSE> SYSTEM_LICENSE { get; set; }

        public virtual DbSet<SYSTEM_LICENSE_DETAIL> SYSTEM_LICENSE_DETAIL { get; set; }

        public virtual DbSet<SYSTEM_LICENSE_FUNCTION> SYSTEM_LICENSE_FUNCTION { get; set; }

        public virtual DbSet<SYSTEM_LICENSE_PACK_FUNC> SYSTEM_LICENSE_PACK_FUNC { get; set; }

        public virtual DbSet<SYSTEM_LICENSE_PACKAGE> SYSTEM_LICENSE_PACKAGE { get; set; }

        public virtual DbSet<SYSTEM_BANKREQLIST> SYSTEM_BANKREQLIST { get; set; }

        public virtual DbSet<SYSTEM_REQUEST_ACTION_LOG> SYSTEM_REQUEST_ACTION_LOG { get; set; }

        public virtual DbSet<SYSTEM_NOTIFCATION_DATA> SYSTEM_NOTIFCATION_DATA { get; set;}

        public virtual DbSet<MST_ORDER_MOBILECONFIG> MST_ORDER_MOBILECONFIG { get; set; }

        public virtual DbSet<SYSTEM_MESSAGE_ARCHIVE> SYSTEM_MESSAGE_ARCHIVE { get; set; }

        public bool SYSTEM_LICENSE_CREATE_JOB(int storeid, string year, string month)
        {
            try
            {
                var pstoreid = new OracleParameter("P_STOREID", OracleDbType.Int16, storeid, ParameterDirection.Input);
                var pyear = new OracleParameter("P_YEAR", OracleDbType.Varchar2, year, ParameterDirection.Input);
                var pmonth = new OracleParameter("P_MONTH", OracleDbType.Varchar2, month, ParameterDirection.Input);

                Database.SqlQuery<object>("BEGIN SYSTEM_LICENSE_CREATE_JOB(:P_STOREID, :P_YEAR, :P_MONTH); END;", pstoreid, pyear, pmonth);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //SYSTEM_ACCOUNT_LICDATA
        public IEnumerable<SYSTEM_ACCOUNT_LICDATA> SYSTEM_ACCOUNT_LICDATA(string sdate,string edate)
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var psdate = new OracleParameter("P_SDATE", OracleDbType.Varchar2, sdate, ParameterDirection.Input);
            var pedate = new OracleParameter("P_EDATE", OracleDbType.Varchar2, edate, ParameterDirection.Input);
            return Database.SqlQuery<SYSTEM_ACCOUNT_LICDATA>("BEGIN SYSTEM_ACCOUNT_LICDATA(:P_SDATE, :P_EDATE, :RETURN_VALUE); END;", psdate, pedate, retvalue);
        }

        public IEnumerable<GET_LOGIN_USER_MENU_SELECT> GET_LOGIN_USER_MENU_SELECT(int roleid, int moduletype, int isparent)
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var pmoduletype = new OracleParameter("P_MODULETYPE", OracleDbType.Int32, moduletype, ParameterDirection.Input);
            var proleid = new OracleParameter("P_ROLEID", OracleDbType.Int32, roleid, ParameterDirection.Input);
            var pisparent = new OracleParameter("P_ISPARENT", OracleDbType.Int32, isparent, ParameterDirection.Input);
            return Database.SqlQuery<GET_LOGIN_USER_MENU_SELECT>("BEGIN GET_LOGIN_USER_MENU_SELECT(:P_MODULETYPE, :P_ROLEID, :P_ISPARENT, :RETURN_VALUE); END;", pmoduletype, proleid, pisparent, retvalue);
        }


        public IEnumerable<SYSTEM_REPORT_LICENSE_HEADER> SYSTEM_REPORT_LICENSE_HEADER(int p_year, string p_month, int p_businessid, int p_storeid)
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var pstoreid = new OracleParameter("P_STOREID", OracleDbType.Int32, p_storeid, ParameterDirection.Input);
            var pbusinessid = new OracleParameter("P_BUSINESSID", OracleDbType.Int32, p_businessid, ParameterDirection.Input);
            var pyear = new OracleParameter("P_YEAR", OracleDbType.Int32, p_year, ParameterDirection.Input);
            var pmonth = new OracleParameter("P_MONTH", OracleDbType.Varchar2, p_month, ParameterDirection.Input);
            return Database.SqlQuery<SYSTEM_REPORT_LICENSE_HEADER>("BEGIN SYSTEM_REPORT_LICENSE_HEADER(:P_YEAR, :P_MONTH, :P_BUSINESSID, :P_STOREIND, :RETURN_VALUE); END;", pyear, pmonth, pbusinessid, pstoreid, retvalue);
        }


        public IEnumerable<SYSTEM_REPORT_LICENSE_DETAIL> SYSTEM_REPORT_LICENSE_DETAIL(int p_year, string p_month, int p_businessid, int p_storeid)
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var pstoreid = new OracleParameter("P_STOREID", OracleDbType.Int32, p_storeid, ParameterDirection.Input);
            var pbusinessid = new OracleParameter("P_BUSINESSID", OracleDbType.Int32, p_businessid, ParameterDirection.Input);
            var pyear = new OracleParameter("P_YEAR", OracleDbType.Int32, p_year, ParameterDirection.Input);
            var pmonth = new OracleParameter("P_MONTH", OracleDbType.Varchar2, p_month, ParameterDirection.Input);
            return Database.SqlQuery<SYSTEM_REPORT_LICENSE_DETAIL>("BEGIN SYSTEM_REPORT_LICENSE_DETAIL(:P_YEAR, :P_MONTH, :P_BUSINESSID, :P_STOREIND, :RETURN_VALUE); END;", pyear, pmonth, pbusinessid, pstoreid, retvalue);
        }


        //public IEnumerable<SYSTEM_REPORT_LICENSE_HEADER> SYSTEM_REPORT_LICENSE_HEADER()
        //{
        //    var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
        //    var pcomid = new OracleParameter("P_STOREID", OracleDbType.Int32, storeid, ParameterDirection.Input);
        //    return Database.SqlQuery<SYSTEM_REPORT_LICENSE_HEADER>("BEGIN SYSTEM_REPORT_LICENSE_HEADER(:P_STOREID, :RETURN_VALUE); END;", pcomid, retvalue);


        //}




        public IEnumerable<GET_STORE_MASTERCONFIG> GET_STORE_MASTERCONFIG(int storeid)
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var pcomid = new OracleParameter("P_STOREID", OracleDbType.Int32, storeid, ParameterDirection.Input);
            return Database.SqlQuery<GET_STORE_MASTERCONFIG>("BEGIN GET_STORE_MASTERCONFIG(:P_STOREID, :RETURN_VALUE); END;", pcomid, retvalue);
        }



        public IEnumerable<MST_SKU_SELECT> MST_SKU_SELECT(int comid)
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var pcomid = new OracleParameter("P_COMID", comid);
            var param3 = new OracleParameter("P_COMID", OracleDbType.Int32, comid, ParameterDirection.Input);
            return Database.SqlQuery<MST_SKU_SELECT>("BEGIN MST_SKU_SELECT(:P_COMID, :RETURN_VALUE); END;", param3, retvalue);
        }


        public IEnumerable<STORENOTIFDATA> GET_STORENOTIFDATA(string  contractno, string  menus)
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var contractnos = new OracleParameter("P_CONTRACTNO", OracleDbType.Varchar2, contractno, ParameterDirection.Input);
            var menunames = new OracleParameter("P_MENUNAMES", OracleDbType.Varchar2, menus, ParameterDirection.Input);
            return Database.SqlQuery<STORENOTIFDATA>("BEGIN GET_STORENOTIFDATA(:P_CONTRACTNO, :P_MENUNAMES, :RETURN_VALUE); END;", contractnos, menunames, retvalue);
        }

        #region Dashboard

        public IEnumerable<DASH_MONTH_LOGINREQUESTCITY> DASH_MONTH_LOGINREQUESTCITY()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_MONTH_LOGINREQUESTCITY>("BEGIN DASH_MONTH_LOGINREQUESTCITY(:RETURN_VALUE); END;", retvalue);
        }

        public IEnumerable<DASH_MONTH_LOGINREQUESTCOUNTRY> DASH_MONTH_LOGINREQUESTCOUNTRY()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_MONTH_LOGINREQUESTCOUNTRY>("BEGIN DASH_MONTH_LOGINREQUESTCOUNTRY(:RETURN_VALUE); END;", retvalue);
        }

        public IEnumerable<DASH_MONTH_LOGINREQUESTDATE> DASH_MONTH_LOGINREQUESTDATE()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_MONTH_LOGINREQUESTDATE>("BEGIN DASH_MONTH_LOGINREQUESTDATE(:RETURN_VALUE); END;", retvalue);
        }


        public IEnumerable<DASH_MONTH_ROUTEREQUEST> DASH_MONTH_ROUTEREQUEST()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_MONTH_ROUTEREQUEST>("BEGIN DASH_MONTH_ROUTEREQUEST(:RETURN_VALUE); END;", retvalue);
        }

        public IEnumerable<DASH_MONTH_BROWSER> DASH_MONTH_BROWSER()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_MONTH_BROWSER>("BEGIN DASH_MONTH_BROWSER(:RETURN_VALUE); END;", retvalue);
        }

        public IEnumerable<DASH_MONTH_OS> DASH_MONTH_OS()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_MONTH_OS>("BEGIN DASH_MONTH_OS(:RETURN_VALUE); END;", retvalue);
        }

        public IEnumerable<DASH_MONTH_OS_VERSION> DASH_MONTH_OS_VERSION()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_MONTH_OS_VERSION>("BEGIN DASH_MONTH_OS_VERSION(:RETURN_VALUE); END;", retvalue);

        }

        //
        public IEnumerable<DASH_SYSTEM_COUNTDATA> DASH_SYSTEM_COUNTDATA()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_SYSTEM_COUNTDATA>("BEGIN DASH_SYSTEM_COUNTDATA(:RETURN_VALUE); END;", retvalue);
        }

        //

       public IEnumerable<DASH_SYSTEM_FUNCTION_USAGE> DASH_SYSTEM_FUNCTION_USAGE()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_SYSTEM_FUNCTION_USAGE>("BEGIN DASH_SYSTEM_FUNCTION_USAGE(:RETURN_VALUE); END;", retvalue);
        }

        public IEnumerable<DASH_SYSTEM_LOGINRATE_MAP> DASH_SYSTEM_LOGINRATE_MAP()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_SYSTEM_LOGINRATE_MAP>("BEGIN DASH_SYSTEM_LOGINRATE_MAP(:RETURN_VALUE); END;", retvalue);
        }

        //DASH_TOP10_LOGINCOMPANY
        public IEnumerable<DASH_TOP10_LOGINCOMPANY> DASH_TOP10_LOGINCOMPANY()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_TOP10_LOGINCOMPANY>("BEGIN DASH_TOP10_LOGINCOMPANY(:RETURN_VALUE); END;", retvalue);
        }

        //DASH_TOP10_REQUESTCOMPANY
        public IEnumerable<DASH_TOP10_REQUESTCOMPANY> DASH_TOP10_REQUESTCOMPANY()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_TOP10_REQUESTCOMPANY>("BEGIN DASH_TOP10_REQUESTCOMPANY(:RETURN_VALUE); END;", retvalue);
        }

        //DASH_OS_STAT
        public IEnumerable<DASH_OS_STAT> DASH_OS_STAT()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_OS_STAT>("BEGIN DASH_OS_STAT(:RETURN_VALUE); END;", retvalue);
        }

        //DASH_BROWSER_STAT
        public IEnumerable<DASH_BROWSER_STAT> DASH_BROWSER_STAT()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_BROWSER_STAT>("BEGIN DASH_BROWSER_STAT(:RETURN_VALUE); END;", retvalue);
        }
        //DASH_LIC_STAT
        public IEnumerable<DASH_LIC_STAT> DASH_LIC_STAT()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_LIC_STAT>("BEGIN DASH_LIC_STAT(:RETURN_VALUE); END;", retvalue);
        }

        public IEnumerable<DASH_USER_LICINFO> DASH_USER_LICINFO()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_USER_LICINFO>("BEGIN DASH_USER_LICINFO(:RETURN_VALUE); END;", retvalue);
        }


        //DASH_BIZ_LAST_REQUEST
        public IEnumerable<DASH_BIZ_LAST_REQUEST> DASH_BIZ_LAST_REQUEST(int comid)
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var p_comid = new OracleParameter("P_COMID", OracleDbType.Varchar2, comid, ParameterDirection.Input);
            return Database.SqlQuery<DASH_BIZ_LAST_REQUEST>("BEGIN DASH_BIZ_LAST_REQUEST(:P_COMID, :RETURN_VALUE); END;", p_comid, retvalue);
        }

        //DASH_STORE_MONTH_DATA
        public IEnumerable<DASH_STORE_MONTH_DATA> DASH_STORE_MONTH_DATA()
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            return Database.SqlQuery<DASH_STORE_MONTH_DATA>("BEGIN DASH_STORE_MONTH_DATA(:RETURN_VALUE); END;", retvalue);
        }


        #endregion

        //DASH_SYSTEM_LOGINRATE_MAP

        //Get User List by Organization id
        public virtual UserListModel GET_USERS(UserFilterView filter)
        {
            UserListModel result;
        
            if (filter.firstName == null) { filter.firstName = ""; }
            if (filter.lastName == null) { filter.lastName = ""; }
            if (filter.phone == null) { filter.phone = ""; }
            if (filter.orderColumn == null) { filter.orderColumn = "id"; }

            if (filter.regEndDate.Year == 1) {
                filter.regEndDate = new DateTime(9999, 12, 31);
            };

            var firstName = new OracleParameter("first_name", OracleDbType.Varchar2, filter.firstName, ParameterDirection.Input);
            var lastName = new OracleParameter("last_name", OracleDbType.Varchar2, filter.lastName, ParameterDirection.Input);
            var userMail = new OracleParameter("user_mail", OracleDbType.Varchar2, filter.userMail, ParameterDirection.Input);
            var regStartDate = new OracleParameter("reg_start_date", OracleDbType.Date, filter.regStartDate, ParameterDirection.Input);
            var regEndDate = new OracleParameter("reg_end_date", OracleDbType.Date, filter.regEndDate, ParameterDirection.Input);
            var phone = new OracleParameter("phone", OracleDbType.Varchar2, filter.phone, ParameterDirection.Input);
            var roleId = new OracleParameter("is_admin", OracleDbType.Int32, filter.roleId, ParameterDirection.Input);
            var orgId = new OracleParameter("org_id", OracleDbType.Int32, filter.orgId, ParameterDirection.Input);
            var orderColumn = new OracleParameter("order_column", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var startRow = new OracleParameter("start_row", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var rowCount = new OracleParameter("row_count", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            int totalCount = Database.SqlQuery<int>("BEGIN GET_USERS_TOTAL("
                                                                               + ":LAST_NAME,"
                                                                               + ":FIRST_NAME,"
                                                                               + ":USER_MAIL,"
                                                                               + ":REG_DATE_START,"
                                                                               + ":REG_DATE_END,"
                                                                               + ":PHONE,"
                                                                               + ":ROLE_ID,"
                                                                               + ":ORG_ID,"
                                                                               + ":RETVAL); END;",
                                                                               lastName,firstName,userMail,
                                                                               regStartDate,regEndDate,phone,
                                                                               roleId,orgId,retvalue).Single();

            if (totalCount == 0) return null;

            IList<User> users = Database.SqlQuery<User>("BEGIN GET_USERS("
                                                                               + ":LAST_NAME,"
                                                                               + ":FIRST_NAME,"
                                                                               + ":USER_MAIL,"
                                                                               + ":REG_DATE_START,"
                                                                               + ":REG_DATE_END,"
                                                                               + ":PHONE,"
                                                                               + ":ROLE_ID,"
                                                                               + ":ORG_ID,"
                                                                               + ":ORDER_COLUMN,"
                                                                               + ":START_ROW,"
                                                                               + ":ROW_COUNT,"
                                                                               + ":RETVAL); END;",
                                                                               lastName, firstName, userMail,
                                                                               regStartDate, regEndDate, phone,
                                                                               roleId, orgId, orderColumn,
                                                                               startRow,rowCount,retvalue).ToList();
            foreach (var user in users) {
                user.Contracts = from c in MST_CONTRACT
                                 join cu in MST_CONTRACT_USERS on c.CONTRACTID equals cu.CONTRACTID
                                 where (cu.USERID == user.id)
                                 select  c ;
            }
            return new UserListModel (totalCount, users);   
        }
 
  
        public virtual StoreVendorListModel GetStoreVendorList(VendorFilterView filter)
        {
            StoreVendorListModel result;
            if (filter.CEONAME == null) { filter.CEONAME = ""; }
            if (filter.COMPANYNAME == null) { filter.COMPANYNAME = ""; }
            if (filter.EMAIL == null) { filter.EMAIL = ""; }
            if (filter.MOBILE == null) { filter.MOBILE = ""; }
            if (filter.REGNO == null) { filter.REGNO = ""; }




            var email = new OracleParameter("P_EMAIL", OracleDbType.Varchar2, filter.EMAIL, ParameterDirection.Input);
            var ceoname = new OracleParameter("P_CEONAME", OracleDbType.Varchar2, filter.CEONAME, ParameterDirection.Input);
            var companyname = new OracleParameter("P_COMPANYNAME", OracleDbType.Varchar2, filter.COMPANYNAME, ParameterDirection.Input);
            var mobile = new OracleParameter("P_MOBILE", OracleDbType.Varchar2, filter.MOBILE, ParameterDirection.Input);
            var regno = new OracleParameter("P_REGNO", OracleDbType.Varchar2, filter.REGNO, ParameterDirection.Input);
            var orderColumn = new OracleParameter("P_ORDER_COLUMN", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var startRow = new OracleParameter("P_START_ROW", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var rowCount = new OracleParameter("P_ROW_COUNT", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);
            var comid = new OracleParameter("P_COMID", OracleDbType.Int32, filter.COMID, ParameterDirection.Input);
            var enabled = new OracleParameter("P_ENABLED", OracleDbType.Int32, filter.ENABLED, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);


            int totalCount = Database.SqlQuery<int>("BEGIN GET_STOREVENDOR_LIST_COUNT("
             + ":P_COMPANYNAME, "
             + ":P_REGNO, "
             + ":P_CEONAME, "
             + ":P_EMAIL, "
             + ":P_MOBILE, "
             + ":P_ENABLED, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":P_COMID, "
             + ":RETURN_VALUE); END; ",
             companyname, regno,
             ceoname, email,
             mobile, enabled, orderColumn,
             startRow, rowCount, comid, retvalue).Single();

            IList<SYSTEM_ORGANIZATION> vendors = Database.SqlQuery<SYSTEM_ORGANIZATION>("BEGIN GET_STOREVENDOR_LIST("
             + ":P_COMPANYNAME, "
             + ":P_REGNO, "
             + ":P_CEONAME, "
             + ":P_EMAIL, "
             + ":P_MOBILE, "
             + ":P_ENABLED, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":P_COMID, "
             + ":RETURN_VALUE); END; ", 
             companyname, regno, 
             ceoname, email,
             mobile, enabled, orderColumn,
             startRow, rowCount, comid, retvalue).ToList();
            return new StoreVendorListModel(totalCount, vendors);
        }

        public virtual StoreVendorListModel GetBusinessVendorList(VendorFilterView filter)
        {
            StoreVendorListModel result;
            if (filter.CEONAME == null) { filter.CEONAME = ""; }
            if (filter.COMPANYNAME == null) { filter.COMPANYNAME = ""; }
            if (filter.EMAIL == null) { filter.EMAIL = ""; }
            if (filter.MOBILE == null) { filter.MOBILE = ""; }
            if (filter.REGNO == null) { filter.REGNO = ""; }
            var email = new OracleParameter("P_EMAIL", OracleDbType.Varchar2, filter.EMAIL, ParameterDirection.Input);
            var ceoname = new OracleParameter("P_CEONAME", OracleDbType.Varchar2, filter.CEONAME, ParameterDirection.Input);
            var companyname = new OracleParameter("P_COMPANYNAME", OracleDbType.Varchar2, filter.COMPANYNAME, ParameterDirection.Input);
            var mobile = new OracleParameter("P_MOBILE", OracleDbType.Varchar2, filter.MOBILE, ParameterDirection.Input);
            var regno = new OracleParameter("P_REGNO", OracleDbType.Varchar2, filter.REGNO, ParameterDirection.Input);
            var orderColumn = new OracleParameter("P_ORDER_COLUMN", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var startRow = new OracleParameter("P_START_ROW", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var rowCount = new OracleParameter("P_ROW_COUNT", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);
            var comid = new OracleParameter("P_COMID", OracleDbType.Int32, filter.COMID, ParameterDirection.Input);
            var enabled = new OracleParameter("P_ENABLED",  OracleDbType.Int32, filter.ENABLED, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);


            int totalCount = Database.SqlQuery<int>("BEGIN GET_BUSINESSEVENDOR_LIST_COUNT("
             + ":P_COMPANYNAME, "
             + ":P_REGNO, "
             + ":P_CEONAME, "
             + ":P_EMAIL, "
             + ":P_MOBILE, "
             + ":P_ENABLED, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":P_COMID, "
             + ":RETURN_VALUE); END; ",
             companyname, regno,
             ceoname, email,
             mobile, enabled, orderColumn,
             startRow, rowCount, comid, retvalue).Single();

            IList<SYSTEM_ORGANIZATION> vendors = Database.SqlQuery<SYSTEM_ORGANIZATION>("BEGIN GET_BUSINESSVENDOR_LIST("
             + ":P_COMPANYNAME, "
             + ":P_REGNO, "
             + ":P_CEONAME, "
             + ":P_EMAIL, "
             + ":P_MOBILE, "
             + ":P_ENABLED, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":P_COMID, "
             + ":RETURN_VALUE); END; ",
             companyname, regno,
             ceoname, email,
             mobile, enabled, orderColumn,
             startRow, rowCount, comid, retvalue).ToList();
            return new StoreVendorListModel(totalCount, vendors);
        }

        public virtual StoreVendorListModel GetSystemVendorList(VendorFilterView filter)
        {
            StoreVendorListModel result;
            if (filter.CEONAME == null) { filter.CEONAME = ""; }
            if (filter.COMPANYNAME == null) { filter.COMPANYNAME = ""; }
            if (filter.EMAIL == null) { filter.EMAIL = ""; }
            if (filter.MOBILE == null) { filter.MOBILE = ""; }
            if (filter.REGNO == null) { filter.REGNO = ""; }
            var email = new OracleParameter("P_EMAIL", OracleDbType.Varchar2, filter.EMAIL, ParameterDirection.Input);
            var ceoname = new OracleParameter("P_CEONAME", OracleDbType.Varchar2, filter.CEONAME, ParameterDirection.Input);
            var companyname = new OracleParameter("P_COMPANYNAME", OracleDbType.Varchar2, filter.COMPANYNAME, ParameterDirection.Input);
            var mobile = new OracleParameter("P_MOBILE", OracleDbType.Varchar2, filter.MOBILE, ParameterDirection.Input);
            var regno = new OracleParameter("P_REGNO", OracleDbType.Varchar2, filter.REGNO, ParameterDirection.Input);
            var orderColumn = new OracleParameter("P_ORDER_COLUMN", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var startRow = new OracleParameter("P_START_ROW", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var rowCount = new OracleParameter("P_ROW_COUNT", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);
            var comid = new OracleParameter("P_COMID", OracleDbType.Int32, filter.COMID, ParameterDirection.Input);
            var enabled = new OracleParameter("P_ENABLED", OracleDbType.Int32, filter.ENABLED, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);


            int totalCount = Database.SqlQuery<int>("BEGIN GET_SYSTEMVENDOR_LIST_COUNT("
             + ":P_COMPANYNAME, "
             + ":P_REGNO, "
             + ":P_CEONAME, "
             + ":P_EMAIL, "
             + ":P_MOBILE, "
             + ":P_ENABLED, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":P_COMID, "
             + ":RETURN_VALUE); END; ",
             companyname, regno,
             ceoname, email,
             mobile, enabled, orderColumn,
             startRow, rowCount, comid, retvalue).Single();

            IList<SYSTEM_ORGANIZATION> vendors = Database.SqlQuery<SYSTEM_ORGANIZATION>("BEGIN GET_SYSTEMVENDOR_LIST("
             + ":P_COMPANYNAME, "
             + ":P_REGNO, "
             + ":P_CEONAME, "
             + ":P_EMAIL, "
             + ":P_MOBILE, "
             + ":P_ENABLED, "
             + ":P_ORDER_COLUMN, "
             + ":P_START_ROW, "
             + ":P_ROW_COUNT, "
             + ":P_COMID, "
             + ":RETURN_VALUE); END; ",
             companyname, regno,
             ceoname, email,
             mobile, enabled, orderColumn,
             startRow, rowCount, comid, retvalue).ToList();
            return new StoreVendorListModel(totalCount, vendors);
        }


        //Get User List by Organization id
        public virtual IEnumerable<User> ORGANIZATION_USERS_SELECT(int id)
        {
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var pcomid = new OracleParameter("P_ORGANIZATION_ID", OracleDbType.Int32, id, ParameterDirection.Input);
            return Database.SqlQuery<User>("BEGIN ORGANIZATION_USERS_SELECT(:P_ORGANIZATION_ID, :RETURN_VALUE); END;", pcomid, retvalue);
        }

        //Check user mail is duplicated
        public virtual Boolean USER_EXISTS(string userEmail) {
            var retValue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var pUserMail = new OracleParameter("P_USERMAIL", OracleDbType.NVarchar2, userEmail, ParameterDirection.Input);
            IList<User> users = Database.SqlQuery<User>("BEGIN CHECK_USER_EXISTS(:P_USERMAIL, :RETURN_VALUE); END;", pUserMail, retValue).ToList();
            return (users.Count >0);
        }

        //Get User By Id
        public virtual SYSTEM_USERS GET_USER(long id)
        {
            SYSTEM_USERS result;
            var retValue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);
            var pId = new OracleParameter("P_USERID", OracleDbType.Long, id, ParameterDirection.Input);



            result = Database.SqlQuery<SYSTEM_USERS>("BEGIN GET_USER(:P_USERID, :RETURN_VALUE); END;", pId, retValue).FirstOrDefault();
            return result;
        }


        // Get Role list by filter
        public virtual RoleListModel GET_ROLE_LIST(MenuRoleFilterView filter) {
            RoleListModel result = new RoleListModel();
            if (filter.roleName == null) { filter.roleName = ""; }
            if (filter.orderColumn == null) { filter.orderColumn = "id"; }

            var organizationId = new OracleParameter("organization_id", OracleDbType.Long, filter.organizationId, ParameterDirection.Input);
            var roleId = new OracleParameter("role_id", OracleDbType.Long, filter.roleId, ParameterDirection.Input);
            var roleName = new OracleParameter("role_name", OracleDbType.Long, filter.roleName, ParameterDirection.Input);
            var orderColumn = new OracleParameter("order_column", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var startRow = new OracleParameter("start_row", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var rowCount = new OracleParameter("row_count", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);


            int totalCount = Database.SqlQuery<int>("BEGIN GET_ROLE_LIST_TOTAL("
                                                                              + ":P_ORGANIZATION_ID,"
                                                                              + ":P_ROLE_ID,"
                                                                              + ":P_ROLE_NAME,"
                                                                              + ":RETVAL); END;",
                                                                              organizationId, roleId,roleName ,retvalue).Single();

            if (totalCount == 0) return null;

            List<SystemOrganizationRoles> roles = Database.SqlQuery<SystemOrganizationRoles>("BEGIN GET_ROLE_LIST("
                                                                                                 + ":P_ORGANIZATION_ID,"
                                                                                                + ":P_ROLE_ID,"
                                                                                                + ":P_ROLE_NAME,"
                                                                                                + ":ORDER_COLUMN,"
                                                                                                + ":START_ROW,"
                                                                                                + ":ROW_COUNT,"
                                                                                                + ":RETVAL); END;",
                                                                                                organizationId,roleId,roleName,
                                                                                                orderColumn,startRow, rowCount, retvalue)
                                                                                                .ToList();

            result.totalCount = totalCount;
            result.roles = roles;


            return result;
        }

        //Get organization list with roles 
        public virtual OrganizationWithRolesListModel GET_ORGANIZAION_WITH_ROLES(OganizationWithRolesFilterView filter) {
            OrganizationWithRolesListModel result = new OrganizationWithRolesListModel(); 

            if (filter.organizationName == null) { filter.organizationName = ""; }
            if (filter.orderColumn == null) { filter.orderColumn = "id"; }


            var organizationName = new OracleParameter("organization_name", OracleDbType.Varchar2, filter.organizationName, ParameterDirection.Input);
            var orderColumn = new OracleParameter("order_column", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var startRow = new OracleParameter("start_row", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var rowCount = new OracleParameter("row_count", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            int totalCount = Database.SqlQuery<int>("BEGIN GET_ROLE_ORGANZATIONS_TOTAL("
                                                                              + ":ORGANIZATION_NAME,"
                                                                              + ":RETVAL); END;",
                                                                              organizationName, retvalue).Single();

            if (totalCount == 0) return null;


            IList<OrganizationWithRoles> organizations = Database.SqlQuery<OrganizationWithRoles>("BEGIN GET_ROLE_ORGANZATIONS("
                                                                             + ":ORGANIZATION_NAME,"
                                                                           +  ":ORDER_COLUMN,"
                                                                           + ":START_ROW,"
                                                                           + ":ROW_COUNT,"
                                                                           + ":RETVAL); END;",
                                                                           organizationName, orderColumn,
                                                                           startRow, rowCount, retvalue).ToList();


            
            foreach (var organization in organizations) {
                var orgId = new OracleParameter("organization_id", OracleDbType.Int32, organization.id, ParameterDirection.Input);
                List<SystemOrganizationRoles> roles = Database.SqlQuery<SystemOrganizationRoles>("BEGIN GET_ROLES_BY_ORGANIZATIONID("
                                                                             + ":ORGANIZATION_ID,"
                                                                           + ":RETVAL); END;",
                                                                           orgId, retvalue).ToList();

                organization.roles = roles;     
            }


            result.totalCount = totalCount;
            result.resultList = organizations;


            return result;
        }

        //Get menu list by roles 
        public virtual IList<RoleMenu> GET_MENUS_BY_ROLE(long id)
        {
            if (id == null) { id = 0; }
            List<RoleMenu> result = new List<RoleMenu>();   

            var roleId = new OracleParameter("role_id", OracleDbType.Long, id , ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            result = Database.SqlQuery<RoleMenu>("BEGIN GET_PARENT_ROLE_MENUS("
                                                                              + ":ROLE_ID,"
                                                                              + ":RETVAL); END;",
                                                                              roleId, retvalue).ToList();

            foreach (RoleMenu menu in result) {
                var parentId = new OracleParameter("parent_id", OracleDbType.Int32, menu.menuId, ParameterDirection.Input);
                menu.childMenus = Database.SqlQuery<RoleMenu>("BEGIN GET_CHILD_ROLE_MENUS("
                                                                              + ":PARENT_ID,"
                                                                              + ":ROLE_ID,"
                                                                              + ":RETVAL); END;",
                                                                              parentId,roleId, retvalue).ToList();
            }

            return result;
        }

        //Get menu list by roles 
        public virtual IList<RoleMenu> GET_MENUS_BY_ORGANIZATION_TYPE(int type)
        {
            if (type == null) { type = 0; }
            List<RoleMenu> result = new List<RoleMenu>();

            var orgType = new OracleParameter("type", OracleDbType.Long, type, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            result = Database.SqlQuery<RoleMenu>("BEGIN GET_PARENT_MENUS_BY_ORG_TYPE("
                                                                              + ":P_ORG_TYPE,"
                                                                              + ":RETVAL); END;",
                                                                              orgType,retvalue).ToList();

            foreach (RoleMenu menu in result)
            {
                var parentId = new OracleParameter("parent_id", OracleDbType.Int32, menu.menuId, ParameterDirection.Input);
                menu.childMenus = Database.SqlQuery<RoleMenu>("BEGIN GET_CHILD_MENUS_BY_ORG_TYPE("
                                                                              + ":P_PARENT_ID,"
                                                                              + ":P_ORG_TYPE,"
                                                                              + ":RETVAL); END;",
                                                                              parentId, orgType, retvalue).ToList();
            }

            return result;
        }






        //Save role 
        public int SAVE_ROLE(SYSTEM_ORGANIZATION_ROLES _role )
        {
         
            int result = 0;

            var roleName = new OracleParameter("role_name", OracleDbType.NVarchar2, _role.ROLENAME, ParameterDirection.Input);
            var orgId = new OracleParameter("org_id", OracleDbType.Int32, _role.ORGID,ParameterDirection.Input);
            var roleId = new OracleParameter("role_id", OracleDbType.Int32, _role.ID, ParameterDirection.Input);
            var isSystem = new OracleParameter("is_system", OracleDbType.Int32, _role.ISSYSTEM, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);


          

            result = Database.SqlQuery<int>("BEGIN SAVE_ROLE("
                                                                    + ":P_ROLE_NAME,"
                                                                    + ":P_ROLE_ID,"
                                                                    + ":P_ORG_ID,"
                                                                    + ":P_IS_SYSTEM,"
                                                                    + ":RETVAL); END;",
                                                                    roleName, roleId,orgId, isSystem, retvalue).FirstOrDefault();
            return result;
        }

        //Remove menu role 
        public int REMOVE_ROLE(int _roleId, int _menuId)
        {

            int result = 0;

            var roleId = new OracleParameter("role_id", OracleDbType.Int32, _roleId, ParameterDirection.Input);
            var menuId = new OracleParameter("role_id", OracleDbType.Int32, _menuId, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);


            result = Database.SqlQuery<int>("BEGIN DELETE_MENU_ROLE("
                                                                    + ":P_ROLE_ID,"
                                                                    + ":P_MENU_ID,"
                                                                    + ":RETVAL); END;",
                                                                    roleId, menuId,  retvalue).FirstOrDefault();
            return result;
        }

        //Add role menus 
        public int ADD_ROLE_MENU(int _id,int _menuId, int _roleId)
        {

            int result = 0;

            var id = new OracleParameter("id", OracleDbType.Int32, _id, ParameterDirection.Input);
            var menuId = new OracleParameter("menu_id", OracleDbType.Int32, _menuId, ParameterDirection.Input);
            var roleId = new OracleParameter("role_id", OracleDbType.Int32, _roleId, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);


            result = Database.SqlQuery<int>("BEGIN ADD_ROLE_MENU("
                                                                    + ":P_ID,"
                                                                    + ":P_MENU_ID,"
                                                                    + ":P_ROLE_ID,"
                                                                    + ":RETVAL); END;",
                                                                    id, menuId,roleId,retvalue).FirstOrDefault();
            return result;
        }


        public virtual IList<RoleMenu> GET_ADD_MENUS(int _roleId)
        {
            if (_roleId == null) { _roleId = 0; }
            List<RoleMenu> result = new List<RoleMenu>();

            var roleId = new OracleParameter("role_id", OracleDbType.Int32, _roleId, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            result = Database.SqlQuery<RoleMenu>("BEGIN GET_PARENT_ORG_ADD_MENUS("
                                                                              + ":P_ROLE_ID,"
                                                                              + ":RETVAL); END;",
                                                                              roleId, retvalue).ToList();

            foreach (RoleMenu menu in result){
                var parentId = new OracleParameter("parent_id", OracleDbType.Int32, menu.menuId, ParameterDirection.Input);
                menu.childMenus = Database.SqlQuery<RoleMenu>("BEGIN GET_CHILD_ORG_ADD_MENUS("
                                                                              + ":PARENT_ID,"
                                                                              + ":ROLE_ID,"
                                                                              + ":RETVAL); END;",
                                                                              parentId, roleId, retvalue).ToList();
            }

            return result;
        }


        public virtual OrganizationListModel GET_COMPANIES(OrganizationFilterView filter)
        {
            if (filter.organizationName == null)
                filter.organizationName = "";
            if (filter.organizationRegisterNumber == null)
                filter.organizationRegisterNumber = ""; 
            if (filter.organizationPhone == null)
                filter.organizationPhone = "";
            if (filter.director == null)
                filter.director = "";
            if (filter.webSite == null)
                filter.webSite = "";
            if (filter.orderColumn == null)
                filter.orderColumn = "id";

            var companyName = new OracleParameter("company_name", OracleDbType.Varchar2, filter.organizationName, ParameterDirection.Input);
            var regNo = new OracleParameter("registration_number", OracleDbType.Varchar2, filter.organizationRegisterNumber, ParameterDirection.Input);
            var phone = new OracleParameter("mobile", OracleDbType.Varchar2, filter.organizationPhone, ParameterDirection.Input);
            var orgType = new OracleParameter("org_type", OracleDbType.Int32, filter.orgnizationType, ParameterDirection.Input);
            var ceo = new OracleParameter("ceo_name", OracleDbType.Varchar2, filter.director, ParameterDirection.Input);
            var web = new OracleParameter("web_site", OracleDbType.Varchar2, filter.webSite, ParameterDirection.Input);
            var enabled = new OracleParameter("enable_d", OracleDbType.Int16, filter.enabled, ParameterDirection.Input);
            var orderColumn = new OracleParameter("order_column", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            var startRow = new OracleParameter("start_row", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            var rowCount = new OracleParameter("row_count", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            int totalCount = Database.SqlQuery<int>("BEGIN GET_ORGANIZATIONS_TOTAL("
                                                                               + ":COMPANY_NAME,"
                                                                               + ":REG_NO,"
                                                                               + ":MOBILE,"
                                                                               + ":ORG_TYPE,"
                                                                               + ":CEO_NAME,"
                                                                               + ":WEB_SITE,"
                                                                               + ":ENABLE_D,"
                                                                               + ":RETVAL); END;",
                                                                               companyName, regNo, phone,
                                                                               orgType, ceo, web, enabled,
                                                                                retvalue).Single();

            if (totalCount == 0) return null;

            IList<TreeCompany> organizations = Database.SqlQuery<TreeCompany>("BEGIN GET_ORGANIZATIONS("
                                                                               + ":COMPANY_NAME,"
                                                                               + ":REG_NO,"
                                                                               + ":MOBILE,"
                                                                               + ":ORG_TYPE,"
                                                                               + ":CEO_NAME,"
                                                                               + ":WEB_SITE,"
                                                                               + ":ENABLE_D,"
                                                                               + ":ORDER_COLUMN,"
                                                                               + ":START_ROW,"
                                                                               + ":ROW_COUNT,"
                                                                               + ":RETVAL); END;",
                                                                               companyName, regNo, phone,
                                                                               orgType, ceo, web, enabled,
                                                                               orderColumn,
                                                                               startRow, rowCount, retvalue).ToList();

            foreach (var organization in organizations)
            {
                var companyId = new OracleParameter("company_name", OracleDbType.Int32, organization.ID, ParameterDirection.Input);

                organization.CHIDLCOMPANYS = Database.SqlQuery<SYSTEM_ORGANIZATION>("BEGIN GET_CHILD_ORGANIZATIONS("
                                                                                   + ":ORG_ID,"
                                                                                   + ":RETVAL); END;",
                                                                                    companyId, retvalue).ToList();
            }

            return new OrganizationListModel(totalCount, organizations);
        }

        //Get contracts by organization id
        public virtual List<MST_CONTRACT> GET_ORGANIZATION_CONTRACTS(int orgId, int orgType){          
            if (orgType == 1) {
               return (from c in MST_CONTRACT
                 where c.BUSINESSID == orgId
                 select c).ToList();
            } else if (orgType == 2) {
                return (from c in MST_CONTRACT
                        where c.STOREID == orgId
                        select c).ToList();
            }
            return null;
      }

        //Add s by organization id
        public virtual int ADD_USER_CONTRACTS(int[] contractIds, int uId)
        {
            int result = 0;

            var userId = new OracleParameter("P_USER_ID", OracleDbType.Int32, uId, ParameterDirection.Input);
            var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            int delete = Database.SqlQuery<int>("BEGIN DELETE_USER_CONTRACTS(:P_USER_ID, :RETVAL); END;", userId, retvalue).FirstOrDefault();

            if (delete > 0) {
                foreach (int id in contractIds) {
                    var userContrcatId = new OracleParameter("P_CONTRACT_USER_ID", OracleDbType.Int32, ParameterDirection.Output);
                    var contractId = new OracleParameter("P_CONTRACT_ID", OracleDbType.Int32, id, ParameterDirection.Input);
                    
                    result = Database.SqlQuery<int>("BEGIN ADD_USER_CONTRACT(:CONTRACT_USER_ID,:USER_ID, :P_CONTRACT_ID,:RETVAL); END;", userContrcatId, userId, contractId,retvalue).FirstOrDefault();
                    if (result == 0) break;
                }

            }

            return result;
        }


        // Get feeback list by dates filter
        public virtual FeedbackListModel GET_FEEDBACKS(FeedbackFilterView filter)
        {
            return new FeedbackListModel(0, null);
            //UserListModel result;
            //if (filter.orderColumn == null) { filter.orderColumn = "id"; }

            //if (filter.endDate.Year == 1)
            //{
            //    filter.endDate = new DateTime(9999, 12, 31);
            //};

            //var feedbackName = new OracleParameter("p_feedback_name", OracleDbType.Varchar2, filter.feedbackName, ParameterDirection.Input);
            //var startDate = new OracleParameter("p_start_date", OracleDbType.Date, filter.startDate, ParameterDirection.Input);
            //var endDate = new OracleParameter("p_end_date", OracleDbType.Date, filter.endDate, ParameterDirection.Input);
            //var orderColumn = new OracleParameter("order_column", OracleDbType.Varchar2, filter.orderColumn, ParameterDirection.Input);
            //var startRow = new OracleParameter("start_row", OracleDbType.Int32, filter.startRow, ParameterDirection.Input);
            //var rowCount = new OracleParameter("row_count", OracleDbType.Int32, filter.rowCount, ParameterDirection.Input);
            //var retvalue = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

            //int totalCount = Database.SqlQuery<int>("BEGIN GET_FEEDBACKS_TOTAL("
            //                                                                   + ":P_FEEDBACK_NAME,"
            //                                                                   + ":P_START_DATE,"
            //                                                                   + ":P_END_DATE,"
            //                                                                   + ":RETVAL); END;",
            //                                                                   feedbackName,startDate,
            //                                                                   endDate,retvalue)
            //                                                                   .Single();

            //if (totalCount == 0) return null;

            //IList<SYSTEM_FEEDBACK> feedbacks = Database.SqlQuery<SYSTEM_FEEDBACK>("BEGIN GET_FEEDBACKS("
            //                                                                   + ":P_FEEDBACK_NAME,"
            //                                                                   + ":P_START_DATE,"
            //                                                                   + ":P_END_DATE,"
            //                                                                   + ":ORDER_COLUMN,"
            //                                                                   + ":START_ROW,"
            //                                                                   + ":ROW_COUNT,"
            //                                                                   + ":RETVAL); END;",
            //                                                                   feedbackName,startDate, 
            //                                                                   endDate,orderColumn,
            //                                                                   startRow, rowCount, 
            //                                                                   retvalue).ToList();
            
            //return new FeedbackListModel(totalCount, feedbacks);
        }



        #endregion

        #region Initialize
        public OracleDbContext(string cString) : base(cString)
        {
            Configuration.LazyLoadingEnabled = false;
            

        }

        public void SaveLogger(HttpContext context)
        {
          //  CompanyLogUtils.SaveCompanyLog(context);
        }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //For Oracle is neccesary
            Database.SetInitializer<OracleDbContext>(null);
            modelBuilder.HasDefaultSchema("EDISYSTEM");
            
           
            var aa = 0;
        }


        #endregion

        #region Methods
        public decimal GetTableID(string tablename)
        {
            try
            {
                return Convert.ToDecimal(Database.SqlQuery<decimal>($"SELECT {tablename}_seq.nextval nextval FROM dual").Single());
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        #endregion


    }





    public class ReflectionPopulator<T>
    {
        public virtual List<T> CreateList(OracleDataReader reader)
        {
            var results = new List<T>();
            var properties = typeof(T).GetProperties();

            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var property in typeof(T).GetProperties())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                    {
                        Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        property.SetValue(item, Convert.ChangeType(reader[property.Name], convertTo), null);
                    }
                }
                results.Add(item);
            }
            return results;
        }
    }

  

}
