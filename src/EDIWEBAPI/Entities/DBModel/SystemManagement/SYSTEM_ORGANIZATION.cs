using EDIWEBAPI.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{

    [JsonObject(Title = "organzation")]
    public class SYSTEM_ORGANIZATION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public string COMPANYNAME { get; set; }
        [Index(IsUnique = true)]
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
        public ORGTYPE?  ORGTYPE { get; set; }
        public int? ENABLED { get; set; }
        public int? PARENTID { get; set; }
        public string PARENTREGNO { get; set; }
        public int ISFOREIGN { get; set; }

        //  public List<MST_CONTRACT> Contract { get; set; }

    }
}
