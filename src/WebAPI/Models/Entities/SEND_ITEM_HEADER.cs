using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Interfaces;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    public class SEND_ITEM_HEADER : AuditableEntity<long>, IDescribableEntity
    {
        [Key]
        [Required]
        public int HEADERID { get; set; }
        [Required]
        public int FROMCOMID { get; set; }
        [Required]
        public int TOCOMID { get; set; }
        [Required]
        public DateTime SENDDATE { get; set; }

        public int CONTRACTID { get; set; }

        public DateTime INDATE { get; set; }

        public DateTime APPROVEDATE { get; set; }

        public int APPROVEUSER { get; set; }

        public string HEADERTYPE { get; set; }
        [Required]
        public int STATUS { get; set; }

        public string Describe()
        {
            return "{  INSYMD : \"" + INSYMD + "\", INSEMP : \"" + INSEMP + "\", UPDYMD : \"" + UPDYMD + "\", UPDEMP : \"" + UPDEMP + "\", HEADERID : \"" + HEADERID + "\", FROMCOMID : \"" + FROMCOMID + "\", TOCOMID : \"" + TOCOMID + "\", SENDDATE : \"" + SENDDATE + "\", INDATE : \"" + INDATE + "\", CONTRACTID : \"" + CONTRACTID + "\", APPROVEDATE : \"" + APPROVEDATE + "\", APPROVEUSER : \"" + APPROVEUSER + "\", HEADERTYPE : \"" + HEADERTYPE + "\", STATUS : \"" + STATUS + "\"}";
        }
    }
}
