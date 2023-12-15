
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.LicenseConfig
{
    public class SYSTEM_LICENSE_USER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal ID { get; set; }
        public decimal? PARENTID { get; set; }
        public int YEAR { get; set; }
        public string MONTH { get; set; }
        public decimal YEARANDMONTH { get; set; }
        public int BUSINESSID { get; set; }
        public int USERID { get; set; }
        public int LICENSEID { get; set; }
        public decimal? PRICE { get; set; }
        public int ENABLED { get; set; }
        public int ANNUAL { get; set; }
        public int QTY { get; set; }
        public decimal? AMOUNT { get; set; }

        public DateTime? ENABLEDDATE { get; set; }
        public DateTime? DISABLEDDATE { get; set; }

        public void SetAnnual(bool annual, RequestModels.UserLicenseRequest license)
        {
            this.ANNUAL = annual ? 1 : 2;
            this.QTY = annual ?
                // ХЭРВЭЭ ЖИЛЭЭР АВСАН ТАЙЛАНГАА ТУХАЙН АВСАН САРДАА АШИГЛААГҮЙ БОЛ МӨНГӨ АВАХГҮЙ
                license.PARENTID.HasValue ? 0 : (license.VALUEANNUAL == 1 ? license.DEFAULTQTY : (license.USAGEQTY > 0 ? license.DEFAULTQTY : 0)) :
                // ХЭРВЭЭ САРААР АВСАН ТАЙЛАНГАА ЦУЦЛАЖ БАЙГАА ТОХИОЛДОЛД ТАЙЛАНГ АШИГЛААГҮЙ БОЛ МӨНГӨ АВАХГҮЙ
                (license.VALUE != 1 ? (license.USAGEQTY > 0 ? 1 : 0) : 1);
            this.PRICE = annual ? license.ANNUALPRICE : license.PRICE;
            SetEnabled(annual ? license.VALUEANNUAL : license.VALUE);
            SetAmount();
        }

        public void SetAmount()
        {
            this.AMOUNT = this.QTY * this.PRICE;
        }

        public void SetEnabled(int enabled)
        {
            this.ENABLED = enabled;
            if (this.ENABLED == 1)
                ENABLEDDATE = DateTime.Now;
            else
                DISABLEDDATE = DateTime.Now;
        }
    }
}
