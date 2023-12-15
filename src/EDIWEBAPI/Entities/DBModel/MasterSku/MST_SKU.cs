﻿using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static EDIWEBAPI.Enums.SystemEnums;

namespace EDIWEBAPI.Entities.DBModel.MasterSku
{
    public class MST_SKU
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int SKUID { get; set; }

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

        public string COLOR { get; set; }

        public string MAKEDBY { get; set; }
        [Required]
        public int UOMID { get; set; }

        public int WEIGHT { get; set; }

        public int? MEASURE { get; set; }

        public int? KEEPUNIT { get; set; }

        public int KEEPUNITVALUE { get; set; }

        public string SKUSIZE { get; set; }

        public string BOXCODE { get; set; }

        public int BOXWEIGHT { get; set; }

        public int BOXQTY { get; set; }

        public int BOXCBM { get; set; }

        public string DESCRIPTION { get; set; }

        public DateTime? INSYMD { get; set; }

        public int INSEMP { get; set; }

        public DateTime? UPDYMD { get; set; }

        public int UPDEMP { get; set; }

        public int? ISCALVAT { get; set; }

        public int UOMVALUE { get; set; }

        public int MEASUREVALUE { get; set; }
        [Required]
        public int ORGID { get; set; }

        public int? ISACTIVE { get; set; }

        public int? BALANCE { get; set; }

        [ForeignKey("ORGID")]
        public virtual SYSTEM_ORGANIZATION SYSTEM_ORGANIZATION { get; set; }
        [ForeignKey("ORIGINID")]
        public virtual MST_ORIGIN MST_ORIGIN { get; set; }
        [ForeignKey("BRANDID")]
        public virtual MST_BRAND MST_BRAND { get; set; }
        [ForeignKey("UOMID")]
        public virtual MST_UOM MST_UOM { get; set; }
    }
}
