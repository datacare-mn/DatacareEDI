
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
    [Table("SYS_SKU")]
    public class SYS_SKU : AuditableEntity<long>, IDescribableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int SKUID { get; set; }
        [Required]
        public int ISCALVAT { get; set; }

        public string SKUCD { get; set; }
        [Required]
        public string SKUNAME { get; set; }

        public string MGLNAME { get; set; }

        public string BILLNAME { get; set; }
        [Required]
        public int BRANDID { get; set; }
        [Required]
        public int ORIGINID { get; set; }

        public string MODELNO { get; set; }

        public int COLOR { get; set; }

        public string MAKEDBY { get; set; }
        [Required]
        public int UOM { get; set; }

        public int WEIGHT { get; set; }

        public int MEASURE { get; set; }

        public int KEEPUNIT { get; set; }

        public int KEEPTYPE { get; set; }

        public int SIZE { get; set; }

        public string BOXCODE { get; set; }

        public int BOXWEIGHT { get; set; }

        public int BOXQTY { get; set; }

        public int BOXCBM { get; set; }

        public string DESCRIPTION { get; set; }

        public string Describe()
        {
            return "{  ISCALVAT : \"" + ISCALVAT + "\", SKUID : \"" + SKUID + "\", SKUCD : \"" + SKUCD + "\", SKUNAME : \"" + SKUNAME + "\", MGLNAME : \"" + MGLNAME + "\", BILLNAME : \"" + BILLNAME + "\", BRANDID : \"" + BRANDID + "\", ORIGINID : \"" + ORIGINID + "\", MODELNO : \"" + MODELNO + "\", COLOR : \"" + COLOR + "\", MAKEDBY : \"" + MAKEDBY + "\", UOM : \"" + UOM + "\", WEIGHT : \"" + WEIGHT + "\", MEASURE : \"" + MEASURE + "\", KEEPUNIT : \"" + KEEPUNIT + "\", KEEPTYPE : \"" + KEEPTYPE + "\", SIZE : \"" + SIZE + "\", BOXCODE : \"" + BOXCODE + "\", BOXWEIGHT : \"" + BOXWEIGHT + "\", BOXQTY : \"" + BOXQTY + "\", BOXCBM : \"" + BOXCBM + "\", DESCRIPTION : \"" + DESCRIPTION + "\", INSYMD : \"" + INSYMD + "\", INSEMP : \"" + INSEMP + "\", UPDYMD : \"" + UPDYMD + "\", UPDEMP : \"" + UPDEMP + "\"}";
        }
    }
}
