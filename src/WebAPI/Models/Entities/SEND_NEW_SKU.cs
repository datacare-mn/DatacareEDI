using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Interfaces;
using WebAPI.Models.CustomModels;

namespace WebAPI.Models.Entities
{
    public class SEND_NEW_SKU : AuditableEntity<long>, IDescribableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        [Required]
        public int SKUID { get; set; }
        [Required]
        public int HEADERID { get; set; }
        [Required]
        public int STOREID { get; set; }
        [Required]
        public int DEPID { get; set; }
        [Required]
        public int CLASSID { get; set; }
        [Required]
        public int SUBCLASSID { get; set; }

        public int SALEPRICE { get; set; }

        public int SUPPLYPRICE { get; set; }

        public int BRANCHID { get; set; }

        public DateTime APPROVEDATE { get; set; }

        public int APPROVEUSER { get; set; }
        [Required]
        public int STATUS { get; set; }

        public string Describe()
        {
            return "{  ID : \"" + ID + "\", SKUID : \"" + SKUID + "\", HEADERID : \"" + HEADERID + "\", STOREID : \"" + STOREID + "\", DEPID : \"" + DEPID + "\", CLASSID : \"" + CLASSID + "\", SUBCLASSID : \"" + SUBCLASSID + "\", SALEPRICE : \"" + SALEPRICE + "\", SUPPLYPRICE : \"" + SUPPLYPRICE + "\", BRANCHID : \"" + BRANCHID + "\", APPROVEDATE : \"" + APPROVEDATE + "\", APPROVEUSER : \"" + APPROVEUSER + "\", STATUS : \"" + STATUS + "\"}";
        }

    }
}
