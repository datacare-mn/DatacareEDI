using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class VendorCompany 
    {
        public VendorCompany(SYSTEM_ORGANIZATION source)
        {
            this.ADDRESS = source.ADDRESS;
            this.CEONAME = source.CEONAME;
            this.COMPANYNAME = source.COMPANYNAME;
            this.EMAIL = source.EMAIL;
            this.ENABLED = source.ENABLED;
            this.FBADDRESS = source.FBADDRESS;
            this.ID = source.ID;
            this.LATITUDE = source.LATITUDE;
            this.LOGO = source.LOGO;
            this.LONGITUDE = source.LONGITUDE;
            this.MOBILE = source.MOBILE;
            this.ORGTYPE = source.ORGTYPE;
            this.PARENTID = source.PARENTID;
            this.REGNO = source.REGNO;
            this.SLOGAN = source.SLOGAN;
            this.WEBSITE = source.WEBSITE;
        }

        public int ID { get; set; }

        public string COMPANYNAME { get; set; }

        public string REGNO { get; set; }

        public string ADDRESS { get; set; }

        public string CEONAME { get; set; }

        public string WEBSITE { get; set; }

        public string EMAIL { get; set; }

        public string FBADDRESS { get; set; }

        public string LONGITUDE { get; set; }

        public string LATITUDE { get; set; }

        public string MOBILE { get; set; }

        public string SLOGAN { get; set; }

        public string LOGO { get; set; }

        public ORGTYPE? ORGTYPE { get; set; }

        public int? ENABLED { get; set; }

        public int? PARENTID { get; set; }

        public string PARENTREGNO { get; set; }
        public List<MST_CONTRACT> Contracts { get; set; }
    }
}
