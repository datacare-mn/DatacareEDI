using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI.Helpers
{
    public static class QueryList
    {
        public static string BusinessSkuList = "SELECT SK.SKUID, SKUCD, SKUNAME, MGLNAME, BILLNAME, MAKEDBY, \"SIZE\", BOXWEIGHT, BOXQTY, BOXCBM, DESCRIPTION, SB.BRANDNAME, SM.MEASURENAME,  " +
                                             " KEEPUNIT || ' ' ||SKU.TYPENAME KEEPVALUE,  " +
                                             " SO.ORIGINNAME,  " +
                                             " SC.COLORNAME,  " +
                                             " MODELNO,  " +
                                             " DECODE(BSS.ISACTIVE, 1,'Идэвхитэй', 'Идэвхигүй') ISACTIVE, " +
                                             " ISCALVAT,  " +
                                             " DECODE(BALANCE, 1, 'Бэлэн', 'Дууссан') balance, " +
                                             " nvl(stre.strcnt, 0) storecount, " +
                                             " SK.INSYMD,  " +
                                             " BCU.FULLNAME INSEMP " +
                                             " FROM SYS_SKU SK  " +
                                             " RIGHT JOIN (SELECT SKUID, SB.ISACTIVE, BALANCE FROM SKU_BUSINESS SB WHERE COMID = :P_COMID) BSS " +
                                             " ON SK.SKUID = BSS.SKUID  " +
                                             " LEFT JOIN SYS_BRAND SB  " +
                                             " ON SB.BRANDID = SK.BRANDID " +
                                             " LEFT JOIN SYS_MEASURE SM  " +
                                             " ON SK.MEASURE = SM.MEASUREID " +
                                             " LEFT JOIN SYS_KEEP_UNITTYPE SKU " +
                                             " ON SKU.TYPEID = SK.KEEPTYPE  " +
                                             " LEFT JOIN SYS_ORIGIN SO  " +
                                             " ON SK.ORIGINID = SO.ORIGINID " +
                                             " LEFT  JOIN SYS_COLORS SC  " +
                                             " ON SK.COLOR = SC.COLORID  " +
                                             " LEFT  JOIN BIZ_COM_USER   bcu " +
                                             " ON SK.INSEMP	=	BCU.USERID " +
                                             " left JOIN (SELECT count(*) strcnt, skuid FROM BIZ_STORE_SKU bss " +
                                             " LEFT JOIN (SELECT * FROM  VIEW_ALL_STORES vas) str " +
                                             " ON bss.STOREID = str.COMID " +
                                             " WHERE bss.COMID = :p_comid " +
                                             " GROUP BY skuid) stre " +
                                             " ON sk.SKUID = stre.SKUID ";
    }
}
